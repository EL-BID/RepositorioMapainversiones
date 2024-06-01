using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JaosLib.Areas.UserAdmin.Models;
using IMRepo.Models.Domain;
using JaosLib.Services.Utilities;
using IMRepo.Data;
using IMRepo;

namespace IMRepo.Areas.UserAdmin.Controllers
{
    [Authorize]
    [Area("UserAdmin")]

    public class HomeController : Controller
    {
        private readonly IMRepoDbContext context;
        private readonly IUserService userService;
        private readonly UserManager<IdentityUser> userManager;

        public HomeController(IMRepoDbContext context,
            IUserService userService,
            UserManager<IdentityUser> userManager
            )
        {
            this.context = context;
            this.userService = userService;
            this.userManager = userManager;
        }

        // GET: HomeController
        public async Task<ActionResult> Index(int errorId, string email, string searchText)
        {
            if (User?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                if (context != null)
                {
                    List<UserProfile>? userProfiles = context?.UserProfile?.ToList();

                    List<IdentityUser> users;
                    if (!string.IsNullOrEmpty(searchText))
                        users = getFilteredUsers(searchText);
                    else
                        users = userManager.Users.OrderBy(u => u.Email).ToList();
                    List<UserSetupModel> models = new List<UserSetupModel>();
                    foreach (IdentityUser user in users)
                    {
                        UserSetupModel model = new UserSetupModel();
                        model.user = user;

                        if (userProfiles?.Count > 0)
                        {
                            model.profile = userProfiles.FirstOrDefault(p => p.Email == user.Email);
                            if (model.profile != null)
                                model.roleName = await userService.getRoleNameFor(model.profile.AspNetUserId);
                        }
                        models.Add(model);
                    }

                    switch (errorId)
                    {
                        case 30:
                            ViewBag.okMessage = "Password reset para " + email;
                            //ViewBag.okMessage = "Password has been reset for " + email;
                            break;
                        case 31:
                            ViewBag.warningMessage = "No se pudo hacer el Password reset" + email;
                            //ViewBag.warningMessage = "Password could not be reset for " + email;
                            break;
                        case 32:
                            ViewBag.errorMessage = "Error haciendo el password reset" + email;
                            //ViewBag.errorMessage = "Error reseting password for " + email;
                            break;
                    }
                    ViewBag.searchText = searchText;
                    return View(models);
                }
            }
            return NotFound();

        }

        List<IdentityUser> getFilteredUsers(string searchText)
        {
            var userIds = (from u in userManager.Users
                           join p in context.UserProfile on u.Email equals p.Email
                           where u.UserName.Contains(searchText)
                           || p.Surname.Contains(searchText)
                           || p.Name.Contains(searchText)
                           select u.Id).ToList();

            // Fetch the users based on the filtered user IDs
            var users = userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .OrderBy(u => u.Email)
                .ToList();

            return users;
        }



        // GET: HomeController/Create
        async public Task<IActionResult> Create(string? mail)
        {
            if (User?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                UserProfile? userProfile = await context.UserProfile.FirstOrDefaultAsync(u => u.Email == mail);
                IdentityUser? user = await userManager.Users.FirstOrDefaultAsync(u => u.Email == mail);
                if (user != null)
                {
                    try
                    {
                        UserProfileModel model = new UserProfileModel();
                        model.AspNetUserId = user.Id;
                        model.Email = user.Email;
                        model.role = await userService.getRoleIdFor(user.Id);
                        setViewDatasForLists();
                        return View(model);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.errorMessage = "Error mostrando el perfil de usuario";
                        //ViewBag.errorMessage = "Error showing User Profile";
                        ViewBag.debugErrorMessage = ex.Message;
                    }

                }
                return RedirectToAction(nameof(Index));
            }
            else
                return NotFound();
        }






        // POST: UserProfile/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(@"
                                              Id_userProfile
                                              ,AspNetUserId
                                              ,Email
                                              ,Surname
                                              ,Name
                                              ,Office
                                              ,Notes
                                              ,role
                                              ")] UserProfileModel userProfileModel, int? captura)
        {
            if (User?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                // sets unselected lists to null
                if (userProfileModel.Office == 0) userProfileModel.Office = null;

                if (ModelState.IsValid)
                {
                    try
                    {
                        context.Add(userProfileModel);
                        await context.SaveChangesAsync();
                        await userService.setRole(User, userProfileModel.AspNetUserId, userProfileModel.role.HasValue ? userProfileModel.role.Value : 0);
                        ViewBag.okMessage = "Información guardada.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ViewBag.warningMessage = "Error creando perfil de usuario";
                        ViewBag.debugMessage = ex.Message;
                    }

                }
                setViewDatasForLists();
                return View(userProfileModel);
            }
            else
                return NotFound();
        }

        //----------- Edit

