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
    public partial class PaymentStageController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<PaymentStageController> logger;
        private readonly ILogTools logTools;


        public PaymentStageController(IMRepoDbContext context
            , ILogger<PaymentStageController> logger
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

        // GET: PaymentStage/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var paymentStages = from o in context.PaymentStage select o;
            if (!string.IsNullOrEmpty(searchText))
                paymentStages = paymentStages.Where(p => p.Title!.Contains(searchText));

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (paymentStages != null)
            {
                paymentStages = OrderBySelectedOrDefault(sortOrder, paymentStages);
                return View(await paymentStages.ToListAsync());
            }
            return View(new List<PaymentStage>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: PaymentStage/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get PaymentStage
            if ((id == null) || (id <= 0) || (context.PaymentStage == null)) return NotFound();
            PaymentStage? paymentStage = await context.PaymentStage
                .FindAsync(id);
            if (paymentStage == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(paymentStage);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: PaymentStage/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: PaymentStage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PaymentStage paymentStage, string? returnUrl, string? bufferedUrl)
        {
            // Check if is title unique
            if (context.PaymentStage.Any(c => c.Title == paymentStage.Title && c.Id != paymentStage.Id))
            {
                ModelState.AddModelError("Title", "Título existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(paymentStage);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "PaymentStage", "Create", paymentStage.Id, paymentStage);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = paymentStage.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Etapa de Pago";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.SortOrderValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "SortOrder");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            return View(paymentStage);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: PaymentStage/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get PaymentStage
            if ((id == null) || (id <= 0) || (context.PaymentStage == null)) return NotFound();
            PaymentStage? paymentStage = await context.PaymentStage
                .FindAsync(id);
            if (paymentStage == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(paymentStage);
        }


        // POST: PaymentStage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] PaymentStage paymentStage, string? returnUrl, string? bufferedUrl)
        {
            if (id != paymentStage.Id)
            {
                return NotFound();
            }
            // Check if is title unique
            if (context.PaymentStage.Any(c => c.Title == paymentStage.Title && c.Id != paymentStage.Id))
            {
                ModelState.AddModelError("Title", "Título existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(paymentStage);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "PaymentStage", "Edit", paymentStage.Id, paymentStage);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!PaymentStageExists(paymentStage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Etapa de Pago. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Etapa de Pago";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.SortOrderValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "SortOrder");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);

            return View(paymentStage);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: PaymentStage/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.PaymentStage == null)) return NotFound();
            PaymentStage? paymentStage = await context.PaymentStage
                .FirstAsync(r => r.Id == id);
            if (paymentStage == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(paymentStage);
            if (paymentStage != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.PaymentStage.Remove(paymentStage);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "PaymentStage", "Delete", paymentStage.Id, paymentStage);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Etapa de Pago";
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



        // Finds all the registers that are using the current registry from Etapa de Pago
        async Task<bool> findExistingLinks(PaymentStage paymentStage)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Pagos using this Etapa de Pago
            List<Payment>             payments = await context.Payment
                .Include(p => p.Product_info)
                .Include(p => p.FundingSource_info)
                .Include(p => p.Stage_info)
                .Where(r => r.Stage == paymentStage.Id).ToListAsync();
            if (payments?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Pagos:<br/>";
                foreach (Payment payment1 in payments)
                    externalLinks += payment1?.Code + " - " + ((payment1?.DateDelivery.HasValue == true) ? payment1?.DateDelivery.Value.ToString("yyyy-MMM-dd") : "") + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Etapa de Pago no puede borrarse<br/>" + externalLinks;
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
        /// Sorts paymentStages by default (Title) or selected order:
        /// title
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of PaymentStage</returns>
        IQueryable<PaymentStage> OrderBySelectedOrDefault(string? sortOrder, IQueryable<PaymentStage> paymentStages)
        {
            ViewBag.titleSort = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.titleIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "title_desc":
                    paymentStages = paymentStages.OrderByDescending(t => t.SortOrder);
                    ViewBag.titleIcon = "bi-caret-up-fill";
                    break;
                default:
                    paymentStages = paymentStages.OrderBy(t => t.SortOrder);
                    ViewBag.titleIcon = "bi-caret-down-fill";
                    break;
            }
            return paymentStages;
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
            // navPaymentStage
            ViewBag.navPaymentStage = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool PaymentStageExists(int id)
        {
            return (context.PaymentStage?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
