using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Teleboard.Models.PresentationModel.Model.TimeBox;
using Teleboard.UI.Models.Device;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host,TenantAdmin")]
    public class TimeBoxesController : BaseController
    {
        public ActionResult Index(string id)
        {
            var deviceIds = id.Split(',').Select(x => Int32.Parse(x)).ToArray();
            var groupScheduling = deviceIds.Count() > 1;

            return View(new DeviceScheduleViewModel()
            {
                Device = null, // DeviceBiz.FindDevice(deviceIds.First()),
                Devices = DeviceBiz.FindDevices(deviceIds),
                Channels = ChannelBiz.ReadChannelsForDeviceSchedule(deviceIds.First()),
                IsGroupScheduling = groupScheduling
            });
        }

        public JsonResult ReadDeviceTimeBoxes(int deviceId)
        {
            return Json(DeviceBiz.ReadDeviceTimeBoxes(deviceId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveEvents(IEnumerable<int> deviceIds, List<EventPM> events)
        {
            string saveTime = "";
            events = events ?? new List<EventPM>();
            if (deviceIds.Count() == 1)
            {
                DeviceBiz.ScheduleDevice(deviceIds.Single(), events, out saveTime);
            }
            else
            {
                DeviceBiz.ScheduleDevices(deviceIds, events);
                saveTime = $"{DateTime.UtcNow.ToShortTimeString()} (UTC)";
            }

            return Json(new { Time = saveTime }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RemoveAllSchedulings(int deviceId)
        {
            DeviceBiz.RemoveAllSchedulings(deviceId, ApplicationUser);
            return Json(true);
        }
    }
}
