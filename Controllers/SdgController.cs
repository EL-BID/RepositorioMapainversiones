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
    public partial class SdgController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<SdgController> logger;
        private readonly ILogTools logTools;


        public SdgController(IMRepoDbContext context
            , ILogger<SdgController> logger
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

        // GET: Sdg/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var sdgs = from o in context.Sdg select o;
            if (!string.IsNullOrEmpty(searchText))
                sdgs = sdgs.Where(p =>
                    p.Number.ToString()!.Contains(searchText)
                    || p.Title!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (sdgs != null)
            {
                sdgs = OrderBySelectedOrDefault(sortOrder, sdgs);
                return View(await sdgs.ToListAsync());
            }
            return View(new List<Sdg>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: Sdg/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Sdg
            if ((id == null) || (id <= 0) || (context.Sdg == null)) return NotFound();
            Sdg? sdg = await context.Sdg
                .FindAsync(id);
            if (sdg == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(sdg);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Sdg/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: Sdg/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Sdg sdg, string? returnUrl, string? bufferedUrl)
        {
            // Check if is number unique
            if (context.Sdg.Any(c => c.Number == sdg.Number && c.Id != sdg.Id))
            {
                ModelState.AddModelError("Number", "# existe en otro registro.");
            }
            // Check if is title unique
            if (context.Sdg.Any(c => c.Title == sdg.Title && c.Id != sdg.Id))
            {
                ModelState.AddModelError("Title", "Nombre existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(sdg);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Sdg", "Create", sdg.Id, sdg);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = sdg.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Objetivo de Desarrollo Sostenible";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.NumberValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Number");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            return View(sdg);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Sdg/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Sdg
            if ((id == null) || (id <= 0) || (context.Sdg == null)) return NotFound();
            Sdg? sdg = await context.Sdg
                .FindAsync(id);
            if (sdg == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(sdg);
        }


        // POST: Sdg/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Sdg sdg, string? returnUrl, string? bufferedUrl)
        {
            if (id != sdg.Id)
            {
                return NotFound();
            }
            // Check if is number unique
            if (context.Sdg.Any(c => c.Number == sdg.Number && c.Id != sdg.Id))
            {
                ModelState.AddModelError("Number", "# existe en otro registro.");
            }
            // Check if is title unique
            if (context.Sdg.Any(c => c.Title == sdg.Title && c.Id != sdg.Id))
            {
                ModelState.AddModelError("Title", "Nombre existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(sdg);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Sdg", "Edit", sdg.Id, sdg);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!SdgExists(sdg.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Objetivo de Desarrollo Sostenible. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Objetivo de Desarrollo Sostenible";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.NumberValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Number");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);

            return View(sdg);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Sdg/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Sdg == null)) return NotFound();
            Sdg? sdg = await context.Sdg
                .FirstAsync(r => r.Id == id);
            if (sdg == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(sdg);
            if (sdg != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Sdg.Remove(sdg);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Sdg", "Delete", sdg.Id, sdg);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Objetivo de Desarrollo Sostenible";
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



        // Finds all the registers that are using the current registry from Objetivo de Desarrollo Sostenible
        async Task<bool> findExistingLinks(Sdg sdg)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Proyectos using this Objetivo de Desarrollo Sostenible
            List<Project>             projects = await context.Project
                .Include(p => p.Sector_info)
                .Include(p => p.Subsector_info)
                .Include(p => p.Office_info)
                .Include(p => p.ExecutingAgency_info)
                .Include(p => p.Stage_info)
                .Include(p => p.Sdg_info)
                .Where(r => r.Sdg == sdg.Id).ToListAsync();
            if (projects?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Proyectos:<br/>";
                foreach (Project project1 in projects)
                    externalLinks += project1?.Name + " - " + project1?.Code + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Objetivo de Desarrollo Sostenible no puede borrarse<br/>" + externalLinks;
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
        /// Sorts sdgs by default (Number) or selected order:
        /// number or title
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Sdg</returns>
        IQueryable<Sdg> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Sdg> sdgs)
        {
            ViewBag.numberSort = string.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.titleSort = sortOrder == "title" ? "title_desc" : "title";
            ViewBag.numberIcon = "bi-caret-down";
            ViewBag.titleIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "title_desc":
                    sdgs = sdgs.OrderByDescending(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-up-fill";
                    break;
                case "title":
                    sdgs = sdgs.OrderBy(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-down-fill";
                    break;
                case "number_desc":
                    sdgs = sdgs.OrderByDescending(t => t.Number);
                    ViewBag.numberIcon = "bi-caret-up-fill";
                    break;
                default:
                    sdgs = sdgs.OrderBy(t => t.Number);
                    ViewBag.numberIcon = "bi-caret-down-fill";
                    break;
            }
            return sdgs;
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
            // navSdg
            ViewBag.navSdg = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool SdgExists(int id)
        {
            return (context.Sdg?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
