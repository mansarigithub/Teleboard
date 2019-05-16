using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Teleboard.Models;
using Teleboard.DomainModel.Core;
using Teleboard.DataAccess.Context;
using Teleboard.Validation.Attributes;
using Teleboard.PresentationModel.Model.Tenant;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host")]
    public class TenantsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.Tenants.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tenant tenant = await db.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            return View(tenant);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<ActionResult> Create(TenantPM tenant)
        {
            await TenantBiz.AddTenantAsync(tenant);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            return View(await TenantBiz.ReadTenant(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<ActionResult> Edit(TenantPM tenant)
        {
            await TenantBiz.UpdateTenantAsync(tenant);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tenant tenant = await db.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return HttpNotFound();
            }
            return View(tenant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Tenant tenant = await db.Tenants.FindAsync(id);
            db.Tenants.Remove(tenant);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Change Tenant Chennal Moderation is True
        public async Task<ActionResult> ChennalModerationTrue(int id)
        {
            var tanentFromDB = db.Tenants.FirstOrDefault(t => t.Id == id);
            tanentFromDB.ChennalModeration = false;
            db.Entry(tanentFromDB).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Change Tenant Chennal Moderation is False
        public async Task<ActionResult> ChennalModerationFalse(int id)
        {
            var tanentFromDB = db.Tenants.FirstOrDefault(t => t.Id == id);
            tanentFromDB.ChennalModeration = true;
            db.Entry(tanentFromDB).State = EntityState.Modified;
            await db.SaveChangesAsync();

            // Change Flag of Chennal table false to true if Admin set Moderation is required fasle

            var tenantChennel = db.Channels.Where(o => o.TenantId == id).ToList(); ;
            tenantChennel.ForEach(o => o.Flag = true);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ContentModerationTrue(int id)
        {
            var tanentFromDB = db.Tenants.FirstOrDefault(t => t.Id == id);
            tanentFromDB.ContentModeration = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ContentModerationFalse(int id)
        {
            var tanentFromDB = db.Tenants.FirstOrDefault(t => t.Id == id);
            tanentFromDB.ContentModeration = false;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Change Tenant TimeBox Moderation is True
        public async Task<ActionResult> TimeBoxModerationTrue(int id)
        {
            var tanentFromDB = db.Tenants.FirstOrDefault(t => t.Id == id);
            tanentFromDB.TimeBoxModeration = false;
            db.Entry(tanentFromDB).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Change Tenant TimeBox Moderation is False
        public async Task<ActionResult> TimeBoxModerationFalse(int id)
        {
            var tanentFromDB = db.Tenants.FirstOrDefault(t => t.Id == id);
            tanentFromDB.TimeBoxModeration = true;
            db.Entry(tanentFromDB).State = EntityState.Modified;
            await db.SaveChangesAsync();
            // Change Flag of TimeBox table false to true if Admin set Moderation is required fasle
            var tenantTimeBox = await db.Channels.Join(db.TimeBoxes, c => c.Id, t => t.ChannelId, (c, t) => new TimeBoxViewModel { Channel = c, TimeBox = t }).Where(o => o.Channel.TenantId == id).ToListAsync();
            tenantTimeBox.ForEach(o => o.TimeBox.Flag = true);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
