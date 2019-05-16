using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Teleboard.DomainModel.Core;

namespace Teleboard.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public static class IdentityModelExtension
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this ApplicationUser obj, UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(obj, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public static class ApplicationRoleExtension
    {
        public static async Task<IdentityResult> GenerateRoleIdentityAsync(this ApplicationRole obj, RoleManager<ApplicationRole> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var roleIdentity = await manager.CreateAsync(obj);
            // Add custom user claims here
            return roleIdentity;
        }
    }
}