using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Teleboard.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Teleboard.Localization.ExtensionMethod;
using Teleboard.DomainModel.Core;
using Teleboard.DataAccess.Context;
using Teleboard.Localization;
using Teleboard.Common.Exception;
using Teleboard.UI.Infrastructure.Globalization;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host")]
    public class AdminController : BaseController
    {
        public AdminController()
        {
        }

        public ActionResult Roles()
        {
            var roles = RoleManager.Roles.Select(o => new GetRolesViewModel { Id = o.Id, Name = o.Name, Description = o.Description }).OrderBy(o => o.Name).ToList();
            return View(roles);
        }

        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddRole(AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {

                var role = new ApplicationRole
                {
                    Name = model.Name,
                    Description = model.Description
                };

                var result = await RoleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> EditRole(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            var editRoleViewMOdel = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            };

            return View(editRoleViewMOdel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(model.Id);

                if (role != null)
                {
                    role.Name = model.Name;
                    role.Description = model.Description;
                }

                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> DeleteRole(string Id)
        {
            var role = await RoleManager.FindByIdAsync(Id);

            if (role != null)
            {
                await RoleManager.DeleteAsync(role);
            }

            return RedirectToAction("Roles");
        }


        public ActionResult Users()
        {
            var users = UserManager.Users.ToList().Select(o => new GetUsersViewModel
            {
                Id = o.Id,
                Email = o.Email,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Roles = string.Join(", ", UserManager.GetRoles(o.Id).OrderBy(n => n).ToArray()),
                Tenants = string.Join(", ", GetTenants(o.Id).OrderBy(n => n.Name).Select(t => t.Name).ToArray()),
                Language = o.GetCulture().Name.ToUpper(),
            }).OrderBy(o => o.Email).ToList();

            return View(users);
        }


        public async Task<ActionResult> AddUser()
        {
            var user = new AddUserViewModel();
            user.RoleInfos = RoleManager.Roles.ToList();

            user.AllRoles = user.RoleInfos.OrderBy(o => o.Name).Select(o => o.Name).ToList();
            user.SelectedRoles = new string[] { };

            user.TenantInfos = await GetTenants();

            user.AllTenants = user.TenantInfos.OrderBy(o => o.Name).Select(o => o.Id.ToString());
            user.SelectedTenants = new string[] { };

            return View(user);
        }

        private async Task<IEnumerable<Tenant>> GetTenants()
        {
            using (var ctx = new ApplicationDbContext())
            {
                return await ctx.Tenants.ToListAsync();
            }
        }

        private bool HasRole(ApplicationUser user, string roleId)
        {
            return true;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(AddUserViewModel model)
        {
            string randomPassword = Guid.NewGuid().ToString();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Language = model.Language
                };

                var result = await UserManager.CreateAsync(user, randomPassword);

                if (result.Succeeded)
                {
                    user = UserManager.FindByEmail(model.Email);

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                    result = await UserManager.ConfirmEmailAsync(user.Id, code);

                    if (result.Succeeded)
                    {
                        model.SelectedRoles = model.SelectedRoles ?? new List<string>();
                        result = await UserManager.AddToRolesAsync(user.Id, model.SelectedRoles.ToArray());

                        if (result.Succeeded)
                        {
                            if (await AddTenantsToUser(model, user))
                            {
                                if (user != null && await UserManager.IsEmailConfirmedAsync(user.Id))
                                {
                                    code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                                    var callbackUrl = Url.Action("ResetPassword", "Account",
                                    new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme);
                                    await UserManager.SendEmailAsync(user.Id, Resources.Welcome, string.Format(Resources.AccountCreatedEmailBody, callbackUrl));
                                }
                                else
                                {
                                    ModelState.AddModelError("", Resources.UserNotFound);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", Resources.CannotAddTenantsToUser);
                            }

                            return RedirectToAction("Users");
                        }
                        else
                        {
                            ModelState.AddModelError("", Resources.CannotAddRolesToUser);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", Resources.EmailConfirmationFailed);
                    }


                }

                AddErrors(result);
            }

            model.RoleInfos = RoleManager.Roles.ToList();

            model.AllRoles = model.RoleInfos.OrderBy(o => o.Name).Select(o => o.Name).ToList();
            model.TenantInfos = await GetTenants();

            model.AllTenants = model.TenantInfos.OrderBy(o => o.Name).Select(o => o.Id.ToString());

            return View(model);
        }

        private async Task<bool> AddTenantsToUser(AddUserViewModel model, ApplicationUser user)
        {
            try
            {
                if (model.SelectedRoles != null && model.SelectedRoles.Count() > 0)
                {
                    using (var ctx = new ApplicationDbContext())
                    {
                        foreach (var item in model.SelectedTenants)
                        {
                            ctx.TenantUsers.Add(new TenantUser { TenantId = int.Parse(item), UserId = user.Id });
                        }

                        var result = await ctx.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ActionResult> EditUser(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            var userViewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Language = user.Language
            };
            userViewModel.RoleInfos = RoleManager.Roles.ToList();
            userViewModel.AllRoles = userViewModel.RoleInfos.OrderBy(o => o.Name).Select(o => o.Name).ToList();
            userViewModel.SelectedRoles = UserManager.GetRoles(user.Id).ToArray();
            userViewModel.TenantInfos = await GetTenants();
            userViewModel.AllTenants = userViewModel.TenantInfos.OrderBy(o => o.Name).Select(o => o.Id.ToString());
            userViewModel.SelectedTenants = await GetTenantIds(user.Id);
            return View(userViewModel);
        }

        private async Task<IEnumerable<string>> GetTenantIds(string userId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                return await ctx.TenantUsers
                    .Where(o => o.UserId == userId)
                    .Select(o => o.TenantId.ToString())
                    .ToListAsync();
            }
        }

        private IEnumerable<Tenant> GetTenants(string userId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.TenantUsers
                    .Where(o => o.UserId == userId)
                    .Join(ctx.Tenants, tu => tu.TenantId, t => t.Id, (tu, t) => t)
                    .ToList();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Language = model.Language;
            await UserManager.UpdateAsync(user);

            if (model.Id == ApplicationUser.Id)
            {
                this.ApplicationUser.Language = model.Language;
            }

            var userRoles = UserManager.GetRoles(user.Id).ToArray();
            await UserManager.RemoveFromRolesAsync(user.Id, userRoles);

            if (model.SelectedRoles != null && model.SelectedRoles.Count() > 0)
            {
                await UserManager.AddToRolesAsync(user.Id, model.SelectedRoles.ToArray());
            }
            await AddUserToTenantsAsync(user.Id, model.SelectedTenants);
            return RedirectToAction("Users");
        }

        private async Task AddUserToTenantsAsync(string userId, IEnumerable<string> selectedTenants)
        {
            using (var ctx = new ApplicationDbContext())
            {
                ctx.TenantUsers.RemoveRange(ctx.TenantUsers.Where(o => o.UserId == userId));
                if (selectedTenants != null && selectedTenants.Any())
                    foreach (var tenantId in selectedTenants)
                    {
                        ctx.TenantUsers.Add(new TenantUser { UserId = userId, TenantId = int.Parse(tenantId) });
                    }
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<ActionResult> DeleteUser(string Id)
        {
            var user = await UserManager.FindByIdAsync(Id);

            if (user != null)
            {
                await UserManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                throw new BusinessException(Resources.UserEmailIsNotConfirmed);
            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, Resources.ResetPassword, string.Format(Resources.ResetPasswordEmailBody, callbackUrl));

            return Json(true);
        }

        public async Task<ActionResult> ResetPasswordToDefault(string Id)
        {
            var user = await UserManager.FindByIdAsync(Id);
            user.EmailConfirmed = true;
            user.PasswordHash = UserManager.PasswordHasher.HashPassword("12345678");
            await UserManager.UpdateAsync(user);
            return RedirectToAction("Users");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}