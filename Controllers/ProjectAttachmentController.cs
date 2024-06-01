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
    public partial class ProjectAttachmentController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<ProjectAttachmentController> logger;
        private readonly ILogTools logTools;


        public ProjectAttachmentController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<ProjectAttachmentController> logger
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

        // GET: ProjectAttachment/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var projectAttachments = from o in context.ProjectAttachment where (o.Project == project.Id) select o;
            if (!string.IsNullOrEmpty(searchText))
                projectAttachments = projectAttachments.Where(p =>
                    p.Title!.Contains(searchText)
                    || p.DateAttached.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projectAttachments != null)
            {
                projectAttachments = OrderBySelectedOrDefault(sortOrder, projectAttachments);
                return View(await projectAttachments.ToListAsync());
            }
            return View(new List<ProjectAttachment>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: ProjectAttachment/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectAttachment
            if ((id == null) || (id <= 0) || (context.ProjectAttachment == null)) return NotFound();
            ProjectAttachment? projectAttachment = await context.ProjectAttachment
                .FindAsync(id);
            if (projectAttachment == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectAttachment);

            return View(projectAttachment);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: ProjectAttachment/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            ProjectAttachment projectAttachment = new()
            {
                Project = project.Id
            };
            return View(projectAttachment);
        }



        // POST: ProjectAttachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectAttachment projectAttachment, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // set UploadDate
            projectAttachment.DateAttached = DateTime.Now;

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(projectAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(projectAttachment))
                    {
                        context.Update(projectAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "ProjectAttachment", "Create", projectAttachment.Id, projectAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = projectAttachment.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Anexo Proyecto";
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
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectAttachment);
            EraseFileNameFieldForAttachments(projectAttachment);

            return View(projectAttachment);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: ProjectAttachment/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectAttachment
            if ((id == null) || (id <= 0) || (context.ProjectAttachment == null)) return NotFound();
            ProjectAttachment? projectAttachment = await context.ProjectAttachment
                .FindAsync(id);
            if (projectAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromProjectAttachment(projectAttachment.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectAttachment);

            return View(projectAttachment);
        }


        // POST: ProjectAttachment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] ProjectAttachment projectAttachment, string? returnUrl, string? bufferedUrl)
        {
            if (id != projectAttachment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(projectAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(projectAttachment))
                    {
                        context.Update(projectAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "ProjectAttachment", "Edit", projectAttachment.Id, projectAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProjectAttachmentExists(projectAttachment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Anexo Proyecto. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Anexo Proyecto";
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
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectAttachment);

            EraseFileNameFieldForAttachments(projectAttachment);

            return View(projectAttachment);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: ProjectAttachment/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.ProjectAttachment == null)) return NotFound();
            ProjectAttachment? projectAttachment = await context.ProjectAttachment
                .FirstAsync(r => r.Id == id);
            if (projectAttachment == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (projectAttachment != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.ProjectAttachment.Remove(projectAttachment);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "ProjectAttachment", "Delete", projectAttachment.Id, projectAttachment
                        , new List<string> { "FileNameInput" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Anexo Proyecto";
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

            string path = fileLoadService.ServerFullPath(FileLoadService.PathProjectAttachments);
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
        /// Sorts projectAttachments by default (DateAttached) or selected order:
        /// title, fileName or dateAttached
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of ProjectAttachment</returns>
        IQueryable<ProjectAttachment> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ProjectAttachment> projectAttachments)
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
                    projectAttachments = projectAttachments.OrderByDescending(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-up-fill";
                    break;
                case "title":
                    projectAttachments = projectAttachments.OrderBy(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-down-fill";
                    break;
                case "fileName_desc":
                    projectAttachments = projectAttachments.OrderByDescending(o => o.FileName);
                    ViewBag.fileNameIcon = "bi-caret-up-fill";
                    break;
                case "fileName":
                    projectAttachments = projectAttachments.OrderBy(o => o.FileName);
                    ViewBag.fileNameIcon = "bi-caret-down-fill";
                    break;
                case "dateAttached_desc":
                    projectAttachments = projectAttachments.OrderByDescending(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-up-fill";
                    break;
                default:
                    projectAttachments = projectAttachments.OrderBy(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-down-fill";
                    break;
            }
            return projectAttachments;
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
            // navProjectAttachment
            ViewBag.navProjectAttachment = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Project
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(ProjectAttachment? projectAttachment)
        {

            // set options for Project
            var listProject = new SelectList(context.Project
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", projectAttachment?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProject = listProject;
        }


        //======================================================== 


        // makes shure active project matches the project for the ProjectAttachment
        async Task<int> GetProjectIdFromProjectAttachment(int projectAttachmentId)
        {
            var projectId = await context.ProjectAttachment
                                .Where(r => r.Id == projectAttachmentId)
                                .Select(r => r.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(ProjectAttachment projectAttachment)
        {
            bool updated = false;

            //---- FileName ----
            if (projectAttachment.FileNameInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("FileName" ,projectAttachment.Project, projectAttachment.Id, projectAttachment.FileNameInput);
                int result = await fileLoadService.UploadFile(serverFileName, projectAttachment.FileNameInput, FileLoadService.PathProjectAttachments);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(projectAttachment.FileName, serverFileName);
                // set
                projectAttachment.FileName = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(ProjectAttachment projectAttachment)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var fileNameErrors = ModelState[nameof(projectAttachment.FileName)]?.Errors;           
            if (fileNameErrors == null || fileNameErrors.Count == 0)
            {
                ModelState.Remove(nameof(projectAttachment.FileName));
                projectAttachment.FileName = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool ProjectAttachmentExists(int id)
        {
            return (context.ProjectAttachment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
