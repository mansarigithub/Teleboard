using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using Teleboard.Common.ExtensionMethod;
using Teleboard.DataAccess.Context;
using Teleboard.PresentationModel.Model.Content;

namespace Teleboard.Controllers
{
    [AllowAnonymous]
    public class ApiController : BaseController
    {
        private ApplicationDbContext db;

        public ApiController()
        {
            db = AppDbContext;
        }

        public ActionResult Schedule(string deviceid)
        {
            var basePath = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/') + "/api/resource/";

            var subscriptionKey = Request.Headers["subscription-key"]?.ToLower();
            IEnumerable<ScheduledContentPM> contents;
            try
            {
                contents = DeviceBiz.ReadDeviceScheduledContents(deviceid, subscriptionKey);
                contents.ForEach(c =>
                {
                    c.Url = basePath + c.ContentTenantId.ToString() + "?name=" + c.ContentSource;
                    //c.ThumbnailUrl = ContentBiz.ComputeContentThumbnailUrl(basePath, c.ContentSource);
                });
            }
            catch
            {
                contents = null;
            }
            return Json(contents, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Resource(int id, string name)
        {
            ContentPM content;
            bool authorized = true;

            try
            {
                content = ContentBiz.ReadContent(name);
            }
            catch
            {
                return HttpNotFound();
            }


            //if (User.Identity.IsAuthenticated) {
            //    if (User.IsInRole("Host")) {
            //        authorized = true;
            //    }
            //    else {
            //        int[] tenantIds = db.GetTenantsFromUser(User.Identity.GetUserId(), User.IsInRole("Host"));
            //        if (tenantIds.Any(o => o == id)) {
            //            authorized = true;
            //        }
            //    }
            //}
            //else {
            //    authorized = Request.Headers["subscription-key"] != null && Request.Headers["subscription-key"].ToLower() == tenantConent.Tenant.SubscriptionKey.ToString().ToLower();
            //}

            if (authorized)
            {
                return File(ContentBiz.ComputeContentFilePath(content.TenantId, content.Source), content.ContentTypeName);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }
    }
}
