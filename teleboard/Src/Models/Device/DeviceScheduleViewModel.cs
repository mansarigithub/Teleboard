using System.Collections.Generic;
using Teleboard.PresentationModel.Model.Channel;
using Teleboard.PresentationModel.Model.Device;

namespace Teleboard.UI.Models.Device
{
    public class DeviceScheduleViewModel
    {
        public DevicePM Device { get; set; }
        public IEnumerable<DevicePM> Devices { get; set; }
        public IEnumerable<ChannelPM> Channels { get; set; }
        public bool IsGroupScheduling { get; set; }
    }
}