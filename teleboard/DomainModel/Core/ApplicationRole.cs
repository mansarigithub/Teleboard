using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }


    }
}