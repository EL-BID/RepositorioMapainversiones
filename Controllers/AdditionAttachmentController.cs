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
    public partial class AdditionAttachmentController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<AdditionAttachmentController> logger;
        private readonly ILogTools logTools;


        public AdditionAttachmentController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<AdditionAttachmentController> logger
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

        // GET: AdditionAttachment/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var additionAttachments = additionId.HasValue
                ? from o in context.AdditionAttachment where (o.Addition == additionId) select o
                : GetAdditionAttachmentsForProject(project.Id);
            if (!string.IsNullOrEmpty(searchText))
                additionAttachments = additionAttachments.Where(p =>
                    p.Title!.Contains(searchText)
                    || p.DateAttached.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, additionId, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (additionAttachments != null)
            {
                additionAttachments = OrderBySelectedOrDefault(sortOrder, additionAttachments);
                return View(await additionAttachments.ToListAsync());
            }
            return View(new List<AdditionAttachment>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: AdditionAttachment/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            // get AdditionAttachment
            if ((id == null) || (id <= 0) || (context.AdditionAttachment == null)) return NotFound();
            AdditionAttachment? additionAttachment = await context.AdditionAttachment
                .FindAsync(id);
            if (additionAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromAdditionAttachment(additionAttachment.Id);
            // set ViewBags
            SetStandardViewBags("Display", true, additionId, returnUrl, bufferedUrl);
            SetViewBagsForLists(additionAttachment, projectId);

            return View(additionAttachment);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: AdditionAttachment/Create
        [HttpGet]
        public IActionResult Create(int? additionId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, additionId, returnUrl, bufferedUrl);
            SetViewBagsForLists(null, project.Id);

            AdditionAttachment additionAttachment = new();
            if (additionId.HasValue) additionAttachment.Addition = additionId.Value;
            return View(additionAttachment);
        }



        // POST: AdditionAttachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AdditionAttachment additionAttachment, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // set UploadDate
            additionAttachment.DateAttached = DateTime.Now;

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(additionAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(additionAttachment))
                    {
                        context.Update(additionAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "AdditionAttachment", "Create", additionAttachment.Id, additionAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = additionAttachment.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Anexo Adición";
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
            SetStandardViewBags("Create", true, additionId, returnUrl, bufferedUrl);
            SetViewBagsForLists(additionAttachment, project.Id);
            EraseFileNameFieldForAttachments(additionAttachment);

            return View(additionAttachment);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: AdditionAttachment/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            // get AdditionAttachment
            if ((id == null) || (id <= 0) || (context.AdditionAttachment == null)) return NotFound();
            AdditionAttachment? additionAttachment = await context.AdditionAttachment
                .FindAsync(id);
            if (additionAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromAdditionAttachment(additionAttachment.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, additionId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(additionAttachment, projectId);

            return View(additionAttachment);
        }


        // POST: AdditionAttachment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] AdditionAttachment additionAttachment, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            if (id != additionAttachment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(additionAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(additionAttachment))
                    {
                        context.Update(additionAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "AdditionAttachment", "Edit", additionAttachment.Id, additionAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!AdditionAttachmentExists(additionAttachment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Anexo Adición. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Anexo Adición";
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
            SetStandardViewBags("Edit", false, additionId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(additionAttachment, project.Id);

            EraseFileNameFieldForAttachments(additionAttachment);

            return View(additionAttachment);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: AdditionAttachment/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.AdditionAttachment == null)) return NotFound();
            AdditionAttachment? additionAttachment = await context.AdditionAttachment
                .FirstAsync(r => r.Id == id);
            if (additionAttachment == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (additionAttachment != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.AdditionAttachment.Remove(additionAttachment);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "AdditionAttachment", "Delete", additionAttachment.Id, additionAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Anexo Adición";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }

            //---- if not saved reload view ----
            // set ViewBags
            SetStandardViewBags("Delete", false, additionId, returnUrl, bufferedUrl);
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

            string path = fileLoadService.ServerFullPath(FileLoadService.PathAdditionAttachments);
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
        /// Sorts additionAttachments by default (DateAttached) or selected order:
        /// title, fileName or dateAttached
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of AdditionAttachment</returns>
        IQueryable<AdditionAttachment> OrderBySelectedOrDefault(string? sortOrder, IQueryable<AdditionAttachment> additionAttachments)
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
                    additionAttachments = additionAttachments.OrderByDescending(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-up-fill";
                    break;
                case "title":
                    additionAttachments = additionAttachments.OrderBy(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-down-fill";
                    break;
                case "fileName_desc":
                    additionAttachments = additionAttachments.OrderByDescending(o => o.FileName);
                    ViewBag.fileNameIcon = "bi-caret-up-fill";
                    break;
                case "fileName":
                    additionAttachments = additionAttachments.OrderBy(o => o.FileName);
                    ViewBag.fileNameIcon = "bi-caret-down-fill";
                    break;
                case "dateAttached_desc":
                    additionAttachments = additionAttachments.OrderByDescending(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-up-fill";
                    break;
                default:
                    additionAttachments = additionAttachments.OrderBy(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-down-fill";
                    break;
            }
            return additionAttachments;
        }

        //==============================================
        //------------- Controller Methods -------------



        /// <summary>
        /// Assigns the standard ViewBags required for navigation.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="returnsbyDefault">If false. The return button will only be available if returnUrl has a value.
        /// If true, will return to caller if no returnUrl is specified.</param>
        void SetStandardViewBags(string action, bool returnsbyDefault, int? additionId, string? returnUrl, string? bufferedUrl)
        {
            if (additionId.HasValue) ViewBag.additionId = additionId;
            // returnUrl
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.returnUrl = returnUrl;
            else if (returnsbyDefault)
                ViewBag.returnUrl = HttpContext.Request.Headers["Referer"];
            // bufferedUrl
            if (!string.IsNullOrEmpty(bufferedUrl)) ViewBag.bufferedUrl = bufferedUrl;
            // navAdditionAttachment
            ViewBag.navAdditionAttachment = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Addition
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(AdditionAttachment? additionAttachment, int projectId)
        {

            // set options for Addition
            var listAddition = new SelectList(context.Addition
                               .Include(a => a.Product_info)
                               .Where(a => a.Product_info!.Project == projectId)
                               .OrderBy(c => c.DateDelivery)
                               .Select(r => new SelectListItem(r.Code, r.Id.ToString())), "Value", "Text", additionAttachment?.Addition).ToList();
            listAddition.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listAddition = listAddition;
        }


        //======================================================== 

        public IQueryable<AdditionAttachment> GetAdditionAttachmentsForProject(int projectId)
        {
            return context.AdditionAttachment
                       .Include(a => a.Addition_info)
                       .ThenInclude(p => p!.Product_info)
                       .Where(a => a.Addition_info!.Product_info!.Project == projectId)
            ;
        }

        // makes shure active project matches the project for the AdditionAttachment
        async Task<int> GetProjectIdFromAdditionAttachment(int additionAttachmentId)
        {
            var projectId = await context.AdditionAttachment
                                .Where(r => r.Id == additionAttachmentId)
                                .Select(r => r.Addition_info!.Product_info!.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(AdditionAttachment additionAttachment)
        {
            bool updated = false;

            //---- FileName ----
            if (additionAttachment.FileNameInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("FileName" ,additionAttachment.Addition, additionAttachment.Id, additionAttachment.FileNameInput);
                int result = await fileLoadService.UploadFile(serverFileName, additionAttachment.FileNameInput, FileLoadService.PathAdditionAttachments);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(additionAttachment.FileName, serverFileName);
                // set
                additionAttachment.FileName = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(AdditionAttachment additionAttachment)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var fileNameErrors = ModelState[nameof(additionAttachment.FileName)]?.Errors;           
            if (fileNameErrors == null || fileNameErrors.Count == 0)
            {
                ModelState.Remove(nameof(additionAttachment.FileName));
                additionAttachment.FileName = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool AdditionAttachmentExists(int id)
        {
            return (context.AdditionAttachment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
