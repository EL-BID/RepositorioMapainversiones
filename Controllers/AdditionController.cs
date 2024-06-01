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
    public partial class AdditionController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly IFileLoadService fileLoadService;
        private readonly ILogger<AdditionController> logger;
        private readonly ILogTools logTools;


        public AdditionController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , IFileLoadService fileLoadService
            , ILogger<AdditionController> logger
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

        // GET: Addition/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, int? productId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var additions = productId.HasValue
                ? from o in context.Addition where (o.Product == productId) select o
                : GetAdditionsForProject(project.Id);
            if (!string.IsNullOrEmpty(searchText))
                additions = additions.Where(p =>
                    p.Code!.Contains(searchText)
                    || p.Value.ToString()!.Contains(searchText)
                    || p.Stage_info!.Name!.Contains(searchText)
                    || p.DateDelivery.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, productId, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (additions != null)
            {
                additions = OrderBySelectedOrDefault(sortOrder, additions);
                additions = additions
                    .Include(p => p.Stage_info);
                return View(await additions.ToListAsync());
            }
            return View(new List<Addition>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: Addition/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, int? productId, string? returnUrl, string? bufferedUrl)
        {
            // get Addition
            if ((id == null) || (id <= 0) || (context.Addition == null)) return NotFound();
            Addition? addition = await context.Addition
                                    .Include(t => t.AdditionAttachments!)
                .FirstAsync(r => r.Id == id);
            if (addition == null) return NotFound();

            int projectId = await GetProjectIdFromAddition(addition.Id);
            // set ViewBags
            SetStandardViewBags("Display", true, productId, returnUrl, bufferedUrl);
            SetViewBagsForLists(addition, projectId);

            return View(addition);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Addition/Create
        [HttpGet]
        public IActionResult Create(int? productId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, productId, returnUrl, bufferedUrl);
            SetViewBagsForLists(null, project.Id);

            Addition addition = new();
            if (productId.HasValue) addition.Product = productId.Value;
            return View(addition);
        }



        // POST: Addition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Addition addition, int? productId, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // Check if is code unique
            if (context.Addition.Any(c => c.Code == addition.Code && c.Id != addition.Id))
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
                    context.Add(addition);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(addition))
                    {
                        context.Update(addition);
                        await context.SaveChangesAsync();
                    }

                    // Assign Code when created
                    addition.Code = (addition.Id + 1000).ToString("d5");
                    context.Update(addition);
                    await context.SaveChangesAsync();

                    // Commit Transaction
                    logTools.Log(User, "Addition", "Create", addition.Id, addition
                        , new List<string> { "AttachmentInput","Attachmentses" });
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = addition.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Adición";
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
            SetStandardViewBags("Create", true, productId, returnUrl, bufferedUrl);
            SetViewBagsForLists(addition, project.Id);
            EraseFileNameFieldForAttachments(addition);

            return View(addition);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Addition/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, int? productId, string? returnUrl, string? bufferedUrl)
        {
            // get Addition
            if ((id == null) || (id <= 0) || (context.Addition == null)) return NotFound();
            Addition? addition = await context.Addition
                                    .Include(t => t.AdditionAttachments!)
                .FirstAsync(r => r.Id == id);
            if (addition == null) return NotFound();

            int projectId = await GetProjectIdFromAddition(addition.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, productId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(addition, projectId);
            ViewBag.additionId = addition.Id;

            return View(addition);
        }


        // POST: Addition/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Addition addition, int? productId, string? returnUrl, string? bufferedUrl)
        {
            if (id != addition.Id)
            {
                return NotFound();
            }
            // Check if is code unique
            if (context.Addition.Any(c => c.Code == addition.Code && c.Id != addition.Id))
            {
                ModelState.AddModelError("Code", "Código existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(addition);
                    await context.SaveChangesAsync();
                    // Attachments
                    if (await UploadAttachments(addition))
                    {
                        context.Update(addition);
                        await context.SaveChangesAsync();
                    }
                    // Commit Transaction
                    logTools.Log(User, "Addition", "Edit", addition.Id, addition
                        , new List<string> { "AttachmentInput","Attachmentses" });
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!AdditionExists(addition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Adición. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Adición";
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
            SetStandardViewBags("Edit", false, productId, returnUrl, bufferedUrl);
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            SetViewBagsForLists(addition, project.Id);
            ViewBag.additionId = addition.Id;

            EraseFileNameFieldForAttachments(addition);

            return View(addition);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Addition/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, int? productId, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Addition == null)) return NotFound();
            Addition? addition = await context.Addition
                .Include(t => t.AdditionAttachments!)
                .FirstAsync(r => r.Id == id);
            if (addition == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (addition != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    DeleteChildren(addition);
                    await context.SaveChangesAsync();
                    context.Addition.Remove(addition);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Addition", "Delete", addition.Id, addition
                        , new List<string> { "AttachmentInput","Attachmentses" });
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Adición";
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

            string path = fileLoadService.ServerFullPath(FileLoadService.PathAdditions);
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
        /// Sorts additions by default (DateDelivery) or selected order:
        /// code, value, stage or dateDelivery
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Addition</returns>
        IQueryable<Addition> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Addition> additions)
        {
            ViewBag.dateDeliverySort = string.IsNullOrEmpty(sortOrder) ? "dateDelivery_desc" : "";
            ViewBag.codeSort = sortOrder == "code" ? "code_desc" : "code";
            ViewBag.valueSort = sortOrder == "value" ? "value_desc" : "value";
            ViewBag.stageSort = sortOrder == "stage" ? "stage_desc" : "stage";
            ViewBag.codeIcon = "bi-caret-down";
            ViewBag.valueIcon = "bi-caret-down";
            ViewBag.stageIcon = "bi-caret-down";
            ViewBag.dateDeliveryIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "code_desc":
                    additions = additions.OrderByDescending(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-up-fill";
                    break;
                case "code":
                    additions = additions.OrderBy(o => o.Code);
                    ViewBag.codeIcon = "bi-caret-down-fill";
                    break;
                case "value_desc":
                    additions = additions.OrderByDescending(o => o.Value);
                    ViewBag.valueIcon = "bi-caret-up-fill";
                    break;
                case "value":
                    additions = additions.OrderBy(o => o.Value);
                    ViewBag.valueIcon = "bi-caret-down-fill";
                    break;
                case "stage_desc":
                    additions = additions.OrderByDescending(o => o.Stage);
                    ViewBag.stageIcon = "bi-caret-up-fill";
                    break;
                case "stage":
                    additions = additions.OrderBy(o => o.Stage);
                    ViewBag.stageIcon = "bi-caret-down-fill";
                    break;
                case "dateDelivery_desc":
                    additions = additions.OrderByDescending(t => t.DateDelivery);
                    ViewBag.dateDeliveryIcon = "bi-caret-up-fill";
                    break;
                default:
                    additions = additions.OrderBy(t => t.DateDelivery);
                    ViewBag.dateDeliveryIcon = "bi-caret-down-fill";
                    break;
            }
            return additions;
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
            // navAddition
            ViewBag.navAddition = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Product and Stage
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(Addition? addition, int projectId)
        {

            // set options for Product
            var listProduct = new SelectList(context.Product
                               .Where(a => a.Project == projectId)
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", addition?.Product).ToList();
            listProduct.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProduct = listProduct;

            // set options for Stage
            var listStage = new SelectList(context.TaskStage
                               .OrderBy(c => c.Order)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", addition?.Stage).ToList();
            listStage.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listStage = listStage;
        }


        //======================================================== 

        public IQueryable<Addition> GetAdditionsForProject(int projectId)
        {
            return context.Addition
                       .Include(a => a.Product_info)
                       .Where(a => a.Product_info!.Project == projectId)
            ;
        }

        // makes shure active project matches the project for the Addition
        async Task<int> GetProjectIdFromAddition(int additionId)
        {
            var projectId = await context.Addition
                                .Where(r => r.Id == additionId)
                                .Select(r => r.Product_info!.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        // delete children

        /// <summary>
        /// Deletes all children records related to the Addition:
        /// additionAttachment
        /// </summary>
        /// <param name="project"></param>
        void DeleteChildren(Addition addition)
        {
            if (addition.AdditionAttachments?.Count > 0)
                addition.AdditionAttachments.ToList().ForEach(c => context.AdditionAttachment.Remove(c));
        }
        #endregion
        #region Attachments

        async Task<bool> UploadAttachments(Addition addition)
        {
            bool updated = false;

            //---- Attachment ----
            if (addition.AttachmentInput != null)
            {
                // upload
                string serverFileName = fileLoadService.ServerFileName("Attachment" ,addition.Product, addition.Id, addition.AttachmentInput);
                int result = await fileLoadService.UploadFile(serverFileName, addition.AttachmentInput, FileLoadService.PathAdditions);
                if (result != FileLoadService.resultOK)
                    throw new FileLoadService.UploadFileException(addition.Attachment, serverFileName);
                // set
                addition.Attachment = serverFileName;
                updated = true;
            }
            return updated;
        }


        void EraseFileNameFieldForAttachments(Addition addition)
        {
            // If model could not be saved and there are no errors for Attachment
            // Erase Attachment to match AttachmentInfo
            // Attachment info will be automatically erased for security reasons.
            var attachmentErrors = ModelState[nameof(addition.Attachment)]?.Errors;           
            if (attachmentErrors == null || attachmentErrors.Count == 0)
            {
                ModelState.Remove(nameof(addition.Attachment));
                addition.Attachment = string.Empty;
            }
        }
        #endregion


        //========================================================

        private bool AdditionExists(int id)
        {
            return (context.Addition?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
