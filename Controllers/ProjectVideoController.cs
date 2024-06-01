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
    public partial class ProjectVideoController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly ILogger<ProjectVideoController> logger;
        private readonly ILogTools logTools;


        public ProjectVideoController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , ILogger<ProjectVideoController> logger
            , ILogTools logTools
        )
        {
            this.context = context;
            this.parentProjectService = parentProjectService;
            this.logger = logger;
            this.logTools = logTools;
        }

        readonly JaosLibUtils jaosLibUtils = new();

        #region Index


        //----------- Index

        // GET: ProjectVideo/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var projectVideos = from o in context.ProjectVideo where (o.Project == project.Id) select o;
            if (!string.IsNullOrEmpty(searchText))
                projectVideos = projectVideos.Where(p =>
                    p.Description!.Contains(searchText)
                    || p.UploadDate.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projectVideos != null)
            {
                projectVideos = OrderBySelectedOrDefault(sortOrder, projectVideos);
                return View(await projectVideos.ToListAsync());
            }
            return View(new List<ProjectVideo>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: ProjectVideo/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectVideo
            if ((id == null) || (id <= 0) || (context.ProjectVideo == null)) return NotFound();
            ProjectVideo? projectVideo = await context.ProjectVideo
                .FindAsync(id);
            if (projectVideo == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectVideo);

            return View(projectVideo);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: ProjectVideo/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            ProjectVideo projectVideo = new()
            {
                Project = project.Id
            };
            return View(projectVideo);
        }



        // POST: ProjectVideo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectVideo projectVideo, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // Ajustar link de YouTube y asignar fecha de cargue
            projectVideo.UploadDate = DateTime.Now;
            if (!string.IsNullOrEmpty(projectVideo.Link))
                projectVideo.Link = projectVideo.Link.Replace("/watch?v=", "/embed/");

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(projectVideo);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "ProjectVideo", "Create", projectVideo.Id, projectVideo);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = projectVideo.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Video";
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
            SetViewBagsForLists(projectVideo);
            return View(projectVideo);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: ProjectVideo/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectVideo
            if ((id == null) || (id <= 0) || (context.ProjectVideo == null)) return NotFound();
            ProjectVideo? projectVideo = await context.ProjectVideo
                .FindAsync(id);
            if (projectVideo == null) return NotFound();

            int projectId = await GetProjectIdFromProjectVideo(projectVideo.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectVideo);

            return View(projectVideo);
        }


        // POST: ProjectVideo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] ProjectVideo projectVideo, string? returnUrl, string? bufferedUrl)
        {
            if (id != projectVideo.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(projectVideo);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "ProjectVideo", "Edit", projectVideo.Id, projectVideo);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProjectVideoExists(projectVideo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Video. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Video";
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
            SetViewBagsForLists(projectVideo);

            return View(projectVideo);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: ProjectVideo/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.ProjectVideo == null)) return NotFound();
            ProjectVideo? projectVideo = await context.ProjectVideo
                .FirstAsync(r => r.Id == id);
            if (projectVideo == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (projectVideo != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.ProjectVideo.Remove(projectVideo);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "ProjectVideo", "Delete", projectVideo.Id, projectVideo);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Video";
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


        //----------------------------------------------
        //==============================================
        //----------------------------------------------
        #endregion
        #region Supporting Methods

        /// <summary>
        /// Sorts projectVideos by default (UploadDate) or selected order:
        /// description or uploadDate
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of ProjectVideo</returns>
        IQueryable<ProjectVideo> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ProjectVideo> projectVideos)
        {
            ViewBag.uploadDateSort = string.IsNullOrEmpty(sortOrder) ? "uploadDate_desc" : "";
            ViewBag.descriptionSort = sortOrder == "description" ? "description_desc" : "description";
            ViewBag.descriptionIcon = "bi-caret-down";
            ViewBag.uploadDateIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "description_desc":
                    projectVideos = projectVideos.OrderByDescending(o => o.Description);
                    ViewBag.descriptionIcon = "bi-caret-up-fill";
                    break;
                case "description":
                    projectVideos = projectVideos.OrderBy(o => o.Description);
                    ViewBag.descriptionIcon = "bi-caret-down-fill";
                    break;
                case "uploadDate_desc":
                    projectVideos = projectVideos.OrderByDescending(t => t.UploadDate);
                    ViewBag.uploadDateIcon = "bi-caret-up-fill";
                    break;
                default:
                    projectVideos = projectVideos.OrderBy(t => t.UploadDate);
                    ViewBag.uploadDateIcon = "bi-caret-down-fill";
                    break;
            }
            return projectVideos;
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
            // navProjectVideo
            ViewBag.navProjectVideo = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Project
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(ProjectVideo? projectVideo)
        {

            // set options for Project
            var listProject = new SelectList(context.Project
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", projectVideo?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProject = listProject;
        }


        //======================================================== 


        // makes shure active project matches the project for the ProjectVideo
        async Task<int> GetProjectIdFromProjectVideo(int projectVideoId)
        {
            var projectId = await context.ProjectVideo
                                .Where(r => r.Id == projectVideoId)
                                .Select(r => r.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool ProjectVideoExists(int id)
        {
            return (context.ProjectVideo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
