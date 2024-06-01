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
    public partial class ProjectFundingController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly ILogger<ProjectFundingController> logger;
        private readonly ILogTools logTools;


        public ProjectFundingController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , ILogger<ProjectFundingController> logger
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

        // GET: ProjectFunding/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var projectFundings = from o in context.ProjectFunding where (o.Project == project.Id) select o;
            if (!string.IsNullOrEmpty(searchText))
                projectFundings = projectFundings.Where(p =>
                    p.Source_info!.Name!.Contains(searchText)
                    || p.Value.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projectFundings != null)
            {
                projectFundings = OrderBySelectedOrDefault(sortOrder, projectFundings);
                projectFundings = projectFundings
                    .Include(p => p.Source_info);
                return View(await projectFundings.ToListAsync());
            }
            return View(new List<ProjectFunding>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: ProjectFunding/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectFunding
            if ((id == null) || (id <= 0) || (context.ProjectFunding == null)) return NotFound();
            ProjectFunding? projectFunding = await context.ProjectFunding
                .FindAsync(id);
            if (projectFunding == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectFunding);

            return View(projectFunding);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: ProjectFunding/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            ProjectFunding projectFunding = new()
            {
                Project = project.Id
            };
            return View(projectFunding);
        }



        // POST: ProjectFunding/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProjectFunding projectFunding, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            unselectedLinksNulled(projectFunding);

            // Create: validate totalCost > Project.PlannedCost
            var totalCost = projectTotalFunding(projectFunding);
            if (!project.PlannedCost.HasValue || totalCost > project.PlannedCost)
            {
                if (!project.PlannedCost.HasValue)
                    ViewBag.errorMessage = string.Format("El proyecto no tiene costo planeado inicial.");
                else
                    ViewBag.errorMessage = string.Format("Financiamiento Total {0} es Superior al valor total del proyecto {1}", totalCost.ToString("n2"), project.PlannedCost.Value.ToString("n2"));
            }
            else

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(projectFunding);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "ProjectFunding", "Create", projectFunding.Id, projectFunding);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = projectFunding.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Fuente de Financiamiento";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.ValueValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Value");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectFunding);
            return View(projectFunding);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: ProjectFunding/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get ProjectFunding
            if ((id == null) || (id <= 0) || (context.ProjectFunding == null)) return NotFound();
            ProjectFunding? projectFunding = await context.ProjectFunding
                .FindAsync(id);
            if (projectFunding == null) return NotFound();

            int projectId = await GetProjectIdFromProjectFunding(projectFunding.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectFunding);

            return View(projectFunding);
        }


        // POST: ProjectFunding/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] ProjectFunding projectFunding, string? returnUrl, string? bufferedUrl)
        {
            if (id != projectFunding.Id)
            {
                return NotFound();
            }
            unselectedLinksNulled(projectFunding);

            // Edit: validate totalCost > Project.PlannedCost
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if (!(project?.Id > 0)) return RedirectToAction("Select", "Project");
            var totalCost = projectTotalFunding(projectFunding);
            if (!project.PlannedCost.HasValue || totalCost > project.PlannedCost)
            {
                if (!project.PlannedCost.HasValue)
                    ViewBag.errorMessage = string.Format("El proyecto no tiene costo planeado inicial.");
                else
                    ViewBag.errorMessage = string.Format("Financiamiento Total {0} es Superior al valor total del proyecto {1}", totalCost.ToString("n2"), project.PlannedCost.Value.ToString("n2"));
            }
            else

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(projectFunding);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "ProjectFunding", "Edit", projectFunding.Id, projectFunding);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProjectFundingExists(projectFunding.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Fuente de Financiamiento. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Fuente de Financiamiento";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.ValueValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Value");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);
            SetViewBagsForLists(projectFunding);

            return View(projectFunding);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: ProjectFunding/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.ProjectFunding == null)) return NotFound();
            ProjectFunding? projectFunding = await context.ProjectFunding
                .FirstAsync(r => r.Id == id);
            if (projectFunding == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(projectFunding);
            if (projectFunding != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.ProjectFunding.Remove(projectFunding);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "ProjectFunding", "Delete", projectFunding.Id, projectFunding);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Fuente de Financiamiento";
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



        // Finds all the registers that are using the current registry from Fuente de Financiamiento
        async Task<bool> findExistingLinks(ProjectFunding projectFunding)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Pagos using this Fuente de Financiamiento
            List<Payment>             payments = await context.Payment
                .Include(p => p.Product_info)
                .Include(p => p.FundingSource_info)
                .Include(p => p.Stage_info)
                .Where(r => r.FundingSource == projectFunding.Id).ToListAsync();
            if (payments?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Pagos:<br/>";
                foreach (Payment payment1 in payments)
                    externalLinks += payment1?.Code + " - " + ((payment1?.DateDelivery.HasValue == true) ? payment1?.DateDelivery.Value.ToString("yyyy-MMM-dd") : "") + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Fuente de Financiamiento no puede borrarse<br/>" + externalLinks;
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
        /// Sorts projectFundings by default (Value) or selected order:
        /// source or value
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of ProjectFunding</returns>
        IQueryable<ProjectFunding> OrderBySelectedOrDefault(string? sortOrder, IQueryable<ProjectFunding> projectFundings)
        {
            ViewBag.valueSort = string.IsNullOrEmpty(sortOrder) ? "value_desc" : "";
            ViewBag.sourceSort = sortOrder == "source" ? "source_desc" : "source";
            ViewBag.sourceIcon = "bi-caret-down";
            ViewBag.valueIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "source_desc":
                    projectFundings = projectFundings.OrderByDescending(o => o.Source);
                    ViewBag.sourceIcon = "bi-caret-up-fill";
                    break;
                case "source":
                    projectFundings = projectFundings.OrderBy(o => o.Source);
                    ViewBag.sourceIcon = "bi-caret-down-fill";
                    break;
                case "value_desc":
                    projectFundings = projectFundings.OrderByDescending(t => t.Value);
                    ViewBag.valueIcon = "bi-caret-up-fill";
                    break;
                default:
                    projectFundings = projectFundings.OrderBy(t => t.Value);
                    ViewBag.valueIcon = "bi-caret-down-fill";
                    break;
            }
            return projectFundings;
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
            // navProjectFunding
            ViewBag.navProjectFunding = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Project, Type and Source
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(ProjectFunding? projectFunding)
        {

            // set options for Project
            var listProject = new SelectList(context.Project
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", projectFunding?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProject = listProject;

            // set options for Type
            var listType = new SelectList(context.FundingType
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", projectFunding?.Type).ToList();
            listType.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listType = listType;

            // set options for Source
            var listSource = new SelectList(context.FundingAgency
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", projectFunding?.Source).ToList();
            listSource.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listSource = listSource;
        }


        
        /// <summary>
        /// For the selects that allow null value, converts any selected value 0 to null.
        /// </summary>
        /// <param name="projectFunding">The ProjectFunding where selects will be validated.</param>
        void unselectedLinksNulled(ProjectFunding projectFunding)
        {
            // sets unselected lists to null
            if (projectFunding.Type == 0) projectFunding.Type = null;

        }

        //======================================================== 


        // makes shure active project matches the project for the ProjectFunding
        async Task<int> GetProjectIdFromProjectFunding(int projectFundingId)
        {
            var projectId = await context.ProjectFunding
                                .Where(r => r.Id == projectFundingId)
                                .Select(r => r.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool ProjectFundingExists(int id)
        {
            return (context.ProjectFunding?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
