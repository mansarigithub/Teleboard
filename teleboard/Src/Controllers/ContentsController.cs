using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Teleboard.Common.Configuration;
using Teleboard.Common.Data;
using Teleboard.Common.Enum;
using Teleboard.Common.ExtensionMethod;
using Teleboard.Common.IO;
using Teleboard.Common.Media;
using Teleboard.DataAccess.Context;
using Teleboard.DomainModel.Core;
using Teleboard.Helper;
using Teleboard.Localization;
using Teleboard.Models;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.Validation.Attributes;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host,TenantAdmin,ContentCreator,Advertiser")]
    public class ContentsController : BaseController
    {


        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ReadContents(DataSourceRequest request, string id)
        {
            request.SearchValue = id?.ToLower();
            var resourceTypes = new List<ResourceType>();
            if (HttpContext.Request.Headers["include-images"].ToLower() == "true")
                resourceTypes.Add(ResourceType.Image);
            if (HttpContext.Request.Headers["include-videos"].ToLower() == "true")
                resourceTypes.Add(ResourceType.Video);
            request.IncludedResourceTypes = resourceTypes;
            DataSourceResult contents = ReadPrivilagedContents(request);

            ((IEnumerable<ContentPM>)contents.records).ForEach(c =>
            {
                c.Url = ContentBiz.ComputeContentUrl(c.Source);
                c.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(c.Source);
            });
            return Json(contents, JsonRequestBehavior.AllowGet);
        }

        private DataSourceResult ReadPrivilagedContents(DataSourceRequest request)
        {
            if (ApplicationUser.IsInRole(AppRole.Host))
                return ContentBiz.ReadContents(request);
            else if (ApplicationUser.IsInRole(AppRole.TenantAdmin) || ApplicationUser.IsInRole(AppRole.ContentCreator))
                return ContentBiz.ReadTenantContents(ApplicationUser, request);
            else if (ApplicationUser.IsInRole(AppRole.Advertiser))
                return ContentBiz.ReadUserContents(ApplicationUser, request);
            else
                throw new InvalidOperationException();
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = await db.Contents.FindAsync(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == content.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(content);
        }

        public ActionResult Create()
        {
            var tenenats = ListItemHelper.GetSelectTenants(db, ApplicationUser);
            if (tenenats.Any())
            {
                tenenats.First().Selected = true;
            }
            var model = new ContentUploadViewModel
            {
                SupportedContentTypes = ContentTypeBiz.ReadContentTypes(),
            };
            ViewBag.SelectTenants = tenenats;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<ActionResult> Create(ContentUploadViewModel model)
        {
            var mimeType = FileHelper.GetFileMimeTypeFromData(model.FileStream.InputStream);
            if (!ContentTypeBiz.Exist(mimeType))
            {
                ModelState.AddModelError("", Resources.ContentTypeNotDefined);
                var tenenats = ListItemHelper.GetSelectTenants(db, ApplicationUser);
                var viewModel = new ContentUploadViewModel { TenantId = int.Parse(tenenats.FirstOrDefault().Value) };
                ViewBag.SelectTenants = tenenats;
                return View(viewModel);
            }
            await ContentBiz.StoreContentAsync(model.TenantId,
                model.FileStream.InputStream,
                model.FileStream.ContentType,
                model.FileStream.FileName,
                model.Description, 
                ApplicationUser);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            var content = ContentBiz.ReadContent(id);
            if (!tenantIds.Any(o => o == content.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            content.Url = ContentBiz.ComputeContentUrl(content.Source);
            content.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(content.Source);
            return View(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ContentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
                if (!tenantIds.Any(o => o == model.TenantId))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                var content = await db.Contents.FirstAsync(o => o.Id == model.Id);
                var tentant = db.Tenants.Where(o => o.Id == model.TenantId).FirstOrDefault();
                if (Convert.ToBoolean(tentant.ContentModeration))
                {
                    if (content.Description != model.Description)
                        content.Flag = false;
                }
                else
                {
                    content.Flag = true;
                }
                content.Description = model.Description;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteContent(int id)
        {
            if (ApplicationUser.IsHostAdmin)
                await ContentBiz.DeleteContentAsync(id);
            else
                await ContentBiz.DeleteContentAsync(id, ApplicationUser);
            return Json(true);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Content content = await db.Contents.FindAsync(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == content.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Content content = await db.Contents.FindAsync(id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == content.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Contents.Remove(content);
            await db.SaveChangesAsync();

            //Delete files
            try
            {
                var fileName = Path.GetFileName(content.Source);
                //var directory = Path.Combine(StorageRoot, content.TenantId.ToString());
                //var path = Path.Combine(directory, fileName);
                //if (Directory.Exists(directory) && System.IO.File.Exists(path))
                //{
                //    System.IO.File.Delete(path);
                //    System.IO.File.Delete(ImageResizer.RenameImageFileWithPostFix(path, "__tiny"));
                //    System.IO.File.Delete(ImageResizer.RenameImageFileWithPostFix(path, "__thumbnail"));
                //    System.IO.File.Delete(ImageResizer.RenameImageFileWithPostFix(path, "__optimized"));
                //}
            }
            catch { }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Host,TenantAdmin")]
        public ActionResult Approve(int id)
        {
            var content = ContentBiz.ReadContentForApproval(id, ApplicationUser);
            content.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(content.Source);
            return View(content);
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ApproveConfirmed(int id)
        {
            Content content = await db.Contents.FindAsync(id);
            int[] tenantIds = db.GetTenantsFromUser(ApplicationUser.Identity.GetUserId(), ApplicationUser.IsInRole("Host"));
            if (!tenantIds.Any(o => o == content.TenantId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Entry(content).State = EntityState.Modified;
            content.Flag = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IEnumerable<Tenant> GetTenants(string userId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                return ctx.TenantUsers
                    .Where(o => o.UserId == userId)
                    .Join(ctx.Tenants, tu => tu.TenantId, t => t.Id, (tu, t) => t)
                    .ToList();
            }
        }
    }
}
