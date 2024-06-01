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
    public partial class ProductController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IParentProjectService parentProjectService;
        private readonly ILogger<ProductController> logger;
        private readonly ILogTools logTools;


        public ProductController(IMRepoDbContext context
            , IParentProjectService parentProjectService
            , ILogger<ProductController> logger
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

        // GET: Product/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            var products = from o in context.Product where (o.Project == project.Id) select o;
            if (!string.IsNullOrEmpty(searchText))
                products = products.Where(p =>
                    p.Name!.Contains(searchText)
                    || p.Cost.ToString()!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (products != null)
            {
                products = OrderBySelectedOrDefault(sortOrder, products);
                return View(await products.ToListAsync());
            }
            return View(new List<Product>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: Product/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Product
            if ((id == null) || (id <= 0) || (context.Product == null)) return NotFound();
            Product? product = await context.Product
                .FindAsync(id);
            if (product == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(product);

            return View(product);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: Product/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            Product product = new()
            {
                Project = project.Id
            };
            return View(product);
        }



        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Product product, string? returnUrl, string? bufferedUrl)
        {
            Project project = parentProjectService.getSessionProject(HttpContext.Session, ViewBag);
            if ((project is null) || (project.Id <= 0)) return RedirectToAction("Select", "Project");
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(product);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Product", "Create", product.Id, product);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = product.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Producto";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.CostValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Cost");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(product);
            return View(product);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get Product
            if ((id == null) || (id <= 0) || (context.Product == null)) return NotFound();
            Product? product = await context.Product
                .FindAsync(id);
            if (product == null) return NotFound();

            int projectId = await GetProjectIdFromProduct(product.Id);

            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(product);

            return View(product);
        }


        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] Product product, string? returnUrl, string? bufferedUrl)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(product);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "Product", "Edit", product.Id, product);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Producto. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Producto";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.CostValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Cost");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);
            SetViewBagsForLists(product);

            return View(product);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: Product/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.Product == null)) return NotFound();
            Product? product = await context.Product
                .Include(t => t.Additions!)
                .ThenInclude(f => f.AdditionAttachments!)
                .Include(t => t.Payments!)
                .ThenInclude(f => f.PaymentAttachments!)
                .FirstAsync(r => r.Id == id);
            if (product == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (product != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    DeleteChildren(product);
                    await context.SaveChangesAsync();
                    context.Product.Remove(product);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "Product", "Delete", product.Id, product);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Producto";
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
        /// Sorts products by default (Name) or selected order:
        /// name or cost
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of Product</returns>
        IQueryable<Product> OrderBySelectedOrDefault(string? sortOrder, IQueryable<Product> products)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.costSort = sortOrder == "cost" ? "cost_desc" : "cost";
            ViewBag.nameIcon = "bi-caret-down";
            ViewBag.costIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "cost_desc":
                    products = products.OrderByDescending(o => o.Cost);
                    ViewBag.costIcon = "bi-caret-up-fill";
                    break;
                case "cost":
                    products = products.OrderBy(o => o.Cost);
                    ViewBag.costIcon = "bi-caret-down-fill";
                    break;
                case "name_desc":
                    products = products.OrderByDescending(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    products = products.OrderBy(t => t.Name);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return products;
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
            // navProduct
            ViewBag.navProduct = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Project
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(Product? product)
        {

            // set options for Project
            var listProject = new SelectList(context.Project
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name + " - " + r.Code, r.Id.ToString())), "Value", "Text", product?.Project).ToList();
            listProject.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listProject = listProject;
        }


        //======================================================== 


        // makes shure active project matches the project for the Product
        async Task<int> GetProjectIdFromProduct(int productId)
        {
            var projectId = await context.Product
                                .Where(r => r.Id == productId)
                                .Select(r => r.Project)
                                .FirstOrDefaultAsync();
            await parentProjectService.checkSessionProject(projectId, HttpContext.Session, User);
            return projectId;
        }

        //----------------------------------------------
        //==============================================
        // delete children

        /// <summary>
        /// Deletes all children records related to the Product:
        /// addition and payment
        /// </summary>
        /// <param name="project"></param>
        void DeleteChildren(Product product)
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
        }
        #endregion


        //========================================================

        private bool ProductExists(int id)
        {
            return (context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