        // GET: UserProfile/Edit/5
        public async Task<IActionResult> Edit(string? mail)
        {
            if (User?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                if ((string.IsNullOrEmpty(mail)) || (context.UserProfile == null))
                    return NotFound();
                UserProfile? userProfile = await context.UserProfile
                           .AsNoTracking().FirstOrDefaultAsync(x => x.Email == mail);
                if (userProfile == null)
                    return NotFound();

                var viewModel = new UserProfileModel()
                {
                    Id = userProfile.Id,
                    AspNetUserId = userProfile.AspNetUserId,
                    Email = userProfile.Email,
                    Surname = userProfile.Surname,
                    Name = userProfile.Name,
                    Office = (userProfile.Office == 0) ? null : userProfile.Office,
                    Notes = userProfile.Notes,
                };
                viewModel.role = await userService.getRoleIdFor(viewModel.AspNetUserId);

                setViewDatasForLists(viewModel);
                return View(viewModel);
            }
            else
                return NotFound();
        }


        // POST: UserProfile/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? mail, [Bind(@"
                                              Id
                                              ,AspNetUserId
                                              ,Email
                                              ,Surname
                                              ,Name
                                              ,Office
                                              ,Notes
                                              ,role
                                                            ")] UserProfileModel userProfileModel)
        {
            if (User?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                if (string.IsNullOrEmpty(userProfileModel.Email) || mail != userProfileModel.Email)
                {
                    return NotFound();
                }
                // sets unselected lists to null
                if (userProfileModel.Office == 0) userProfileModel.Office = null;
                if (userProfileModel.role == 0) userProfileModel.role = null;

                if (ModelState.IsValid)
                {
                    try
                    {
                        await userService.setRole(User, userProfileModel.AspNetUserId, userProfileModel.role.HasValue ? userProfileModel.role.Value : 0);

                        context.Update(userProfileModel);
                        await context.SaveChangesAsync();
                        ViewBag.okMessage = "Perfil de usuario actualizado";
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserProfileExists(userProfileModel.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            ViewBag.warningMessage = "Error actualizando perfil de usuario";
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.warningMessage = "Error actualizando perfil de usuario";
                        ViewBag.debugMessage = ex.Message;
                    }
                    //                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));

                //setViewDatasForLists(userProfileModel);
                //return View(userProfileModel);
            }
            else
                return NotFound();
        }


        // GET: HomeController/ChangePassword/5
        public ActionResult ChangePassword(string? mail)
        {
            return View();
        }

        // POST: HomeController/ChangePassword
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string? mail, [Bind(@"
                                              old_password
                                              ,new_password
                                              ,repeat_password
                                                            ")] PasswordChangeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    IdentityUser user1 = userManager.FindByEmailAsync(User?.Identity?.Name).Result;
                    var code = await userManager.GeneratePasswordResetTokenAsync(user1);
                    var result = await userManager.ResetPasswordAsync(user1, code, model.new_password);
                    if (!result.Succeeded)
                    {
                        ViewBag.errorMessage = "No se pudo cambiar el password";
                    }
                    else
                        ViewBag.okMessage = "Password modificado";
                }
                catch
                {
                    ViewBag.errorMessage = "Error modificando password";
                }
            }
            return View();
        }

        // POST: HomeController/ResetPassword/5
        [HttpPost]
        public async Task<ActionResult> ResetPassword(string id, IFormCollection collection)
        {
            if (User?.IsInRole(ProjectGlobals.RoleAdmin) == true)
            {
                try
                {
                    IdentityUser user1 = userManager.FindByEmailAsync(id).Result;
                    var code = await userManager.GeneratePasswordResetTokenAsync(user1);
                    var result = await userManager.ResetPasswordAsync(user1, code, ProjectGlobals.defaultPassword);
                    if (!result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index), new { errorId = 31, email = id });
                    }
                    else
                        return RedirectToAction(nameof(Index), new { errorId = 30, email = id });
                }
                catch
                {
                    return RedirectToAction(nameof(Index), new { errorId = 32, email = id });
                }
            }
            else
                return NotFound();
        }



        //----------- Controller Methods

        void setViewDatasForLists(UserProfileModel viewModel)
        {

            // roles
            List<IdentityRole> roles = userService.getRoles(User);
            var listRoles = new SelectList(roles, "Id", "Name", viewModel.role).ToList();
            listRoles.Insert(0, new SelectListItem { Value = "0", Text = "Select an option" });
            ViewData["Roles"] = listRoles;
        }


        void setViewDatasForLists()
        {
            // roles
            List<IdentityRole> roles = userService.getRoles(User);
            var listRoles = new SelectList(roles, "Id", "Name").ToList();
            listRoles.Insert(0, new SelectListItem { Value = "0", Text = "Select an option" });
            ViewData["Roles"] = listRoles;
        }

        private bool UserProfileExists(int id)
        {
            return (context.UserProfile?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
