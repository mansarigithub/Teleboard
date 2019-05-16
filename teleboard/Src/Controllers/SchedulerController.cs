using System;
using System.Linq;
using System.Web.Mvc;
using Teleboard.Business.Core;
using Teleboard.Common.Data;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.UI.Models.Device;
using Teleboard.Localization;

namespace Teleboard.Controllers
{
    [Authorize(Roles = "Host,TenantAdmin")]
    public class SchedulerController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ReadDevices(DataSourceRequest request, bool? includeDetails)
        {
            if (includeDetails.HasValue && includeDetails.Value)
                return Json(DeviceBiz.ReadDevicesWithDetails(ApplicationUser, request), JsonRequestBehavior.AllowGet);
            else
                return Json(DeviceBiz.ReadDevices(ApplicationUser, request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var tenanats = ApplicationUser.IsHostAdmin ? TenantBiz.ReadAllTenants() : TenantBiz.ReadUserTenants(UserId);
            return View(new CreateDeviceViewModel()
            {
                Device = new DevicePM() { Id = tenanats.First().Id },
                Tenants = tenanats,
                TimeZones = TimeZoneInfo.GetSystemTimeZones(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public ActionResult Create(CreateDeviceViewModel viewModel)
        {
            if (DeviceBiz.DeviceExist(viewModel.Device.DeviceId))
            {
                ModelState.AddModelError("", Resources.DeviceIdentifierIsDuplicate);
                return Create();
            }
            DeviceBiz.CreateDevice(viewModel.Device);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var device = DeviceBiz.FindDevice(id);
            return View(new EditDeviceViewModel()
            {
                Device = device,
                TimeZones = TimeZoneInfo.GetSystemTimeZones(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public ActionResult Edit(EditDeviceViewModel viewModel)
        {
            DeviceBiz.UpdateDevice(viewModel.Device);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            DeviceBiz.DeleteDevice(id);
            return View("Index");
        }

        [HttpGet]
        public ActionResult ReadTenant(int id)
        {
            return Json(TenantBiz.ReadTenant(id), JsonRequestBehavior.AllowGet);
        }
    }
}
