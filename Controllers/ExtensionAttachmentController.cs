using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using NuGet.Protocol;
using Newtonsoft.Json;
using IMRepo.Data;
using IMRepo.Models.Domain;
using IMRepo.Services.Utilities;
using IMRepo.Services.basic;
using JaosLib.Services.Utilities;

namespace IMRepo.Controllers
{
    [Authorize(Roles =ProjectGlobals.registeredRoles)]
    public partial class ExtensionAttachmentController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<ExtensionAttachmentController> logger;
        private readonly ILogTools logTools;


        public ExtensionAttachmentController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<ExtensionAttachmentController> logger
            , ILogTools logTools
        )
        {
            this.context = context;
            this.parentProjectService = parentProjectService;
            this.fileLoadService = fileLoadService;
            this.logger = logger;
            this.logTools = logTools;
        }

        readonly JaosLibUtils jaosLibUtils = new();

        #region Index


        //----------- Index

        // GET: ExtensionAttachment/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var extensionAttachments = extensionId.HasValue
                ? from o in context.ExtensionAttachment where (o.Extension == extensionId) select o
                : GetExtensionAttachmentsForProject(project.Id);
            if (!string.IsNullOrEmpty(searchText))
                extensionAttachments = extensionAttachments.Where(p =>
                    p.Title!.Contains(searchText)
                    || p.DateAttached.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, extensionId, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (extensionAttachments != null)
            {
                extensionAttachments = OrderBySelectedOrDefault(sortOrder, extensionAttachments);
                return View(await extensionAttachments.ToListAsync());
            }
            return View(new List<ExtensionAttachment>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: ExtensionAttachment/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            // get ExtensionAttachment
            if ((id == null) || (id <= 0) || (context.ExtensionAttachment == null)) return NotFound();
            ExtensionAttachment? extensionAttachment = await context.ExtensionAttachment
                .FindAsync(id);
            if (extensionAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromExtensionAttachment(extensionAttachment.Id);
            // set ViewBags
            SetStandardViewBags("Display", true, extensionId, returnUrl, bufferedUrl);
            SetViewBagsForLists(extensionAttachment, projectId);

            return View(extensionAttachment);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: ExtensionAttachment/Create
        [HttpGet]
        public IActionResult Create(int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, extensionId, returnUrl, bufferedUrl);
            SetViewBagsForLists(null, project.Id);

            ExtensionAttachment extensionAttachment = new();
            if (extensionId.HasValue) extensionAttachment.Extension = extensionId.Value;
            return View(extensionAttachment);
        }



        // POST: ExtensionAttachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ExtensionAttachment extensionAttachment, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // set UploadDate
            extensionAttachment.DateAttached = DateTime.Now;

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(extensionAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(extensionAttachment))
                    {
                        context.Update(extensionAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "ExtensionAttachment", "Create", extensionAttachment.Id, extensionAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = extensionAttachment.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Anexo Extensión";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.warningMessage = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, extensionId, returnUrl, bufferedUrl);
            SetViewBagsForLists(extensionAttachment, project.Id);
            EraseFileNameFieldForAttachments(extensionAttachment);

            return View(extensionAttachment);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: ExtensionAttachment/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            // get ExtensionAttachment
            if ((id == null) || (id <= 0) || (context.ExtensionAttachment == null)) return NotFound();
            ExtensionAttachment? extensionAttachment = await context.ExtensionAttachment
                .FindAsync(id);
            if (extensionAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromExtensionAttachment(extensionAttachment.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, extensionId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(extensionAttachment, projectId);

            return View(extensionAttachment);
        }


        // POST: ExtensionAttachment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] ExtensionAttachment extensionAttachment, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            if (id != extensionAttachment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(extensionAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(extensionAttachment))
                    {
                        context.Update(extensionAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "ExtensionAttachment", "Edit", extensionAttachment.Id, extensionAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ExtensionAttachmentExists(extensionAttachment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Anexo Extensión. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Anexo Extensión";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.warningMessage = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, extensionId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(extensionAttachment, project.Id);

            EraseFileNameFieldForAttachments(extensionAttachment);

            return View(extensionAttachment);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: ExtensionAttachment/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.ExtensionAttachment == null)) return NotFound();
            ExtensionAttachment? extensionAttachment = await context.ExtensionAttachment
                .FirstAsync(r => r.Id == id);
            if (extensionAttachment == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (extensionAttachment != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.ExtensionAttachment.Remove(extensionAttachment);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "ExtensionAttachment", "Delete", extensionAttachment.Id, extensionAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Anexo Extensión";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }

            //---- if not saved reload view ----
            // set ViewBags
            SetStandardViewBags("Delete", false, extensionId, returnUrl, bufferedUrl);
            jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
            if (HttpContext.Request.Headers.TryGetValue("Referer", out StringValues referer))
                return Redirect(referer.ToString());
            else
                return RedirectToAction("Index", "Home");  
        }
        #endregion
        #region Buttons


        //----------- Buttons


        [HttpPost]
        public async Task<IActionResult> Search()
        {
            return await Task.Run(() => RedirectToAction("Index"));
        }

        public IActionResult Download(string serverFileName,string downloadName)
        {
            if (string.IsNullOrEmpty(serverFileName)) return NoContent();

            string path = fileLoadService.ServerFullPath(FileLoadService.PathExtensionAttachments);
            string pathWithFile = Path.Combine(path, serverFileName);

            // Return the file as a FileResult
            return File(System.IO.File.OpenRead(pathWithFile), "application/octet-stream", downloadName + Path.GetExtension(serverFileName));
        }

        //----------------------------------------------
        //==============================================
        //----------------------------------------------
        #endregion
        #region Supporting Methods

        /// <summary>
        /// Sorts extensionAttachments by default (DateAttached) or selected order:
        /// title, fileName or dateAttached
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of ExtensionAttachment</returns>
        IQueryable<ExtensionAttachment> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ExtensionAttachment> extensionAttachments)
        {
            ViewBag.dateAttachedSort = string.IsNullOrEmpty(sortOrder) ? "dateAttached_desc" : "";
            ViewBag.titleSort = sortOrder == "title" ? "title_desc" : "title";
            ViewBag.fileNameSort = sortOrder == "fileName" ? "fileName_desc" : "fileName";
            ViewBag.titleIcon = "bi-caret-down";
            ViewBag.fileNameIcon = "bi-caret-down";
            ViewBag.dateAttachedIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "title_desc":
                    extensionAttachments = extensionAttachments.OrderByDescending(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-up-fill";
                    break;
                case "title":
                    extensionAttachments = extensionAttachments.OrderBy(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-down-fill";
                    break;
                case "fileName_desc":
                    extensionAttachments = extensionAttachments.OrderByDescending(o => o.FileName);
                    ViewBag.fileNameIcon = "bi-caret-up-fill";
                    break;
                case "fileName":
                    extensionAttachments = extensionAttachments.OrderBy(o => o.FileName);
                    ViewBag.fileNameIcon = "bi-caret-down-fill";
                    break;
                case "dateAttached_desc":
                    extensionAttachments = extensionAttachments.OrderByDescending(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-up-fill";
                    break;
                default:
                    extensionAttachments = extensionAttachments.OrderBy(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-down-fill";
                    break;
            }
            return extensionAttachments;
        }

        //==============================================
        //------------- Controller Methods -------------



        /// <summary>
        /// Assigns the standard ViewBags required for navigation.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="returnsbyDefault">If false. The return button will only be available if returnUrl has a value.
        /// If true, will return to caller if no returnUrl is specified.</param>
        void SetStandardViewBags(string action, bool returnsbyDefault, int? extensionId, string? returnUrl, string? bufferedUrl)
        {
            if (extensionId.HasValue) ViewBag.extensionId = extensionId;
            // returnUrl
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.returnUrl = returnUrl;
            else if (returnsbyDefault)
                ViewBag.returnUrl = HttpContext.Request.Headers["Referer"];
            // bufferedUrl
            if (!string.IsNullOrEmpty(bufferedUrl)) ViewBag.bufferedUrl = bufferedUrl;
            // navExtensionAttachment
            ViewBag.navExtensionAttachment = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Extension
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(ExtensionAttachment? extensionAttachment, int projectId)
        {

            // set options for Extension
            var listExtension = new SelectList(context.Extension
                               .Where(a => a.Project == projectId)
                               .OrderBy(c => c.DateDelivery)
                               .Select(r => new SelectListItem(r.Code + " - " + ((r.DateDelivery.HasValue == true) ? r.DateDelivery.Value.ToString("yyyy-MMM-dd") : ""), r.Id.ToString())), "Value", "Text", extensionAttachment?.Extension).ToList();
            listExtension.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listExtension = listExtension;
        }


        //======================================================== 

        public IQueryable<ExtensionAttachment> GetExtensionAttachmentsForProject(int projectId)
        {
            return context.ExtensionAttachment
                       .Include(a => a.Extension_info)
                       .Where(a => a.Extension_info!.Project == projectId)
            ;
        }

        // makes shure active project matches the project for the ExtensionAttachment
        async Task<int> GetProjectIdFromExtensionAttachment(int extensionAttachmentId)
        {
            var projectId = await context.ExtensionAttachment
                                .Where(r => r.Id == extensionAttachmentId)
                                .Select(r => r.Extension_info!.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(ExtensionAttachment extensionAttachment)
        {
            bool updated = false;

            //---- FileName ----
            if (extensionAttachment.FileNameInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("FileName" ,extensionAttachment.Extension, extensionAttachment.Id, extensionAttachment.FileNameInput);
                int result = await fileLoadService.UploadFile(serverFileName, extensionAttachment.FileNameInput, FileLoadService.PathExtensionAttachments);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(extensionAttachment.FileName, serverFileName);
                // set
                extensionAttachment.FileName = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(ExtensionAttachment extensionAttachment)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var fileNameErrors = ModelState[nameof(extensionAttachment.FileName)]?.Errors;           
            if (fileNameErrors == null || fileNameErrors.Count == 0)
            {
                ModelState.Remove(nameof(extensionAttachment.FileName));
                extensionAttachment.FileName = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool ExtensionAttachmentExists(int id)
        {
            return (context.ExtensionAttachment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
