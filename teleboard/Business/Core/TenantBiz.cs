using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.Common.Data;
using System.Data.Entity;
using Teleboard.Mapper.Core;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.DomainModel.Core;
using Teleboard.Common.Exception;
using Teleboard.Localization;
using Teleboard.PresentationModel.Model.Tenant;
using Teleboard.Common.Enum;
using Z.EntityFramework.Plus;

namespace Teleboard.Business.Core
{
    public class TenantBiz : BizBase<Tenant>
    {
        private ApplicationDbContext Context { get; set; }

        public TenantBiz(ApplicationDbContext context) : base(context)
        {
            Context = context;
        }

        public IEnumerable<TenantPM> ReadAllTenants()
        {
            return Context.Tenants.MapTo<TenantPM>().ToList();
        }

        public IEnumerable<TenantPM> ReadUserTenants(string userId)
        {
            return Context.TenantUsers
                .Where(tu => tu.UserId == userId)
                .Select(tu => tu.Tenant)
                .MapTo<TenantPM>()
                .ToList();
        }

        public async Task<TenantPM> ReadTenant(int tenantId)
        {
            var tanent = await Context.Tenants.SingleAsync(t => t.Id == tenantId);
            return tanent.GetTenantPM();
        }

        public async Task AddTenantAsync(TenantPM tenantPM)
        {
            var tenant = tenantPM.GetTenant();
            tenant.SubscriptionKey = Guid.NewGuid();
            Add(tenant);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateTenantAsync(TenantPM tenantPM)
        {
            await Read(t => t.Id == tenantPM.Id)
                .UpdateAsync(t => new Tenant()
                {
                    Name = tenantPM.Name,
                    Description = tenantPM.Description,
                    AdvertisementStatus = tenantPM.AdvertisementStatus
                });
            if (tenantPM.AdvertisementStatus == TenantAdvertisementStatus.Disabled)
            {
                await Context.Devices.UpdateAsync(d => new Device() { PlayAdvertisement = false });
            }
            await Context.SaveChangesAsync();
        }
    }
}
