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
    public partial class TaskStageController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<TaskStageController> logger;
        private readonly ILogTools logTools;


        public TaskStageController(IMRepoDbContext context
            , ILogger<TaskStageController> logger
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

        // GET: TaskStage/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var taskStages = from o in context.TaskStage select o;
            if (!string.IsNullOrEmpty(searchText))
                taskStages = taskStages.Where(p => p.Name!.Contains(searchText));

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (taskStages != null)
            {
                taskStages = OrderBySelectedOrDefault(sortOrder, taskStages);
                return View(await taskStages.ToListAsync());
            }
            return View(new List<TaskStage>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: TaskStage/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get TaskStage
            if ((id == null) || (id <= 0) || (context.TaskStage == null)) return NotFound();
            TaskStage? taskStage = await context.TaskStage
                .FindAsync(id);
            if (taskStage == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);

            return View(taskStage);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: TaskStage/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);

            return View();
        }



        // POST: TaskStage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TaskStage taskStage, string? returnUrl, string? bufferedUrl)
        {
            // Check if is name unique
            if (context.TaskStage.Any(c => c.Name == taskStage.Name && c.Id != taskStage.Id))
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
                    context.Add(taskStage);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "TaskStage", "Create", taskStage.Id, taskStage);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = taskStage.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Etapa de Trámite";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.OrderValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Order");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            return View(taskStage);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: TaskStage/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get TaskStage
            if ((id == null) || (id <= 0) || (context.TaskStage == null)) return NotFound();
            TaskStage? taskStage = await context.TaskStage
                .FindAsync(id);
            if (taskStage == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);

            return View(taskStage);
        }


        // POST: TaskStage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] TaskStage taskStage, string? returnUrl, string? bufferedUrl)
        {
            if (id != taskStage.Id)
            {
                return NotFound();
            }
            // Check if is name unique
            if (context.TaskStage.Any(c => c.Name == taskStage.Name && c.Id != taskStage.Id))
            {
                ModelState.AddModelError("Name", "Nombre existe en otro registro.");
            }
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(taskStage);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "TaskStage", "Edit", taskStage.Id, taskStage);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!TaskStageExists(taskStage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Etapa de Trámite. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Etapa de Trámite";
                    ViewBag.debugErrorMessage = ex.Message + ex.InnerException?.Message;
                }
            }
            else
            {
                // Invalid ModelState.
                ViewBag.OrderValidationMsg = jaosLibUtils.getMandatoryNumbersErrorMessages(ModelState, "Order");
            }

            //---- if not saved reload View ----
            // set ViewBags
            SetStandardViewBags("Edit", false, returnUrl, bufferedUrl);

            return View(taskStage);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: TaskStage/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.TaskStage == null)) return NotFound();
            TaskStage? taskStage = await context.TaskStage
                .FirstAsync(r => r.Id == id);
            if (taskStage == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            bool isLinked = await findExistingLinks(taskStage);
            if (taskStage != null && !isLinked)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.TaskStage.Remove(taskStage);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "TaskStage", "Delete", taskStage.Id, taskStage);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Etapa de Trámite";
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



        // Finds all the registers that are using the current registry from Etapa de Trámite
        async Task<bool> findExistingLinks(TaskStage taskStage)
        {
            bool isLinked = false;
            string externalLinks = string.Empty;

            //search for Adiciones using this Etapa de Trámite
            List<Addition>             additions = await context.Addition
                .Include(p => p.Product_info)
                .Include(p => p.Stage_info)
                .Where(r => r.Stage == taskStage.Id).ToListAsync();
            if (additions?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Adiciones:<br/>";
                foreach (Addition addition1 in additions)
                    externalLinks += addition1?.Code + "<br/>";
                externalLinks += "<br/>";
            }

            //search for Extensiones using this Etapa de Trámite
            List<Extension>             extensions = await context.Extension
                .Include(p => p.Project_info)
                .Include(p => p.Stage_info)
                .Where(r => r.Stage == taskStage.Id).ToListAsync();
            if (extensions?.Count > 0)
            {
                if (!isLinked) isLinked = true;
                externalLinks += "Se usa en  Extensiones:<br/>";
                foreach (Extension extension1 in extensions)
                    externalLinks += extension1?.Code + " - " + ((extension1?.DateDelivery.HasValue == true) ? extension1?.DateDelivery.Value.ToString("yyyy-MMM-dd") : "") + "<br/>";
                externalLinks += "<br/>";
            }

            if (isLinked) externalLinks = "Etapa de Trámite no puede borrarse<br/>" + externalLinks;
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
        /// Sorts taskStages by default (Name) or selected order:
        /// name
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of TaskStage</returns>
        IQueryable<TaskStage> OrderBySelectedOrDefault(string? sortOrder, IQueryable<TaskStage> taskStages)
        {
            ViewBag.nameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.nameIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "name_desc":
                    taskStages = taskStages.OrderByDescending(t => t.Order);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                default:
                    taskStages = taskStages.OrderBy(t => t.Order);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
            }
            return taskStages;
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
            // navTaskStage
            ViewBag.navTaskStage = $"{action}";
        }


        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool TaskStageExists(int id)
        {
            return (context.TaskStage?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
