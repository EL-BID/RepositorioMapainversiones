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
    public partial class FundingAgencyController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<FundingAgencyController> logger;
        private readonly ILogTools logTools;


        public FundingAgencyController(IMRepoDbContext context
            , ILogger<FundingAgencyController> logger
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

        // GET: FundingAgency/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var fundingAgencies = from o in context.FundingAgency select o;
            if (!string.IsNullOrEmpty(searchText))
                fundingAgencies = fundingAgencies.Where(p => p.Name!.Contains(searchText));

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (fundingAgencies != null)
            {
                fundingAgencies = OrderBySelectedOrDefault(sortOrder, fundingAgencies);
                return View(await fundingAgencies.ToListAsync());
            }
            return View(new List<FundingAgency>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: FundingAgency/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get FundingAgency
            if ((id == null) || (id <= 0) || (context.FundingAgency == null)) return NotFound();
            FundingAgency? fundingAgency = await context.FundingAgency
                .FindAsync(id);
            if (fundingAgency == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(fundingAgency);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: FundingAgency/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: FundingAgency/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] FundingAgency fundingAgency, string? returnUrl, string? bufferedUrl)
        {
            // Check if is name unique
            if (context.FundingAgency.Any(c => c.Name == fundingAgency.Name && c.Id != fundingAgency.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            // Check if is acronym unique
            if (context.FundingAgency.Any(c => c.Acronym == fundingAgency.Acronym && c.Id != fundingAgency.Id))
            {
                ModelState.AddModelError("Acronym", "Sigla existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(fundingAgency);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "FundingAgency", "Create", fundingAgency.Id, fundingAgency);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = fundingAgency.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Entidad Financiadora";
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
            return View(fundingAgency);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: FundingAgency/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get FundingAgency
            if ((id == null) || (id <= 0) || (context.FundingAgency == null)) return NotFound();
            FundingAgency? fundingAgency = await context.FundingAgency
                .FindAsync(id);
            if (fundingAgency == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(fundingAgency);
        }


        // POST: FundingAgency/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] FundingAgency fundingAgency, string? returnUrl, string? bufferedUrl)
        {
            if (id != fundingAgency.Id)
            {
                return NotFound();
            }
            // Check if is name unique
            if (context.FundingAgency.Any(c => c.Name == fundingAgency.Name && c.Id != fundingAgency.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            // Check if is acronym unique
            if (context.FundingAgency.Any(c => c.Acronym == fundingAgency.Acronym && c.Id != fundingAgency.Id))
            {
                ModelState.AddModelError("Acronym", "Sigla existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(fundingAgency);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "FundingAgency", "Edit", fundingAgency.Id, fundingAgency);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!FundingAgencyExists(fundingAgency.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Entidad Financiadora. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Entidad Financiadora";
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

            return View(fundingAgency);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: FundingAgency/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.FundingAgency == null)) return NotFound();
            FundingAgency? fundingAgency = await context.FundingAgency
                .FirstAsync(r => r.Id == id);
            if (fundingAgency == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(fundingAgency);
            if (fundingAgency != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.FundingAgency.Remove(fundingAgency);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "FundingAgency", "Delete", fundingAgency.Id, fundingAgency);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Entidad Financiadora";
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



        // Finds all the registers that are using the current registry from Entidad Financiadora
        async Task<bool> findExistingLinks(FundingAgency fundingAgency)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Fuentes de Financiamiento using this Entidad Financiadora
            List<ProjectFunding>             projectFundings = await context.ProjectFunding
                .Include(p => p.Project_info)
                .Include(p => p.Type_info)
                .Include(p => p.Source_info)
                .Where(r => r.Source == fundingAgency.Id).ToListAsync();
            if (projectFundings?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Fuentes de Financiamiento:<br/>";
                foreach (ProjectFunding projectFunding1 in projectFundings)
                    externalLinks += projectFunding1?.Type_info?.Name + " - " + projectFunding1?.Source_info?.Name + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Entidad Financiadora no puede borrarse<br/>" + externalLinks;
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
        /// Sorts fundingAgencies by default (Name) or selected order:
        /// name
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of FundingAgency</returns>
        IQueryable<FundingAgency> OrderBySelectedOrDefault(string? sortOrder, IQueryable<FundingAgency> fundingAgencies)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.nameIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "name_desc":
                    fundingAgencies = fundingAgencies.OrderByDescending(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    fundingAgencies = fundingAgencies.OrderBy(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return fundingAgencies;
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
            // navFundingAgency
            ViewBag.navFundingAgency = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool FundingAgencyExists(int id)
        {
            return (context.FundingAgency?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
