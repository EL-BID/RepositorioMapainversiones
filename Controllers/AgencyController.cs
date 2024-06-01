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
    public partial class AgencyController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<AgencyController> logger;
        private readonly ILogTools logTools;


        public AgencyController(IMRepoDbContext context
            , ILogger<AgencyController> logger
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

        // GET: Agency/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var agencies = from o in context.Agency select o;
            if (!string.IsNullOrEmpty(searchText))
                agencies = agencies.Where(p =>
                    p.Name!.Contains(searchText)
                    || p.Acronym!.Contains(searchText)
                    || p.OfficialID!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (agencies != null)
            {
                agencies = OrderBySelectedOrDefault(sortOrder, agencies);
                return View(await agencies.ToListAsync());
            }
            return View(new List<Agency>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: Agency/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Agency
            if ((id == null) || (id <= 0) || (context.Agency == null)) return NotFound();
            Agency? agency = await context.Agency
                .FindAsync(id);
            if (agency == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(agency);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Agency/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: Agency/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Agency agency, string? returnUrl, string? bufferedUrl)
        {
            // Check if is name unique
            if (context.Agency.Any(c => c.Name == agency.Name && c.Id != agency.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            // Check if is acronym unique
            if (context.Agency.Any(c => c.Acronym == agency.Acronym && c.Id != agency.Id))
            {
                ModelState.AddModelError("Acronym", "Sigla existe en otro registro.");
            }
            // Check if is officialID unique
            if (context.Agency.Any(c => c.OfficialID == agency.OfficialID && c.Id != agency.Id))
            {
                ModelState.AddModelError("OfficialID", "# ID existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(agency);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Agency", "Create", agency.Id, agency);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = agency.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Entidad Ejecutora";
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
            return View(agency);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Agency/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Agency
            if ((id == null) || (id <= 0) || (context.Agency == null)) return NotFound();
            Agency? agency = await context.Agency
                .FindAsync(id);
            if (agency == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(agency);
        }


        // POST: Agency/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Agency agency, string? returnUrl, string? bufferedUrl)
        {
            if (id != agency.Id)
            {
                return NotFound();
            }
            // Check if is name unique
            if (context.Agency.Any(c => c.Name == agency.Name && c.Id != agency.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            // Check if is acronym unique
            if (context.Agency.Any(c => c.Acronym == agency.Acronym && c.Id != agency.Id))
            {
                ModelState.AddModelError("Acronym", "Sigla existe en otro registro.");
            }
            // Check if is officialID unique
            if (context.Agency.Any(c => c.OfficialID == agency.OfficialID && c.Id != agency.Id))
            {
                ModelState.AddModelError("OfficialID", "# ID existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(agency);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Agency", "Edit", agency.Id, agency);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!AgencyExists(agency.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Entidad Ejecutora. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Entidad Ejecutora";
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

            return View(agency);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Agency/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Agency == null)) return NotFound();
            Agency? agency = await context.Agency
                .FirstAsync(r => r.Id == id);
            if (agency == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(agency);
            if (agency != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Agency.Remove(agency);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Agency", "Delete", agency.Id, agency);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Entidad Ejecutora";
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



        // Finds all the registers that are using the current registry from Entidad Ejecutora
        async Task<bool> findExistingLinks(Agency agency)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Proyectos using this Entidad Ejecutora
            List<Project>             projects = await context.Project
                .Include(p => p.Sector_info)
                .Include(p => p.Subsector_info)
                .Include(p => p.Office_info)
                .Include(p => p.ExecutingAgency_info)
                .Include(p => p.Stage_info)
                .Include(p => p.Sdg_info)
                .Where(r => r.ExecutingAgency == agency.Id).ToListAsync();
            if (projects?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Proyectos:<br/>";
                foreach (Project project1 in projects)
                    externalLinks += project1?.Name + " - " + project1?.Code + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Entidad Ejecutora no puede borrarse<br/>" + externalLinks;
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
        /// Sorts agencies by default (Name) or selected order:
        /// name, acronym or officialID
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Agency</returns>
        IQueryable<Agency> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Agency> agencies)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.acronymSort = sortOrder == "acronym" ? "acronym_desc" : "acronym";
            ViewBag.officialIDSort = sortOrder == "officialID" ? "officialID_desc" : "officialID";
            ViewBag.nameIcon = "bi-caret-down";
            ViewBag.acronymIcon = "bi-caret-down";
            ViewBag.officialIDIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "acronym_desc":
                    agencies = agencies.OrderByDescending(o => o.Acronym);
                    ViewBag.acronymIcon = "bi-caret-up-fill";
                    break;
                case "acronym":
                    agencies = agencies.OrderBy(o => o.Acronym);
                    ViewBag.acronymIcon = "bi-caret-down-fill";
                    break;
                case "officialID_desc":
                    agencies = agencies.OrderByDescending(o => o.OfficialID);
                    ViewBag.officialIDIcon = "bi-caret-up-fill";
                    break;
                case "officialID":
                    agencies = agencies.OrderBy(o => o.OfficialID);
                    ViewBag.officialIDIcon = "bi-caret-down-fill";
                    break;
                case "name_desc":
                    agencies = agencies.OrderByDescending(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    agencies = agencies.OrderBy(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return agencies;
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
            // navAgency
            ViewBag.navAgency = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool AgencyExists(int id)
        {
            return (context.Agency?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
