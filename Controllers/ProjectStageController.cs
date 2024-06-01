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
using JaosLib.Services.Utilities;

namespace IMRepo.Controllers
{
    [Authorize(Roles =ProjectGlobals.registeredRoles)]
    public partial class ProjectStageController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<ProjectStageController> logger;
        private readonly ILogTools logTools;


        public ProjectStageController(IMRepoDbContext context
            , ILogger<ProjectStageController> logger
            , ILogTools logTools
        )
        {
            this.context = context;
            this.logger = logger;
            this.logTools = logTools;
        }

        readonly JaosLibUtils jaosLibUtils = new();

        #region Index


        //----------- Index

        // GET: ProjectStage/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var projectStages = from o in context.ProjectStage select o;
            if (!string.IsNullOrEmpty(searchText))
                projectStages = projectStages.Where(p => p.Name!.Contains(searchText));

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projectStages != null)
            {
                projectStages = OrderBySelectedOrDefault(sortOrder, projectStages);
                return View(await projectStages.ToListAsync());
            }
            return View(new List<ProjectStage>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: ProjectStage/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectStage
            if ((id == null) || (id <= 0) || (context.ProjectStage == null)) return NotFound();
            ProjectStage? projectStage = await context.ProjectStage
                .FindAsync(id);
            if (projectStage == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(projectStage);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: ProjectStage/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: ProjectStage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectStage projectStage, string? returnUrl, string? bufferedUrl)
        {
            // Check if is name unique
            if (context.ProjectStage.Any(c => c.Name == projectStage.Name && c.Id != projectStage.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(projectStage);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "ProjectStage", "Create", projectStage.Id, projectStage);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = projectStage.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Etapa Proyecto";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.OrderValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Order");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            return View(projectStage);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: ProjectStage/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectStage
            if ((id == null) || (id <= 0) || (context.ProjectStage == null)) return NotFound();
            ProjectStage? projectStage = await context.ProjectStage
                .FindAsync(id);
            if (projectStage == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(projectStage);
        }


        // POST: ProjectStage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] ProjectStage projectStage, string? returnUrl, string? bufferedUrl)
        {
            if (id != projectStage.Id)
            {
                return NotFound();
            }
            // Check if is name unique
            if (context.ProjectStage.Any(c => c.Name == projectStage.Name && c.Id != projectStage.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(projectStage);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "ProjectStage", "Edit", projectStage.Id, projectStage);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProjectStageExists(projectStage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Etapa Proyecto. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Etapa Proyecto";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.OrderValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Order");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);

            return View(projectStage);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: ProjectStage/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.ProjectStage == null)) return NotFound();
            ProjectStage? projectStage = await context.ProjectStage
                .FirstAsync(r => r.Id == id);
            if (projectStage == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(projectStage);
            if (projectStage != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.ProjectStage.Remove(projectStage);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "ProjectStage", "Delete", projectStage.Id, projectStage);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Etapa Proyecto";
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



        // Finds all the registers that are using the current registry from Etapa Proyecto
        async Task<bool> findExistingLinks(ProjectStage projectStage)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Proyectos using this Etapa Proyecto
            List<Project>             projects = await context.Project
                .Include(p => p.Sector_info)
                .Include(p => p.Subsector_info)
                .Include(p => p.Office_info)
                .Include(p => p.ExecutingAgency_info)
                .Include(p => p.Stage_info)
                .Include(p => p.Sdg_info)
                .Where(r => r.Stage == projectStage.Id).ToListAsync();
            if (projects?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Proyectos:<br/>";
                foreach (Project project1 in projects)
                    externalLinks += project1?.Name + " - " + project1?.Code + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Etapa Proyecto no puede borrarse<br/>" + externalLinks;
            ViewBag.warningMessage = externalLinks;
            return isLinked;
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
        /// Sorts projectStages by default (Name) or selected order:
        /// name
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of ProjectStage</returns>
        IQueryable<ProjectStage> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ProjectStage> projectStages)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.nameIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "name_desc":
                    projectStages = projectStages.OrderByDescending(t => t.Order);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    projectStages = projectStages.OrderBy(t => t.Order);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return projectStages;
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
            // navProjectStage
            ViewBag.navProjectStage = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool ProjectStageExists(int id)
        {
            return (context.ProjectStage?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
