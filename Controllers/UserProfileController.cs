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
    public partial class UserProfileController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly ILogger<UserProfileController> logger;
        private readonly ILogTools logTools;


        public UserProfileController(IMRepoDbContext context
            , ILogger<UserProfileController> logger
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

        // GET: UserProfile/Index/
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchText, string? returnUrl, string? bufferedUrl)
        {
            var userProfiles = from o in context.UserProfile select o;
            if (!string.IsNullOrEmpty(searchText))
                userProfiles = userProfiles.Where(p =>
                    p.Email!.Contains(searchText)
                    || p.Name!.Contains(searchText)
                    || p.Surname!.Contains(searchText)
                );

            // set ViewBags
            SetStandardViewBags("Index", false, returnUrl, bufferedUrl);
            ViewBag.searchText = searchText;

            if (userProfiles != null)
            {
                userProfiles = OrderBySelectedOrDefault(sortOrder, userProfiles);
                return View(await userProfiles.ToListAsync());
            }
            return View(new List<UserProfile>());
        }


        #endregion
        #region Display


        //----------- Display

        // GET: UserProfile/Display/5
        [HttpGet]
        public async Task<IActionResult> Display(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get UserProfile
            if ((id == null) || (id <= 0) || (context.UserProfile == null)) return NotFound();
            UserProfile? userProfile = await context.UserProfile
                .FindAsync(id);
            if (userProfile == null) return NotFound();

            // set ViewBags
            SetStandardViewBags("Display", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(userProfile);

            return View(userProfile);
        }


        #endregion
        #region Create


        //----------- Create

        // GET: UserProfile/Create
        [HttpGet]
        public IActionResult Create(string? returnUrl, string? bufferedUrl)
        {
            // set ViewBags
            SetStandardViewBags("Create", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(null);

            return View();
        }



        // POST: UserProfile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] UserProfile userProfile, string? returnUrl, string? bufferedUrl)
        {
            unselectedLinksNulled(userProfile);
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    // Insert
                    context.Add(userProfile);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "UserProfile", "Create", userProfile.Id, userProfile);
                    transaction.Commit();
                    // Goto Edit
                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return RedirectToAction(nameof(Edit), new { id = userProfile.Id, returnUrl, bufferedUrl });
                }
                catch (Exception ex)
                {
                    // Error creating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error creando Perfil de Usuario";
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
            SetViewBagsForLists(userProfile);
            return View(userProfile);
        }
        #endregion
        #region Edit


        //----------- Edit

        // GET: UserProfile/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? bufferedUrl)
        {
            // get UserProfile
            if ((id == null) || (id <= 0) || (context.UserProfile == null)) return NotFound();
            UserProfile? userProfile = await context.UserProfile
                .FindAsync(id);
            if (userProfile == null) return NotFound();


            jaosLibUtils.receiveMessagesFromTempData(TempData, ViewBag);
            // set ViewBags
            SetStandardViewBags("Edit", true, returnUrl, bufferedUrl);
            SetViewBagsForLists(userProfile);

            return View(userProfile);
        }


        // POST: UserProfile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [FromForm] UserProfile userProfile, string? returnUrl, string? bufferedUrl)
        {
            if (id != userProfile.Id)
            {
                return NotFound();
            }
            unselectedLinksNulled(userProfile);
            if (ModelState.IsValid)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.Update(userProfile);
                    await context.SaveChangesAsync();
                    // Commit Transaction
                    logTools.Log(User, "UserProfile", "Edit", userProfile.Id, userProfile);
                    transaction.Commit();
                    ViewBag.okMessage = "La información ha sido actualizada.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Same record being updated. Try again.
                    transaction.RollbackToSavepoint("startingPoint");
                    if (!UserProfileExists(userProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ViewBag.errorMessage = "Concurrency Error Actualizando Perfil de Usuario. Por favor intente nuevamente..";
                    }
                }
                catch (Exception ex)
                {
                    // Error updating info.
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error Actualizando Perfil de Usuario";
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
            SetViewBagsForLists(userProfile);

            return View(userProfile);
        }
        #endregion
        #region Delete


        //----------- Delete

        // POST: UserProfile/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id, string? returnUrl, string? bufferedUrl)
        {
            if ((id == null) || (id <= 0) || (context.UserProfile == null)) return NotFound();
            UserProfile? userProfile = await context.UserProfile
                .FirstAsync(r => r.Id == id);
            if (userProfile == null) return NotFound();

            if (string.IsNullOrEmpty(returnUrl)) returnUrl = HttpContext.Request.Headers["Referer"];
            if (string.IsNullOrEmpty(returnUrl)) return NotFound();

            if (userProfile != null)
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    transaction.CreateSavepoint("startingPoint");
                    context.UserProfile.Remove(userProfile);
                    await context.SaveChangesAsync();
                    logTools.Log(User, "UserProfile", "Delete", userProfile.Id, userProfile);
                    transaction.Commit();

                    ViewBag.okMessage = "La información ha sido actualizada.";
                    jaosLibUtils.sendMessagesOnTempData(TempData, ViewBag);
                    return Redirect(returnUrl);
                }
                catch (Exception ex)
                {
                    transaction.RollbackToSavepoint("startingPoint");
                    ViewBag.errorMessage = "Error borrando Perfil de Usuario";
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
        /// Sorts userProfiles by default (Email) or selected order:
        /// email, name or surname
        /// </summary>
        /// <param name="sortOrder">Name of the field that will be used to sort.
        /// Receiving null will sort in default order. 
        /// Receiving field name sorts in ascending order.
        /// Receiving the field name followed by "_desc" sorts in descending order
        /// </param>
        /// <param name="projects"></param>
        /// <returns>Sorted IQueryable list of UserProfile</returns>
        IQueryable<UserProfile> OrderBySelectedOrDefault(string? sortOrder, IQueryable<UserProfile> userProfiles)
        {
            ViewBag.emailSort = string.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.nameSort = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.surnameSort = sortOrder == "surname" ? "surname_desc" : "surname";
            ViewBag.emailIcon = "bi-caret-down";
            ViewBag.nameIcon = "bi-caret-down";
            ViewBag.surnameIcon = "bi-caret-down";
            switch (sortOrder)
            {
                case "name_desc":
                    userProfiles = userProfiles.OrderByDescending(o => o.Name);
                    ViewBag.nameIcon = "bi-caret-up-fill";
                    break;
                case "name":
                    userProfiles = userProfiles.OrderBy(o => o.Name);
                    ViewBag.nameIcon = "bi-caret-down-fill";
                    break;
                case "surname_desc":
                    userProfiles = userProfiles.OrderByDescending(o => o.Surname);
                    ViewBag.surnameIcon = "bi-caret-up-fill";
                    break;
                case "surname":
                    userProfiles = userProfiles.OrderBy(o => o.Surname);
                    ViewBag.surnameIcon = "bi-caret-down-fill";
                    break;
                case "email_desc":
                    userProfiles = userProfiles.OrderByDescending(t => t.Email);
                    ViewBag.emailIcon = "bi-caret-up-fill";
                    break;
                default:
                    userProfiles = userProfiles.OrderBy(t => t.Email);
                    ViewBag.emailIcon = "bi-caret-down-fill";
                    break;
            }
            return userProfiles;
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
            // navUserProfile
            ViewBag.navUserProfile = $"{action}";
        }


        /// <summary>
        /// Sets the ViewBag content for the SelectLists: Office
        /// </summary>
        /// <param name="project">The project that will be used to set the default values.</param>
        void SetViewBagsForLists(UserProfile? userProfile)
        {

            // set options for Office
            var listOffice = new SelectList(context.Office
                               .OrderBy(c => c.Name)
                               .Select(r => new SelectListItem(r.Name, r.Id.ToString())), "Value", "Text", userProfile?.Office).ToList();
            listOffice.Insert(0, new SelectListItem { Value = "0", Text = JaosUITitles.selectAnOption });
            ViewBag.listOffice = listOffice;
        }


        
        /// <summary>
        /// For the selects that allow null value, converts any selected value 0 to null.
        /// </summary>
        /// <param name="userProfile">The UserProfile where selects will be validated.</param>
        void unselectedLinksNulled(UserProfile userProfile)
        {
            // sets unselected lists to null
            if (userProfile.Office == 0) userProfile.Office = null;

        }

        //----------------------------------------------
        //==============================================
        #endregion


        //========================================================

        private bool UserProfileExists(int id)
        {
            return (context.UserProfile?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
