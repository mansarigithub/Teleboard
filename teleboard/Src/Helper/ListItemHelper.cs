using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Teleboard.DomainModel.Core;
using Teleboard.Models;
using Teleboard.DataAccess.Context;

namespace Teleboard.Helper
{
    public class ListItemHelper
    {
        public static IEnumerable<SelectListItem> GetSelectTenants(ApplicationDbContext db, IPrincipal user)
        {
            var list = new List<SelectListItem>();
            List<Tenant> tenants = new List<Tenant>();
            if (user.IsInRole("Host"))
            {
                tenants = db.Tenants.ToList();
            }
            else
            {
                int[] tenantIds = db.GetTenantsFromUser(user.Identity.GetUserId(), user.IsInRole("Host"));
                tenants = db.Tenants.Where(o => tenantIds.Contains(o.Id)).ToList();
            }

            tenants.ForEach(o => list.Add(new SelectListItem
            {
                Text = o.Name,
                Value = o.Id.ToString(),
            }));

            return list;
        }
    }
}