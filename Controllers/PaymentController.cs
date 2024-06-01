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
    public partial class PaymentController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<PaymentController> logger;
        private readonly ILogTools logTools;


        public PaymentController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<PaymentController> logger
            , ILogTools logTools
        )
        {
            this.context = context;
            this.parentProjectService = parentProjectService;
            this.fileLoadService = fileLoadService;
            this.logger = logger;
            this.logTools = logTools;
        }

        readonly JaosLibUtils jaosLibUtils = new();

        #region Index


        //----------- Index

        // GET: Payment/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, int? productId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var payments = productId.HasValue
                ? from o in context.Payment where (o.Product == productId) select o
                : GetPaymentsForProject(project.Id);
            if (!string.IsNullOrEmpty(searchText))
                payments = payments.Where(p =>
                    p.Code!.Contains(searchText)
                    || p.PaymentAmount.ToString()!.Contains(searchText)
                    || p.PhysicalAdvance.ToString()!.Contains(searchText)
                    || p.DateDelivery.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, productId, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (payments != null)
            {
                payments = OrderBySelectedOrDefault(sortOrder, payments);
                return View(await payments.ToListAsync());
            }
            return View(new List<Payment>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: Payment/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, int? productId, string? returnUrl, string? bufferedUrl)
        {
            // get Payment
            if ((id == null) || (id <= 0) || (context.Payment == null)) return NotFound();
            Payment? payment = await context.Payment
                .FindAsync(id);
            if (payment == null) return NotFound();

            int projectId = await GetProjectIdFromPayment(payment.Id);
            // set ViewBags
            SetStandardViewBags("Display", true, productId, returnUrl, bufferedUrl);
            SetViewBagsForLists(payment, projectId);

            return View(payment);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Payment/Create
        [HttpGet]
        public IActionResult Create(int? productId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, productId, returnUrl, bufferedUrl);
            SetViewBagsForLists(null, project.Id);

            Payment payment = new();
            if (productId.HasValue) payment.Product = productId.Value;
            return View(payment);
        }



        // POST: Payment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Payment payment, int? productId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // Validate Total Payment Greater than Project Cost
            // checks if total payments is greater than total programmed.
            AdvanceBasic? advanceBasic = paymentsGreaterThanProgrammed(payment);
            if (advanceBasic != null && advanceBasic.invalid)
            {
                ModelState.AddModelError("Total",
                string.Format("Suma de pagos ({0}) superior a costo programado del proyecto ({1})."
                                , advanceBasic.actual.HasValue ? advanceBasic.actual.Value.ToString("n2") : ""
                                , advanceBasic.programmed.HasValue ? advanceBasic.programmed.Value.ToString("n2") : ""
                            ));
            }

            // Check if is code unique
            if (context.Payment.Any(c => c.Code == payment.Code && c.Id != payment.Id))
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
                    context.Add(payment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(payment))
                    {
                        context.Update(payment);
                        await context.SaveChangesAsync();
                    }

                    // Assign Code when created
                    payment.Code = (payment.Id + 1000).ToString("d5");
                    context.Update(payment);
                    await context.SaveChangesAsync();

                    // Commit Transaction
                    logTools.Log(User, "Payment", "Create", payment.Id, payment
                        , new List<string> { "AttachmentAdvanceInput","AttachmentPaymentInput" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = payment.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Pago";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.PaymentAmountValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PaymentAmount");
                ViewBag.PhysicalAdvanceValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PhysicalAdvance");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, productId, returnUrl, bufferedUrl);
            SetViewBagsForLists(payment, project.Id);
            EraseFileNameFieldForAttachments(payment);

            return View(payment);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Payment/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? productId, string? returnUrl, string? bufferedUrl)
        {
            // get Payment
            if ((id == null) || (id <= 0) || (context.Payment == null)) return NotFound();
            Payment? payment = await context.Payment
                .FindAsync(id);
            if (payment == null) return NotFound();

            int projectId = await GetProjectIdFromPayment(payment.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, productId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(payment, projectId);

            return View(payment);
        }


        // POST: Payment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Payment payment, int? productId, string? returnUrl, string? bufferedUrl)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            // Validate Total Payment Greater than Project Cost
            // checks if total payments is greater than total programmed.
            AdvanceBasic? advanceBasic = paymentsGreaterThanProgrammed(payment);
            if (advanceBasic != null && advanceBasic.invalid)
            {
                ModelState.AddModelError("Total",
                string.Format("Suma de pagos ({0}) superior a costo programado del proyecto ({1})."
                                , advanceBasic.actual.HasValue ? advanceBasic.actual.Value.ToString("n2") : ""
                                , advanceBasic.programmed.HasValue ? advanceBasic.programmed.Value.ToString("n2") : ""
                            ));
            }

            // Check if is code unique
            if (context.Payment.Any(c => c.Code == payment.Code && c.Id != payment.Id))
            {
                ModelState.AddModelError("Code", "Código existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(payment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(payment))
                    {
                        context.Update(payment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "Payment", "Edit", payment.Id, payment
                        , new List<string> { "AttachmentAdvanceInput","AttachmentPaymentInput" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!PaymentExists(payment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Pago. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Pago";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.PaymentAmountValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PaymentAmount");
                ViewBag.PhysicalAdvanceValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "PhysicalAdvance");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, productId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(payment, project.Id);

            EraseFileNameFieldForAttachments(payment);

            return View(payment);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Payment/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, int? productId, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Payment == null)) return NotFound();
            Payment? payment = await context.Payment
                .Include(t => t.PaymentAttachments!)
                .FirstAsync(r => r.Id == id);
            if (payment == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (payment != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    DeleteChildren(payment);
                    await context.SaveChangesAsync();
                    context.Payment.Remove(payment);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Payment", "Delete", payment.Id, payment
                        , new List<string> { "AttachmentAdvanceInput","AttachmentPaymentInput" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Pago";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }

            //---- if not saved reload view ----
            // set ViewBags
            SetStandardViewBags("Delete", false, productId, returnUrl, bufferedUrl);
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

        public IActionResult Download(string serverFileName,string downloadName)
        {
            if (string.IsNullOrEmpty(serverFileName)) return NoContent();

            string path = fileLoadService.ServerFullPath(FileLoadService.PathPayments);
            string pathWithFile = Path.Combine(path, serverFileName);

            // Return the file as a FileResult
            return File(System.IO.File.OpenRead(pathWithFile), "application/octet-stream", downloadName + Path.GetExtension(serverFileName));
        }

        //----------------------------------------------
        //==============================================
        //----------------------------------------------
        #endregion
        #region Supporting Methods

        /// <summary>
        /// Sorts payments by default (DateDelivery) or selected order:
        /// code, paymentAmount, physicalAdvance or dateDelivery
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Payment</returns>
        IQueryable<Payment> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Payment> payments)
        {
            ViewBag.dateDeliverySort = string.IsNullOrEmpty(sortOrder) ? "dateDelivery_desc" : "";
            ViewBag.codeSort = sortOrder == "code" ? "code_desc" : "code";
            ViewBag.paymentAmountSort = sortOrder == "paymentAmount" ? "paymentAmount_desc" : "paymentAmount";
            ViewBag.physicalAdvanceSort = sortOrder == "physicalAdvance" ? "physicalAdvance_desc" : "physicalAdvance";
            ViewBag.codeIcon = "bi-caret-down";
            ViewBag.paymentAmountIcon = "bi-caret-down";
            ViewBag.physicalAdvanceIcon = "bi-caret-down";
            ViewBag.dateDeliveryIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "code_desc":
                    payments = payments.OrderByDescending(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-up-fill";
                    break;
                case "code":
                    payments = payments.OrderBy(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-down-fill";
                    break;
                case "paymentAmount_desc":
                    payments = payments.OrderByDescending(o => o.PaymentAmount);
                    ViewBag.paymentAmountIcon = "bi-caret-up-fill";
                    break;
                case "paymentAmount":
                    payments = payments.OrderBy(o => o.PaymentAmount);
                    ViewBag.paymentAmountIcon = "bi-caret-down-fill";
                    break;
                case "physicalAdvance_desc":
                    payments = payments.OrderByDescending(o => o.PhysicalAdvance);
                    ViewBag.physicalAdvanceIcon = "bi-caret-up-fill";
                    break;
                case "physicalAdvance":
                    payments = payments.OrderBy(o => o.PhysicalAdvance);
                    ViewBag.physicalAdvanceIcon = "bi-caret-down-fill";
                    break;
                case "dateDelivery_desc":
                    payments = payments.OrderByDescending(t => t.DateDelivery);
                    ViewBag.dateDeliveryIcon = "bi-caret-up-fill";
                    break;
                default:
                    payments = payments.OrderBy(t => t.DateDelivery);
                    ViewBag.dateDeliveryIcon = "bi-caret-down-fill";
                    break;
            }
            return payments;
        }

        //==============================================
        //------------- Controller Methods -------------



        /// <summary>
        /// Assigns the standard ViewBags required for navigation.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="returnsbyDefault">If false. The return button will only be available if returnUrl has a value.
        /// If true, will return to caller if no returnUrl is specified.</param>
        void SetStandardViewBags(string action, bool returnsbyDefault, int? productId, string? returnUrl, string? bufferedUrl)
        {
            if (productId.HasValue) ViewBag.productId = productId;
            // returnUrl
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.returnUrl = returnUrl;
            else if (returnsbyDefault)
                ViewBag.returnUrl = HttpContext.Request.Headers["Referer"];
            // bufferedUrl
            if (!string.IsNullOrEmpty(bufferedUrl)) ViewBag.bufferedUrl = bufferedUrl;
            // navPayment
            ViewBag.navPayment = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Product, FundingSource and Stage
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(Payment? payment, int projectId)
        {

            // set options for Product
            var listProduct = new SelectList(context.Product
                               .Where(a => a.Project == projectId)
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", payment?.Product).ToList();
            listProduct.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProduct = listProduct;

            // set options for FundingSource
            var listFundingSource = new SelectList(context.ProjectFunding
                               .Where(a => a.Project == projectId)
                               .OrderBy(c => c.Value)
                               .Select(r => new SelectListItem((r.Type_info != null ? r.Type_info.Name : "") + " - " + (r.Source_info != null ? r.Source_info.Name : ""), r.Id.ToString())), "Value", "Text", payment?.FundingSource).ToList();
            listFundingSource.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listFundingSource = listFundingSource;

            // set options for Stage
            var listStage = new SelectList(context.PaymentStage
                               .OrderBy(c => c.SortOrder)
                               .Select(r => new SelectListItem(r.Title, r.Id.ToString())), "Value", "Text", payment?.Stage).ToList();
            listStage.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listStage = listStage;
        }


        //======================================================== 

        public IQueryable<Payment> GetPaymentsForProject(int projectId)
        {
            return context.Payment
                       .Include(a => a.Product_info)
                       .Where(a => a.Product_info!.Project == projectId)
            ;
        }

        // makes shure active project matches the project for the Payment
        async Task<int> GetProjectIdFromPayment(int paymentId)
        {
            var projectId = await context.Payment
                                .Where(r => r.Id == paymentId)
                                .Select(r => r.Product_info!.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        // delete children

        /// <summary>
        /// Deletes all children records related to the Payment:
        /// paymentAttachment
        /// </summary>
        /// <param name="project"></param>
        void DeleteChildren(Payment payment)
        {
            if (payment.PaymentAttachments?.Count > 0)
                payment.PaymentAttachments.ToList().ForEach(c => context.PaymentAttachment.Remove(c));
        }
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(Payment payment)
        {
            bool updated = false;

            //---- AttachmentAdvance ----
            if (payment.AttachmentAdvanceInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("AttachmentAdvance" ,payment.Product, payment.Id, payment.AttachmentAdvanceInput);
                int result = await fileLoadService.UploadFile(serverFileName, payment.AttachmentAdvanceInput, FileLoadService.PathPayments);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(payment.AttachmentAdvance, serverFileName);
                // set
                payment.AttachmentAdvance = serverFileName;
                updated = true;
            }
            //---- AttachmentPayment ----
            if (payment.AttachmentPaymentInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("AttachmentPayment" ,payment.Product, payment.Id, payment.AttachmentPaymentInput);
                int result = await fileLoadService.UploadFile(serverFileName, payment.AttachmentPaymentInput, FileLoadService.PathPayments);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(payment.AttachmentPayment, serverFileName);
                // set
                payment.AttachmentPayment = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(Payment payment)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var attachmentAdvanceErrors = ModelState[nameof(payment.AttachmentAdvance)]?.Errors;           
            if (attachmentAdvanceErrors == null || attachmentAdvanceErrors.Count == 0)
            {
                ModelState.Remove(nameof(payment.AttachmentAdvance));
                payment.AttachmentAdvance = string.Empty;
            }
            var attachmentPaymentErrors = ModelState[nameof(payment.AttachmentPayment)]?.Errors;           
            if (attachmentPaymentErrors == null || attachmentPaymentErrors.Count == 0)
            {
                ModelState.Remove(nameof(payment.AttachmentPayment));
                payment.AttachmentPayment = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool PaymentExists(int id)
        {
            return (context.Payment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
