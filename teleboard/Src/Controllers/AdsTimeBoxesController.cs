using System.Collections.Generic;
using System.Web.Mvc;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.UI.Models.Device;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Advertiser")]
    public class AdsTimeBoxesController : BaseController
    {
        public ActionResult Index(int id)
        {
            return View(new AdsScheduleViewModel()
            {
                Device = DeviceBiz.FindDevice(id),
                Contents = ContentBiz.ReadUserContents(ApplicationUser.Id),
            });
        }

        public JsonResult ReadDeviceTimeBoxes(int deviceId)
        {
            return Json(DeviceBiz.ReadDeviceTimeBoxesForAdvertisements(deviceId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveTimeBox(int timeboxId, IEnumerable<ContentPM> selectedContents)
        {
            string saveTime;
            selectedContents = selectedContents ?? new List<ContentPM>();
            var timeBox = DeviceBiz.UpdateChannelAdvertisements(ApplicationUser, timeboxId, selectedContents, out saveTime);
            return Json(new
            {
                Time = saveTime,
                TimeBox = timeBox,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveAllSchedulings(int deviceId)
        {
            DeviceBiz.RemoveAllSchedulings(deviceId, ApplicationUser);
            return Json(true);
        }

        [HttpGet]
        public JsonResult ReadChannelContents(int channelId)
        {
            return Json(ContentBiz.ReadChannelContentsForAdvertisementPage(channelId), JsonRequestBehavior.AllowGet);
        }

    }
}
