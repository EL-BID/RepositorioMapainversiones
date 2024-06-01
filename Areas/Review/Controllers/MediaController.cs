using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMRepo.Data;
using IMRepo.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using IMRepo.Services.basic;
using JaosLib.Services.Utilities;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;
using IMRepo.Services.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using IMRepo.Controllers;
using IMRepo.Areas.Review.Models;

namespace IMRepo.Areas.PEU.Controllers
{
    [Authorize]
    [Area("Review")]

    public class MediaController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<ProjectImageController> logger;


        public MediaController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<ProjectImageController> logger
        )
        {
            this.context = context;
            this.parentProjectService = parentProjectService;
            this.fileLoadService = fileLoadService;
            this.logger = logger;
        }


        readonly JaosLibUtils jaosLibUtils = new();



        #region Index
        //----------- Index

        // GET: ProjectImage
        public async Task<IActionResult> Index(string? sortOrder, int? projectId)
        {
            Project? project = await parentProjectService.getProjectFromIdOrSession(projectId, User, HttpContext.Session, ViewBag);
            if (project == null)
                return await Task.Run(() => RedirectToAction("Select", "Product", new { Area = "" }));
            projectId = project.Id;
            var projectImages = from o in context.ProjectImage where (o.Project == projectId) select o;
            var projectVideos = from o in context.ProjectVideo where (o.Project == projectId) select o;
            ViewBag.projectId = projectId;
            ViewBag.videos = projectVideos.ToList();
            ViewBag.navMedia = "active";
            if (projectImages != null || projectVideos != null)
            {
                if (projectImages != null)
                {
                    projectImages = OrderBySelectedOrDefault(sortOrder, projectImages);
                    projectImages = projectImages.Include(p => p.Project_info);
                }
                if (projectImages?.Any() ?? false)
                {
                    return View(await projectImages.ToListAsync());
                }
            }
            return View();
        }

        #endregion
        #region Create

        #region Create


        //----------- Create

        // GET: ProjectImage/Create
        [HttpGet]
        public IActionResult Create(int? projectId, string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, projectId, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            ProjectImages projectImages = new();
            if (projectId.HasValue) projectImages.Project = projectId.Value;
            return View(projectImages);
        }



        // POST: ProjectImage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectImages projectImages, int? projectId, string? returnUrl, string? bufferedUrl)
        {

            // set UploadDate
            projectImages.UploadDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");

                    if (projectImages.FilesInput != null && projectImages.FilesInput?.Count > 0)
                    {
                        // Loop through each file
                        foreach (var uploadedFile in projectImages.FilesInput)
                        {
                            ProjectImage projectImage = CreateUsing(projectImages);
                            var s = projectImage.Id;
                            // Insert
                            context.Add(projectImage);
                            await context.SaveChangesAsync();


                            // upload
                            string serverFileName = fileLoadService.ServerFileName("File", projectImage.Project, projectImage.Id, uploadedFile);
                            int result = await fileLoadService.UploadFile(serverFileName, uploadedFile, FileLoadService.PathProjectImages);
                            if (result != FileLoadService.resultOK)
                                throw new FileLoadService.UploadFileException(uploadedFile.FileName, serverFileName);

                            // set
                            projectImage.File = serverFileName;
                            context.Update(projectImage);
                            await context.SaveChangesAsync();

                        }

                    }

                    // Commit
                    logger.LogWarning(JsonConvert.SerializeObject(projectImages));
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    if (returnUrl != null)
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Imagen";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
                ViewBag.warningMessage = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, projectId, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectImages);
            return View(projectImages);
        }

        #endregion



        #endregion
        #region Delete

        //----------- Delete

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!(id > 0) || (context.ProjectImage == null))
                return NotFound();
            ProjectImage? projectImage = await context.ProjectImage.FindAsync(id);
            if (projectImage == null)
                return NotFound();


            if (projectImage != null)
            {
                context.ProjectImage.Remove(projectImage);

                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        //----------- Delete

        [HttpPost]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            if (!(id > 0) || (context.ProjectVideo == null))
                return NotFound();
            ProjectVideo? projectVideo = await context.ProjectVideo.FindAsync(id);
            if (projectVideo == null)
                return NotFound();

            if (projectVideo != null)
            {
                context.ProjectVideo.Remove(projectVideo);

                await context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        #endregion 

        //----------- Buttons


        [HttpPost]
        public async Task<IActionResult> Back()
        {
            return await Task.Run(() => RedirectToAction("Index"));
        }

        #region Controller Methods


        //-- Sort Table by default or user selected order
        IQueryable<ProjectImage> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ProjectImage> projectImages)
        {
            ViewBag.descriptionSort = string.IsNullOrEmpty(sortOrder) ? "description_desc" : "";
            ViewBag.descriptionIcon = "bi-caret-down";
            ViewBag.descriptionIcon = sortOrder switch
            {
                "description_desc" => "bi-caret-up-fill",
                _ => "bi-caret-down-fill",
            };
            return projectImages;
        }




        //----------- Controller Methods



        async Task<Project?> GetProject(int projectId)
        {
            if (projectId > 0)
                return await context.Project.FirstOrDefaultAsync(p => p.Id == projectId);
            return null;
        }

        #endregion
        #region Create Methods
        //==============================================
        //------------- Controller Methods -------------


        /// <summary>
        /// Assigns the standard ViewBags required for navigation.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="returnsbyDefault">If false. The return button will only be available 
        /// if returnUrl has a value.
        /// If true, will return to caller if no returnUrl is specified.</param>
        void SetStandardViewBags(string action, bool returnsbyDefault, int? projectId, string? returnUrl, string? bufferedUrl)
        {
            if (projectId.HasValue) ViewBag.projectId = projectId;
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


        void SetViewBagsForLists(ProjectImage? projectImage)
        {

            // set options for Project
            var listProject = new SelectList(context.Project.OrderBy(c => c.Name).Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", projectImage?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = "Seleccionar una opción" });
            ViewBag.listProject = listProject;
        }


        public ProjectImage CreateUsing(ProjectImages projectImages)
        {
            return new ProjectImage
            {
                Id = projectImages.Id,
                Project = projectImages.Project,
                File = projectImages.File,
                Description = projectImages.Description,
                ImageDate = projectImages.ImageDate,
                UploadDate = projectImages.UploadDate,
                Project_info = projectImages.Project_info
            };
        }

        //----------------------------------------------
        //==============================================

        #endregion
    }
}
