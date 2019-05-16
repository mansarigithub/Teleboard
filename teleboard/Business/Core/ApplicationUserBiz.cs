using System.Threading.Tasks;
using System.Linq;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using System.Data.Entity;
using System;
using System.Collections.Generic;

namespace Teleboard.Business.Core
{
    public class ApplicationUserBiz : BizBase<ApplicationUser>
    {
        private ApplicationDbContext Context { get; set; }

        public ApplicationUserBiz(ApplicationDbContext context) : base(context)
        {
            Context = context;
        }

        public async Task<ApplicationUser> FindUserByTokenAsync(string authKey)
        {
            return (await Context.Users.SingleOrDefaultAsync(u => u.AuthenticationTokens.Any(t => t.Token == authKey)));
        }

        public async Task<bool> UserHasMembershipInTenantAsync(string userId, int tenantId)
        {
            return (await Context.TenantUsers.AnyAsync(tu => tu.UserId == userId && tu.TenantId == tenantId));
        }

        public void UpdateUser(string id, string firstName, string lastName, IEnumerable<string> selectedRoles, IEnumerable<string> selectedTenants)
        {
            var user = Include(u => u.Roles).Single(u => u.Id == id);
            user.FirstName = firstName;
            user.LastName = lastName;

            
        }
    }
}
