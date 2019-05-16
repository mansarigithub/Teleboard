using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Teleboard.Common.Enum;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class ApplicationUser : IdentityUser, IPrincipal
    {
        public ApplicationUser()
        {
            TenantUsers = new List<TenantUser>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IIdentity Identity { get; set; }

        public virtual ICollection<TenantUser> TenantUsers { get; set; }
        public virtual ICollection<AuthenticationToken> AuthenticationTokens { get; set; }
        public virtual ICollection<Content> Contents { get; set; }


        public bool IsInRole(string role)
        {
            return this.Roles.Any(x => x.RoleId == role);
        }

        public bool IsInRole(AppRole role)
        {
            return this.Roles.Any(x => x.RoleId.ToLower() == role.ToString().ToLower());
        }


        public Language Language { get; set; }

        [NotMapped]
        public bool IsHostAdmin
        {
            get
            {
                return Roles.Any(r => r.RoleId.ToLower() == "host");
            }
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public CultureInfo GetCulture()
        {
            return Language == Language.English ? 
                CultureInfo.GetCultureInfo("en-US") : 
                CultureInfo.GetCultureInfo("fa-IR");
        }
    }
}