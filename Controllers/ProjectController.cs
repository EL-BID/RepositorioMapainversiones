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
    public partial class ProjectController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly ILogger<ProjectController> logger;
        private readonly ILogTools logTools;


        public ProjectController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , ILogger<ProjectController> logger
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

        // GET: Project/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var projects = from o in context.Project select o;
            if (!string.IsNullOrEmpty(searchText))
                projects = projects.Where(p =>
                    p.Name!.Contains(searchText)
                    || p.Code!.Contains(searchText)
                    || p.Sector_info!.Name!.Contains(searchText)
                    || p.Office_info!.Name!.Contains(searchText)
                    || p.Stage_info!.Name!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projects != null)
            {
                projects = OrderBySelectedOrDefault(sortOrder, projects);
                projects = projects
                    .Include(p => p.Sector_info)
                    .Include(p => p.Office_info)
                    .Include(p => p.Stage_info);
                return View(await projects.ToListAsync());
            }
            return View(new List<Project>());
        }


        #endregion
        #region Select


        //----------- Select

        // GET: Project/Select
        [HttpGet]
        public async Task<IActionResult> Select(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var projects = from o in context.Project select o;
            if (!string.IsNullOrEmpty(searchText))
                projects = projects.Where(p =>
                    p.Name!.Contains(searchText)
                    || p.Code!.Contains(searchText)
                    || p.Sector_info!.Name!.Contains(searchText)
                    || p.Office_info!.Name!.Contains(searchText)
                    || p.Stage_info!.Name!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Select", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (projects != null)
            {
                projects = OrderBySelectedOrDefault(sortOrder, projects);
                projects = projects
                    .Include(p => p.Sector_info)
                    .Include(p => p.Office_info)
                    .Include(p => p.Stage_info);
                return View(await projects.ToListAsync());
            }
            else
                return View(null);
        }
        #endregion
        #region Display


        //----------- Display

        // GET: Project/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            Project? project = await parentProjectService.getProjectFromIdOrSession(id, User, HttpContext.Session, ViewBag);
            if (project == null) return await Task.Run(() => RedirectToAction("Select", "Project"));
            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(project);

            return View(project);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Project/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            return View();
        }



        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Project project, string? returnUrl, string? bufferedUrl)
        {
            unselectedLinksNulled(project);
            // Check if is name unique
            if (context.Project.Any(c => c.Name == project.Name && c.Id != project.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            // Check if is code unique
            if (context.Project.Any(c => c.Code == project.Code && c.Id != project.Id))
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
                    context.Add(project);
                    await context.SaveChangesAsync();

                    // Assign Code when created
                    project.Code = (project.Id + 1000).ToString("d5");
                    context.Update(project);
                    await context.SaveChangesAsync();

                    // Commit Transaction
                    logTools.Log(User, "Project", "Create", project.Id, project
                        , new List<string> { "Fuenteses","Attachmentses" });
                    transaction.Commit();
                    parentProjectService.setSessionProject(project, HttpContext.Session);
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = project.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Proyecto";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.PlannedCostValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PlannedCost");
                ViewBag.PlannedDurationValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PlannedDuration");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(project);
            return View(project);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Project/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            Project? project = await parentProjectService.getProjectFromIdOrSession(id, User, HttpContext.Session, ViewBag);
            if (project == null) return await Task.Run(() => RedirectToAction("Select", "Project"));

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(project);
            ViewBag.projectId = project.Id;

            return View(project);
        }


        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Project project, string? returnUrl, string? bufferedUrl)
        {
            unselectedLinksNulled(project);
            // Check if is name unique
            if (context.Project.Any(c => c.Name == project.Name && c.Id != project.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            // Check if is code unique
            if (context.Project.Any(c => c.Code == project.Code && c.Id != project.Id))
            {
                ModelState.AddModelError("Code", "Código existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(project);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Project", "Edit", project.Id, project
                        , new List<string> { "Fuenteses","Attachmentses" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Proyecto. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Proyecto";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.PlannedCostValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PlannedCost");
                ViewBag.PlannedDurationValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PlannedDuration");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);
            SetViewBagsForLists(project);
            ViewBag.projectId = project.Id;

            return View(project);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Project/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Project == null)) return NotFound();
            Project? project = await context.Project
                .Include(t => t.ProjectAttachments!)
                .Include(t => t.Extensions!)
                .ThenInclude(f => f.ExtensionAttachments!)
                .Include(t => t.ProjectFundings!)
                .Include(t => t.ProjectImages!)
                .Include(t => t.Products!)
                .ThenInclude(f => f.Additions!)
                .ThenInclude(f => f.AdditionAttachments!)
                .Include(t => t.Products!)
                .ThenInclude(f => f.Payments!)
                .ThenInclude(f => f.PaymentAttachments!)
                .Include(t => t.ProjectVideos!)
                .FirstAsync(r => r.Id == id);
            if (project == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (project != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    DeleteChildren(project);
                    await context.SaveChangesAsync();
                    context.Project.Remove(project);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Project", "Delete", project.Id, project
                        , new List<string> { "Fuenteses","Attachmentses" });
                    transaction.Commit();

                    parentProjectService.setSessionProject(null, HttpContext.Session);
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Proyecto";
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
        /// Sorts projects by default (Name) or selected order:
        /// name, code, sector, office or stage
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Project</returns>
        IQueryable<Project> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Project> projects)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.codeSort = sortOrder == "code" ? "code_desc" : "code";
            ViewBag.sectorSort = sortOrder == "sector" ? "sector_desc" : "sector";
            ViewBag.officeSort = sortOrder == "office" ? "office_desc" : "office";
            ViewBag.stageSort = sortOrder == "stage" ? "stage_desc" : "stage";
            ViewBag.nameIcon = "bi-caret-down";
            ViewBag.codeIcon = "bi-caret-down";
            ViewBag.sectorIcon = "bi-caret-down";
            ViewBag.officeIcon = "bi-caret-down";
            ViewBag.stageIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "code_desc":
                    projects = projects.OrderByDescending(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-up-fill";
                    break;
                case "code":
                    projects = projects.OrderBy(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-down-fill";
                    break;
                case "sector_desc":
                    projects = projects.OrderByDescending(o => o.Sector);
                    ViewBag.sectorIcon = "bi-caret-up-fill";
                    break;
                case "sector":
                    projects = projects.OrderBy(o => o.Sector);
                    ViewBag.sectorIcon = "bi-caret-down-fill";
                    break;
                case "office_desc":
                    projects = projects.OrderByDescending(o => o.Office);
                    ViewBag.officeIcon = "bi-caret-up-fill";
                    break;
                case "office":
                    projects = projects.OrderBy(o => o.Office);
                    ViewBag.officeIcon = "bi-caret-down-fill";
                    break;
                case "stage_desc":
                    projects = projects.OrderByDescending(o => o.Stage);
                    ViewBag.stageIcon = "bi-caret-up-fill";
                    break;
                case "stage":
                    projects = projects.OrderBy(o => o.Stage);
                    ViewBag.stageIcon = "bi-caret-down-fill";
                    break;
                case "name_desc":
                    projects = projects.OrderByDescending(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    projects = projects.OrderBy(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return projects;
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
            // navProject
            ViewBag.navProject = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Sector, Subsector, Office, ExecutingAgency, Stage and Sdg
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(Project? project)
        {

            // set options for Sector
            var listSector = new SelectList(context.Sector
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", project?.Sector).ToList();
            listSector.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listSector = listSector;

            // set options for Subsector
            var listSubsector = new SelectList(context.Subsector
                               .OrderBy(c => c.Sector).ThenBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", project?.Subsector).ToList();
            listSubsector.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listSubsector = listSubsector;
            ViewBag.listSubsectorParent = context.Subsector.Select(r => new { parentId = r.Sector.ToString(), id = r.Id.ToString() }).ToList().ToJson();

            // set options for Office
            var listOffice = new SelectList(context.Office
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", project?.Office).ToList();
            listOffice.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listOffice = listOffice;

            // set options for ExecutingAgency
            var listExecutingAgency = new SelectList(context.Agency
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", project?.ExecutingAgency).ToList();
            listExecutingAgency.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listExecutingAgency = listExecutingAgency;

            // set options for Stage
            var listStage = new SelectList(context.ProjectStage
                               .OrderBy(c => c.Order)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", project?.Stage).ToList();
            listStage.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listStage = listStage;

            // set options for Sdg
            var listSdg = new SelectList(context.Sdg
                               .OrderBy(c => c.Number)
                               .Select(r => new SelectListItem(((r.Number.HasValue == true) ? r.Number.Value.ToString("n0") : "") + " - " + r.Title, r.Id.ToString())), "Value", "Text", project?.Sdg).ToList();
            listSdg.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listSdg = listSdg;
        }


        
        /// <summary>
        /// For the selects that allow null value, converts any selected value 0 to null.
        /// </summary>
        /// <param name="project">The Project where selects will be validated.</param>
        void unselectedLinksNulled(Project project)
        {
            // sets unselected lists to null
            if (project.Sector == 0) project.Sector = null;
            if (project.Subsector == 0) project.Subsector = null;
            if (project.Office == 0) project.Office = null;
            if (project.ExecutingAgency == 0) project.ExecutingAgency = null;
            if (project.Sdg == 0) project.Sdg = null;

        }

        //----------------------------------------------
        //==============================================
        // delete children

        /// <summary>
        /// Deletes all children records related to the Project:
        /// projectAttachment, extension, projectFunding, projectImage, product and projectVideo
        /// </summary>
        /// <param name="project"></param>
        void DeleteChildren(Project project)
        {
            if (project.ProjectAttachments?.Count > 0)
                project.ProjectAttachments.ToList().ForEach(c => context.ProjectAttachment.Remove(c));
            if (project.Extensions?.Count > 0)
                foreach (var extension in project.Extensions)
                {
                    if (extension.ExtensionAttachments?.Count > 0)
                        extension.ExtensionAttachments.ToList().ForEach(c => context.ExtensionAttachment.Remove(c));
                    context.Extension.Remove(extension);
                }
            if (project.ProjectFundings?.Count > 0)
                project.ProjectFundings.ToList().ForEach(c => context.ProjectFunding.Remove(c));
            if (project.ProjectImages?.Count > 0)
                project.ProjectImages.ToList().ForEach(c => context.ProjectImage.Remove(c));
            if (project.Products?.Count > 0)
                foreach (var product in project.Products)
                {
                    if (product.Additions?.Count > 0)
                        foreach (var addition in product.Additions)
                        {
                            if (addition.AdditionAttachments?.Count > 0)
                                addition.AdditionAttachments.ToList().ForEach(c => context.AdditionAttachment.Remove(c));
                            context.Addition.Remove(addition);
                        }
                    if (product.Payments?.Count > 0)
                        foreach (var payment in product.Payments)
                        {
                            if (payment.PaymentAttachments?.Count > 0)
                                payment.PaymentAttachments.ToList().ForEach(c => context.PaymentAttachment.Remove(c));
                            context.Payment.Remove(payment);
                        }
                    context.Product.Remove(product);
                }
            if (project.ProjectVideos?.Count > 0)
                project.ProjectVideos.ToList().ForEach(c => context.ProjectVideo.Remove(c));
        }
        #endregion


        //========================================================

        private bool ProjectExists(int id)
        {
            return (context.Project?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
