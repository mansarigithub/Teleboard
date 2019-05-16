using System.Collections.Generic;
using Teleboard.PresentationModel.Model.Channel;
using Teleboard.PresentationModel.Model.Content;
using Teleboard.PresentationModel.Model.Device;

namespace Teleboard.UI.Models.Device
{
    public class AdsScheduleViewModel
    {
        public DevicePM Device { get; set; }
        public IEnumerable<ContentPM> Contents { get; set; }
    }
}