using System.Web.Mvc;
using Teleboard.Business.Core;
using Teleboard.Common.Data;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Advertiser")]
    public class AdsDevicesController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ReadDevices(DataSourceRequest request)
        {
            return Json(DeviceBiz.ReadAdvertisementDevices(ApplicationUser, request), JsonRequestBehavior.AllowGet);
        }
    }
}
