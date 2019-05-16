using System;
using System.Collections.Generic;
using Teleboard.PresentationModel.Model.Device;
using Teleboard.PresentationModel.Model.Tenant;

namespace Teleboard.UI.Models.Device
{
    public class EditDeviceViewModel
    {
        public DevicePM Device { get; set; }
        public IEnumerable<TimeZoneInfo> TimeZones { get; set; }

    }
}
