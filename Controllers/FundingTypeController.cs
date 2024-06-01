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
    public partial class FundingTypeController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<FundingTypeController> logger;
        private readonly ILogTools logTools;


        public FundingTypeController(IMRepoDbContext context
            , ILogger<FundingTypeController> logger
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

        // GET: FundingType/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var fundingTypes = from o in context.FundingType select o;
            if (!string.IsNullOrEmpty(searchText))
                fundingTypes = fundingTypes.Where(p => p.Name!.Contains(searchText));

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (fundingTypes != null)
            {
                fundingTypes = OrderBySelectedOrDefault(sortOrder, fundingTypes);
                return View(await fundingTypes.ToListAsync());
            }
            return View(new List<FundingType>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: FundingType/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get FundingType
            if ((id == null) || (id <= 0) || (context.FundingType == null)) return NotFound();
            FundingType? fundingType = await context.FundingType
                .FindAsync(id);
            if (fundingType == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(fundingType);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: FundingType/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: FundingType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] FundingType fundingType, string? returnUrl, string? bufferedUrl)
        {
            // Check if is name unique
            if (context.FundingType.Any(c => c.Name == fundingType.Name && c.Id != fundingType.Id))
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
                    context.Add(fundingType);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "FundingType", "Create", fundingType.Id, fundingType);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = fundingType.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Tipo de Financiamiento";
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
            return View(fundingType);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: FundingType/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get FundingType
            if ((id == null) || (id <= 0) || (context.FundingType == null)) return NotFound();
            FundingType? fundingType = await context.FundingType
                .FindAsync(id);
            if (fundingType == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(fundingType);
        }


        // POST: FundingType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] FundingType fundingType, string? returnUrl, string? bufferedUrl)
        {
            if (id != fundingType.Id)
            {
                return NotFound();
            }
            // Check if is name unique
            if (context.FundingType.Any(c => c.Name == fundingType.Name && c.Id != fundingType.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(fundingType);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "FundingType", "Edit", fundingType.Id, fundingType);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!FundingTypeExists(fundingType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Tipo de Financiamiento. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Tipo de Financiamiento";
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

            return View(fundingType);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: FundingType/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.FundingType == null)) return NotFound();
            FundingType? fundingType = await context.FundingType
                .FirstAsync(r => r.Id == id);
            if (fundingType == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(fundingType);
            if (fundingType != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.FundingType.Remove(fundingType);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "FundingType", "Delete", fundingType.Id, fundingType);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Tipo de Financiamiento";
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



        // Finds all the registers that are using the current registry from Tipo de Financiamiento
        async Task<bool> findExistingLinks(FundingType fundingType)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Fuentes de Financiamiento using this Tipo de Financiamiento
            List<ProjectFunding>             projectFundings = await context.ProjectFunding
                .Include(p => p.Project_info)
                .Include(p => p.Type_info)
                .Include(p => p.Source_info)
                .Where(r => r.Type == fundingType.Id).ToListAsync();
            if (projectFundings?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Fuentes de Financiamiento:<br/>";
                foreach (ProjectFunding projectFunding1 in projectFundings)
                    externalLinks += projectFunding1?.Type_info?.Name + " - " + projectFunding1?.Source_info?.Name + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Tipo de Financiamiento no puede borrarse<br/>" + externalLinks;
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
        /// Sorts fundingTypes by default (Name) or selected order:
        /// name
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of FundingType</returns>
        IQueryable<FundingType> OrderBySelectedOrDefault(string? sortOrder, IQueryable<FundingType> fundingTypes)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.nameIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "name_desc":
                    fundingTypes = fundingTypes.OrderByDescending(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    fundingTypes = fundingTypes.OrderBy(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return fundingTypes;
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
            // navFundingType
            ViewBag.navFundingType = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool FundingTypeExists(int id)
        {
            return (context.FundingType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
