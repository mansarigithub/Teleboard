using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Teleboard.Common.Data;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host,TenantAdmin")]
    public class ApprovalController : BaseController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(DataSourceRequest request)
        {
            var contents = ContentBiz.ReadUnApprovedContents(ApplicationUser);
            contents.ForEach(c =>
            {
                c.Url = ContentBiz.ComputeContentUrl(c.Source);
                c.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(c.Source);
            });
            return View(contents);
        }

        public async Task<ActionResult> ApproveTimebox(int id)
        {
            TimeBox timebox = await db.TimeBoxes.FindAsync(id);

            db.Entry(timebox).State = EntityState.Modified;
            timebox.Flag = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ContentDetails(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = await db.Contents.FindAsync(id);
            if (content == null) {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == content.TenantId)) {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(content);
        }

        public async Task<ActionResult> ChennalDetails(int? id)
        {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null) {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId)) {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(channel);
        }

        [HttpGet]
        public async Task<ActionResult> ContentApprove(int id)
        {
            Content content = await db.Contents.FindAsync(id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == content.TenantId)) {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Entry(content).State = EntityState.Modified;
            content.Flag = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> ChennalApprove(int id)
        {
            Channel channel = await db.Channels.FindAsync(id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId)) {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Entry(channel).State = EntityState.Modified;
            channel.Flag = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IEnumerable<Tenant> GetTenants(string userId)
        {
            using (var ctx = new ApplicationDbContext()) {
                return ctx.TenantUsers
                    .Where(o => o.UserId == userId)
                    .Join(ctx.Tenants, tu => tu.TenantId, t => t.Id, (tu, t) => t)
                    .ToList();
            }
        }
    }
}