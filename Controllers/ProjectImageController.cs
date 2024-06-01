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
    public partial class ProjectImageController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<ProjectImageController> logger;
        private readonly ILogTools logTools;


        public ProjectImageController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<ProjectImageController> logger
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

        // GET: ProjectImage/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var projectImages = from o in context.ProjectImage where (o.Project == project.Id) select o;
            if (!string.IsNullOrEmpty(searchText))
                projectImages = projectImages.Where(p => p.Description!.Contains(searchText));

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projectImages != null)
            {
                projectImages = OrderBySelectedOrDefault(sortOrder, projectImages);
                return View(await projectImages.ToListAsync());
            }
            return View(new List<ProjectImage>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: ProjectImage/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectImage
            if ((id == null) || (id <= 0) || (context.ProjectImage == null)) return NotFound();
            ProjectImage? projectImage = await context.ProjectImage
                .FindAsync(id);
            if (projectImage == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectImage);

            return View(projectImage);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: ProjectImage/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            ProjectImage projectImage = new()
            {
                Project = project.Id
            };
            return View(projectImage);
        }



        // POST: ProjectImage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectImage projectImage, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // set UploadDate
            projectImage.UploadDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(projectImage);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(projectImage))
                    {
                        context.Update(projectImage);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "ProjectImage", "Create", projectImage.Id, projectImage
                        , new List<string> { "FileInput" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = projectImage.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Imagen";
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
            SetViewBagsForLists(projectImage);
            EraseFileNameFieldForAttachments(projectImage);

            return View(projectImage);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: ProjectImage/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectImage
            if ((id == null) || (id <= 0) || (context.ProjectImage == null)) return NotFound();
            ProjectImage? projectImage = await context.ProjectImage
                .FindAsync(id);
            if (projectImage == null) return NotFound();

            int projectId = await GetProjectIdFromProjectImage(projectImage.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectImage);

            return View(projectImage);
        }


        // POST: ProjectImage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] ProjectImage projectImage, string? returnUrl, string? bufferedUrl)
        {
            if (id != projectImage.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(projectImage);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(projectImage))
                    {
                        context.Update(projectImage);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "ProjectImage", "Edit", projectImage.Id, projectImage
                        , new List<string> { "FileInput" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProjectImageExists(projectImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Imagen. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Imagen";
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
            SetViewBagsForLists(projectImage);

            EraseFileNameFieldForAttachments(projectImage);

            return View(projectImage);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: ProjectImage/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.ProjectImage == null)) return NotFound();
            ProjectImage? projectImage = await context.ProjectImage
                .FirstAsync(r => r.Id == id);
            if (projectImage == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (projectImage != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.ProjectImage.Remove(projectImage);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "ProjectImage", "Delete", projectImage.Id, projectImage
                        , new List<string> { "FileInput" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Imagen";
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

            string path = fileLoadService.ServerFullPath(FileLoadService.PathProjectImages);
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
        /// Sorts projectImages by default (Description) or selected order:
        /// description
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of ProjectImage</returns>
        IQueryable<ProjectImage> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ProjectImage> projectImages)
        {
            ViewBag.descriptionSort = string.IsNullOrEmpty(sortOrder) ? "description_desc" : "";
            ViewBag.descriptionIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "description_desc":
                    projectImages = projectImages.OrderByDescending(t => t.ImageDate);
                    ViewBag.descriptionIcon = "bi-caret-up-fill";
                    break;
                default:
                    projectImages = projectImages.OrderBy(t => t.ImageDate);
                    ViewBag.descriptionIcon = "bi-caret-down-fill";
                    break;
            }
            return projectImages;
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
            // navProjectImage
            ViewBag.navProjectImage = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Project
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(ProjectImage? projectImage)
        {

            // set options for Project
            var listProject = new SelectList(context.Project
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", projectImage?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProject = listProject;
        }


        //======================================================== 


        // makes shure active project matches the project for the ProjectImage
        async Task<int> GetProjectIdFromProjectImage(int projectImageId)
        {
            var projectId = await context.ProjectImage
                                .Where(r => r.Id == projectImageId)
                                .Select(r => r.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(ProjectImage projectImage)
        {
            bool updated = false;

            //---- File ----
            if (projectImage.FileInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("File" ,projectImage.Project, projectImage.Id, projectImage.FileInput);
                int result = await fileLoadService.UploadFile(serverFileName, projectImage.FileInput, FileLoadService.PathProjectImages);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(projectImage.File, serverFileName);
                // set
                projectImage.File = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(ProjectImage projectImage)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var fileErrors = ModelState[nameof(projectImage.File)]?.Errors;           
            if (fileErrors == null || fileErrors.Count == 0)
            {
                ModelState.Remove(nameof(projectImage.File));
                projectImage.File = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool ProjectImageExists(int id)
        {
            return (context.ProjectImage?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
