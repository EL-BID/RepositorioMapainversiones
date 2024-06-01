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
    public partial class PaymentAttachmentController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<PaymentAttachmentController> logger;
        private readonly ILogTools logTools;


        public PaymentAttachmentController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<PaymentAttachmentController> logger
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

        // GET: PaymentAttachment/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var paymentAttachments = paymentId.HasValue
                ? from o in context.PaymentAttachment where (o.Payment == paymentId) select o
                : GetPaymentAttachmentsForProject(project.Id);
            if (!string.IsNullOrEmpty(searchText))
                paymentAttachments = paymentAttachments.Where(p =>
                    p.Title!.Contains(searchText)
                    || p.DateAttached.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, paymentId, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (paymentAttachments != null)
            {
                paymentAttachments = OrderBySelectedOrDefault(sortOrder, paymentAttachments);
                return View(await paymentAttachments.ToListAsync());
            }
            return View(new List<PaymentAttachment>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: PaymentAttachment/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            // get PaymentAttachment
            if ((id == null) || (id <= 0) || (context.PaymentAttachment == null)) return NotFound();
            PaymentAttachment? paymentAttachment = await context.PaymentAttachment
                .FindAsync(id);
            if (paymentAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromPaymentAttachment(paymentAttachment.Id);
            // set ViewBags
            SetStandardViewBags("Display", true, paymentId, returnUrl, bufferedUrl);
            SetViewBagsForLists(paymentAttachment, projectId);

            return View(paymentAttachment);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: PaymentAttachment/Create
        [HttpGet]
        public IActionResult Create(int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, paymentId, returnUrl, bufferedUrl);
            SetViewBagsForLists(null, project.Id);

            PaymentAttachment paymentAttachment = new();
            if (paymentId.HasValue) paymentAttachment.Payment = paymentId.Value;
            return View(paymentAttachment);
        }



        // POST: PaymentAttachment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PaymentAttachment paymentAttachment, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");

            // set UploadDate
            paymentAttachment.DateAttached = DateTime.Now;

            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(paymentAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(paymentAttachment))
                    {
                        context.Update(paymentAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "PaymentAttachment", "Create", paymentAttachment.Id, paymentAttachment
                        , new List<string> { "FileInput" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = paymentAttachment.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Anexo Pago";
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
            SetStandardViewBags("Create", true, paymentId, returnUrl, bufferedUrl);
            SetViewBagsForLists(paymentAttachment, project.Id);
            EraseFileNameFieldForAttachments(paymentAttachment);

            return View(paymentAttachment);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: PaymentAttachment/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            // get PaymentAttachment
            if ((id == null) || (id <= 0) || (context.PaymentAttachment == null)) return NotFound();
            PaymentAttachment? paymentAttachment = await context.PaymentAttachment
                .FindAsync(id);
            if (paymentAttachment == null) return NotFound();

            int projectId = await GetProjectIdFromPaymentAttachment(paymentAttachment.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, paymentId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(paymentAttachment, projectId);

            return View(paymentAttachment);
        }


        // POST: PaymentAttachment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] PaymentAttachment paymentAttachment, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            if (id != paymentAttachment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(paymentAttachment);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(paymentAttachment))
                    {
                        context.Update(paymentAttachment);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "PaymentAttachment", "Edit", paymentAttachment.Id, paymentAttachment
                        , new List<string> { "FileInput" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!PaymentAttachmentExists(paymentAttachment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Anexo Pago. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Anexo Pago";
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
            SetStandardViewBags("Edit", false, paymentId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(paymentAttachment, project.Id);

            EraseFileNameFieldForAttachments(paymentAttachment);

            return View(paymentAttachment);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: PaymentAttachment/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.PaymentAttachment == null)) return NotFound();
            PaymentAttachment? paymentAttachment = await context.PaymentAttachment
                .FirstAsync(r => r.Id == id);
            if (paymentAttachment == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (paymentAttachment != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.PaymentAttachment.Remove(paymentAttachment);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "PaymentAttachment", "Delete", paymentAttachment.Id, paymentAttachment
                        , new List<string> { "FileInput" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Anexo Pago";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }

            //---- if not saved reload view ----
            // set ViewBags
            SetStandardViewBags("Delete", false, paymentId, returnUrl, bufferedUrl);
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

            string path = fileLoadService.ServerFullPath(FileLoadService.PathPaymentAttachments);
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
        /// Sorts paymentAttachments by default (DateAttached) or selected order:
        /// title, file or dateAttached
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of PaymentAttachment</returns>
        IQueryable<PaymentAttachment> OrderBySelectedOrDefault(string? sortOrder, IQueryable<PaymentAttachment> paymentAttachments)
        {
            ViewBag.dateAttachedSort = string.IsNullOrEmpty(sortOrder) ? "dateAttached_desc" : "";
            ViewBag.titleSort = sortOrder == "title" ? "title_desc" : "title";
            ViewBag.fileSort = sortOrder == "file" ? "file_desc" : "file";
            ViewBag.titleIcon = "bi-caret-down";
            ViewBag.fileIcon = "bi-caret-down";
            ViewBag.dateAttachedIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "title_desc":
                    paymentAttachments = paymentAttachments.OrderByDescending(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-up-fill";
                    break;
                case "title":
                    paymentAttachments = paymentAttachments.OrderBy(o => o.Title);
                    ViewBag.titleIcon = "bi-caret-down-fill";
                    break;
                case "file_desc":
                    paymentAttachments = paymentAttachments.OrderByDescending(o => o.File);
                    ViewBag.fileIcon = "bi-caret-up-fill";
                    break;
                case "file":
                    paymentAttachments = paymentAttachments.OrderBy(o => o.File);
                    ViewBag.fileIcon = "bi-caret-down-fill";
                    break;
                case "dateAttached_desc":
                    paymentAttachments = paymentAttachments.OrderByDescending(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-up-fill";
                    break;
                default:
                    paymentAttachments = paymentAttachments.OrderBy(t => t.DateAttached);
                    ViewBag.dateAttachedIcon = "bi-caret-down-fill";
                    break;
            }
            return paymentAttachments;
        }

        //==============================================
        //------------- Controller Methods -------------



        /// <summary>
        /// Assigns the standard ViewBags required for navigation.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="returnsbyDefault">If false. The return button will only be available if returnUrl has a value.
        /// If true, will return to caller if no returnUrl is specified.</param>
        void SetStandardViewBags(string action, bool returnsbyDefault, int? paymentId, string? returnUrl, string? bufferedUrl)
        {
            if (paymentId.HasValue) ViewBag.paymentId = paymentId;
            // returnUrl
            if (!string.IsNullOrEmpty(returnUrl))
                ViewBag.returnUrl = returnUrl;
            else if (returnsbyDefault)
                ViewBag.returnUrl = HttpContext.Request.Headers["Referer"];
            // bufferedUrl
            if (!string.IsNullOrEmpty(bufferedUrl)) ViewBag.bufferedUrl = bufferedUrl;
            // navPaymentAttachment
            ViewBag.navPaymentAttachment = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Payment
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(PaymentAttachment? paymentAttachment, int projectId)
        {

            // set options for Payment
            var listPayment = new SelectList(context.Payment
                               .Include(a => a.Product_info)
                               .Where(a => a.Product_info!.Project == projectId)
                               .OrderBy(c => c.DateDelivery)
                               .Select(r => new SelectListItem(r.Code + " - " + ((r.DateDelivery.HasValue == true) ? r.DateDelivery.Value.ToString("yyyy-MMM-dd") : ""), r.Id.ToString())), "Value", "Text", paymentAttachment?.Payment).ToList();
            listPayment.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listPayment = listPayment;
        }


        //======================================================== 

        public IQueryable<PaymentAttachment> GetPaymentAttachmentsForProject(int projectId)
        {
            return context.PaymentAttachment
                       .Include(a => a.Payment_info)
                       .ThenInclude(p => p!.Product_info)
                       .Where(a => a.Payment_info!.Product_info!.Project == projectId)
            ;
        }

        // makes shure active project matches the project for the PaymentAttachment
        async Task<int> GetProjectIdFromPaymentAttachment(int paymentAttachmentId)
        {
            var projectId = await context.PaymentAttachment
                                .Where(r => r.Id == paymentAttachmentId)
                                .Select(r => r.Payment_info!.Product_info!.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(PaymentAttachment paymentAttachment)
        {
            bool updated = false;

            //---- File ----
            if (paymentAttachment.FileInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("File" ,paymentAttachment.Payment, paymentAttachment.Id, paymentAttachment.FileInput);
                int result = await fileLoadService.UploadFile(serverFileName, paymentAttachment.FileInput, FileLoadService.PathPaymentAttachments);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(paymentAttachment.File, serverFileName);
                // set
                paymentAttachment.File = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(PaymentAttachment paymentAttachment)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var fileErrors = ModelState[nameof(paymentAttachment.File)]?.Errors;           
            if (fileErrors == null || fileErrors.Count == 0)
            {
                ModelState.Remove(nameof(paymentAttachment.File));
                paymentAttachment.File = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool PaymentAttachmentExists(int id)
        {
            return (context.PaymentAttachment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
