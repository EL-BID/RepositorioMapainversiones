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
    public partial class ExtensionController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<ExtensionController> logger;
        private readonly ILogTools logTools;


        public ExtensionController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<ExtensionController> logger
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

        // GET: Extension/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var extensions = from o in context.Extension where (o.Project == project.Id) select o;
            if (!string.IsNullOrEmpty(searchText))
                extensions = extensions.Where(p =>
                    p.Code!.Contains(searchText)
                    || p.Days.ToString()!.Contains(searchText)
                    || p.DateDelivery.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (extensions != null)
            {
                extensions = OrderBySelectedOrDefault(sortOrder, extensions);
                return View(await extensions.ToListAsync());
            }
            return View(new List<Extension>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: Extension/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Extension
            if ((id == null) || (id <= 0) || (context.Extension == null)) return NotFound();
            Extension? extension = await context.Extension
                                    .Include(t => t.ExtensionAttachments!)
                .FirstAsync(r => r.Id == id);
            if (extension == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(extension);

            return View(extension);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Extension/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            Extension extension = new()
            {
                Project = project.Id
            };
            return View(extension);
        }



        // POST: Extension/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Extension extension, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // Check if is code unique
            if (context.Extension.Any(c => c.Code == extension.Code && c.Id != extension.Id))
            {
                ModelState.AddModelError("Code", "Código existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(extension);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(extension))
                    {
                        context.Update(extension);
                        await context.SaveChangesAsync();
                    }

                    // Assign Code when created
                    extension.Code = (extension.Id + 1000).ToString("d5");
                    context.Update(extension);
                    await context.SaveChangesAsync();

                    // Commit Transaction
                    logTools.Log(User, "Extension", "Create", extension.Id, extension
                        , new List<string> { "AttachmentInput","Attachmentses" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = extension.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Extensión";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.DaysValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Days");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(extension);
            EraseFileNameFieldForAttachments(extension);

            return View(extension);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Extension/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Extension
            if ((id == null) || (id <= 0) || (context.Extension == null)) return NotFound();
            Extension? extension = await context.Extension
                                    .Include(t => t.ExtensionAttachments!)
                .FirstAsync(r => r.Id == id);
            if (extension == null) return NotFound();

            int projectId = await GetProjectIdFromExtension(extension.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(extension);
            ViewBag.extensionId = extension.Id;

            return View(extension);
        }


        // POST: Extension/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Extension extension, string? returnUrl, string? bufferedUrl)
        {
            if (id != extension.Id)
            {
                return NotFound();
            }
            // Check if is code unique
            if (context.Extension.Any(c => c.Code == extension.Code && c.Id != extension.Id))
            {
                ModelState.AddModelError("Code", "Código existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(extension);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(extension))
                    {
                        context.Update(extension);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "Extension", "Edit", extension.Id, extension
                        , new List<string> { "AttachmentInput","Attachmentses" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ExtensionExists(extension.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Extensión. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Extensión";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.DaysValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Days");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);
            SetViewBagsForLists(extension);
            ViewBag.extensionId = extension.Id;

            EraseFileNameFieldForAttachments(extension);

            return View(extension);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Extension/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Extension == null)) return NotFound();
            Extension? extension = await context.Extension
                .Include(t => t.ExtensionAttachments!)
                .FirstAsync(r => r.Id == id);
            if (extension == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (extension != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    DeleteChildren(extension);
                    await context.SaveChangesAsync();
                    context.Extension.Remove(extension);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Extension", "Delete", extension.Id, extension
                        , new List<string> { "AttachmentInput","Attachmentses" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Extensión";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }

            //---- if not saved reload view ----
            // set ViewBags
            SetStandardViewBags("Delete", false, returnUrl, bufferedUrl);
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

            string path = fileLoadService.ServerFullPath(FileLoadService.PathExtensions);
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
        /// Sorts extensions by default (DateDelivery) or selected order:
        /// code, days or dateDelivery
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Extension</returns>
        IQueryable<Extension> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Extension> extensions)
        {
            ViewBag.dateDeliverySort = string.IsNullOrEmpty(sortOrder) ? "dateDelivery_desc" : "";
            ViewBag.codeSort = sortOrder == "code" ? "code_desc" : "code";
            ViewBag.daysSort = sortOrder == "days" ? "days_desc" : "days";
            ViewBag.codeIcon = "bi-caret-down";
            ViewBag.daysIcon = "bi-caret-down";
            ViewBag.dateDeliveryIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "code_desc":
                    extensions = extensions.OrderByDescending(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-up-fill";
                    break;
                case "code":
                    extensions = extensions.OrderBy(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-down-fill";
                    break;
                case "days_desc":
                    extensions = extensions.OrderByDescending(o => o.Days);
                    ViewBag.daysIcon = "bi-caret-up-fill";
                    break;
                case "days":
                    extensions = extensions.OrderBy(o => o.Days);
                    ViewBag.daysIcon = "bi-caret-down-fill";
                    break;
                case "dateDelivery_desc":
                    extensions = extensions.OrderByDescending(t => t.DateDelivery);
                    ViewBag.dateDeliveryIcon = "bi-caret-up-fill";
                    break;
                default:
                    extensions = extensions.OrderBy(t => t.DateDelivery);
                    ViewBag.dateDeliveryIcon = "bi-caret-down-fill";
                    break;
            }
            return extensions;
        }

        //==============================================
        //------------- Controller Methods -------------



        /// <summary>
        /// Assigns the standard ViewBags required for navigation.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="returnsbyDefault">If false. The return button will only be available if returnUrl has a value.
        /// If true, will return to caller if no returnUrl is specified.</param>
        void SetStandardViewBags(string action, bool returnsbyDefault, string? returnUrl, string? bufferedUrl)
        {
            // returnUrl
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.returnUrl = returnUrl;
            else if (returnsbyDefault)
                ViewBag.returnUrl = HttpContext.Request.Headers["Referer"];
            // bufferedUrl
            if (!string.IsNullOrEmpty(bufferedUrl)) ViewBag.bufferedUrl = bufferedUrl;
            // navExtension
            ViewBag.navExtension = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Project and Stage
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(Extension? extension)
        {

            // set options for Project
            var listProject = new SelectList(context.Project
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", extension?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProject = listProject;

            // set options for Stage
            var listStage = new SelectList(context.TaskStage
                               .OrderBy(c => c.Order)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", extension?.Stage).ToList();
            listStage.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listStage = listStage;
        }


        //======================================================== 


        // makes shure active project matches the project for the Extension
        async Task<int> GetProjectIdFromExtension(int extensionId)
        {
            var projectId = await context.Extension
                                .Where(r => r.Id == extensionId)
                                .Select(r => r.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        // delete children

        /// <summary>
        /// Deletes all children records related to the Extension:
        /// extensionAttachment
        /// </summary>
        /// <param name="project"></param>
        void DeleteChildren(Extension extension)
        {
            if (extension.ExtensionAttachments?.Count > 0)
                extension.ExtensionAttachments.ToList().ForEach(c => context.ExtensionAttachment.Remove(c));
        }
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(Extension extension)
        {
            bool updated = false;

            //---- Attachment ----
            if (extension.AttachmentInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("Attachment" ,extension.Project, extension.Id, extension.AttachmentInput);
                int result = await fileLoadService.UploadFile(serverFileName, extension.AttachmentInput, FileLoadService.PathExtensions);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(extension.Attachment, serverFileName);
                // set
                extension.Attachment = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(Extension extension)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var attachmentErrors = ModelState[nameof(extension.Attachment)]?.Errors;           
            if (attachmentErrors == null || attachmentErrors.Count == 0)
            {
                ModelState.Remove(nameof(extension.Attachment));
                extension.Attachment = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool ExtensionExists(int id)
        {
            return (context.Extension?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
