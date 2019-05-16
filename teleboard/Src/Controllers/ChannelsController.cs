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
using Teleboard.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Teleboard.DomainModel.Core;
using Teleboard.DataAccess.Context;
using Teleboard.Common.Data;
using Teleboard.Business.Core;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.Common.ExtensionMethod;
using Teleboard.PresentationModel.Model.Channel;
using Teleboard.Mapper.Core;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host,TenantAdmin")]
    public class ChannelsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ContentsController _ContentsController = new ContentsController();


        public ChannelsController()
        {
        }

        [HttpGet]
        public ActionResult ReadChannels(DataSourceRequest request)
        {
            var channels = ApplicationUser.IsInRole("Host") ?
                ChannelBiz.ReadAllChannels(request) :
                ChannelBiz.ReadChannelsForTenantAdmin(ApplicationUser, request);
            return Json(channels, JsonRequestBehavior.AllowGet);
        }





















        // GET: Channels
        public async Task<ActionResult> Index()
        {
            var roles = RoleManager.Roles.Select(o => new GetRolesViewModel { Id = o.Id, Name = o.Name, Description = o.Description }).OrderBy(o => o.Name).ToList();
            string[] userRoles = new string[roles.Count];
            var users = UserManager.Users.ToList().Select(o => new GetUsersViewModel
            {
                Id = o.Id,
                Email = o.Email,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Roles = string.Join(", ", UserManager.GetRoles(o.Id).OrderBy(n => n).ToArray()),
                Tenants = string.Join(", ", _ContentsController.GetTenants(o.Id).OrderBy(n => n.Name).Select(t => t.Name).ToArray())
            }).Where(o => o.Id == ApplicationUser.Identity.GetUserId()).ToList();
            foreach (var item in users)
            {
                userRoles = item.Roles.Trim().Split(',');
            }

            List<ChannelIndexViewModel> vms = new List<ChannelIndexViewModel>();
            if (ApplicationUser.IsInRole("Host"))
            {
                vms = await db.Channels
                    .Join(db.Tenants, c => c.TenantId, t => t.Id, (c, t) => new ChannelIndexViewModel
                    {
                        Channel = c,
                        Tenant = t
                    }).ToListAsync();
            }
            else
            {
                int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
                vms = await db.Channels.Where(o => tenantIds.Contains(o.TenantId))
                    .Join(db.Tenants, c => c.TenantId, t => t.Id, (c, t) => new ChannelIndexViewModel
                    {
                        Channel = c,
                        Tenant = t
                    }).ToListAsync();
            }
            ViewBag.UserRoles = userRoles;
            return View(vms);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(channel);
        }


        public ActionResult Create()
        {
            var tenenats = ListItemHelper.GetSelectTenants(db, ApplicationUser);
            var model = new ChannelPM { TenantId = int.Parse(tenenats.FirstOrDefault().Value) };
            ViewBag.SelectTenants = tenenats;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ChannelPM channel)
        {
            if (ModelState.IsValid)
            {
                int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
                if (!tenantIds.Any(o => o == channel.TenantId))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                var tentant = db.Tenants.Where(o => o.Id == channel.TenantId).FirstOrDefault();

                if (Convert.ToBoolean(tentant.ChennalModeration))
                {
                    channel.Flag = true;
                }
                else
                {
                    channel.Flag = false;
                }
                db.Channels.Add(channel.GetChannel());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(channel);
        }

        public async Task<ActionResult> Contents(int id)
        {
            var userId = UserId;
            var channel = ChannelBiz.ReadChannel(id);
            var contents = ContentBiz.ReadContents(channel.TenantId);
            var selectedContents = ContentBiz.ReadChannelContents(channel.Id);
            var availableContents = contents.Where(c => !selectedContents.Any(x => x.ContentId == c.Id));

            contents.ForEach(c =>
            {
                c.Url = ContentBiz.ComputeContentUrl(c.Source);
                c.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(c.Source);
            });
            selectedContents.ForEach(c =>
            {
                c.Url = ContentBiz.ComputeContentUrl(c.Source);
                c.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(c.Source);
            });

            return View(new ChannelContentViewModel
            {
                DefaultDelaySeconds = 10,
                Channel = channel,
                SelectedContents = selectedContents,
                AvailableContents = availableContents,
                ContentTypes = await db.ContentTypes.ToListAsync()
            });
        }

        public async Task<ActionResult> Edit(int id)
        {
            Channel channel = await db.Channels.Include(ch => ch.Tenant).SingleAsync(ch => ch.Id == id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            return View(channel.GetChannelPM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ChannelPM channel)
        {
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            var tentant = db.Tenants.Where(o => o.Id == channel.TenantId).FirstOrDefault();

            if (Convert.ToBoolean(tentant.ChennalModeration) == true)
            {
                channel.Flag = true;
            }
            else if (Convert.ToBoolean(tentant.ChennalModeration) == false)
            {
                channel.Flag = false;
            }
            db.Entry(channel.GetChannel()).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(channel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Channel channel = await db.Channels.FindAsync(id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Channels.Remove(channel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = await db.Channels.FindAsync(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(channel);
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApproveConfirmed(int id)
        {
            Channel channel = await db.Channels.FindAsync(id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == channel.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Entry(channel).State = EntityState.Modified;
            channel.Flag = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Select(int contentId, int channelId, int? delay)
        {
            await ChannelBiz.AddContentToChannelAsync(channelId, contentId, delay);
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Deselect(int contentId, int channelId)
        {
            var channelContent = await db.ChannelContents.SingleOrDefaultAsync(o => o.ChannelId == channelId && o.ContentId == contentId);
            if (channelContent != null)
            {
                db.ChannelContents.Remove(channelContent);
                await db.SaveChangesAsync();
            }
            return Json(true);
        }

        [HttpPost]
        public async Task<ActionResult> Delay(int contentId, int channelId, int delay)
        {
            ChannelContent channelContent = await db.ChannelContents.FirstOrDefaultAsync(o => o.ChannelId == channelId && o.ContentId == contentId);
            if (channelContent != null)
            {
                channelContent.DelaySeconds = delay;
                await db.SaveChangesAsync();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

        }

        [HttpPost]
        public async Task<ActionResult> Swap(int channelId, int firstContentId, int secondContentId)
        {
            await ChannelBiz.SwapChannelContentsSequence(channelId, firstContentId, secondContentId);
            return Json(true);
        }
    }
}
