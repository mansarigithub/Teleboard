using System;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Device
{
    public class DeviceDetailsForDeviceListPagePM
    {
        public int Id { get; set; }

        [LocalizedName("Name")]
        public string Name { get; set; }

        [LocalizedName("Description")]
        public string Description { get; set; }

        [LocalizedName("DeviceId")]
        public string DeviceId { get; set; }

        [LocalizedName("TenantName")]
        public string TenantName { get; set; }

        public int TenantId { get; set; }

        public string CurrentPlayingChannelName { get; set; }
        public int? CurrentPlayingChannelId { get; set; }

        [LocalizedName("LastConnectivity")]
        public DateTime? LastConnectedUtc { get; set; }

        [LocalizedName]
        public string TimeZoneId { get; set; }

        public DateTime? LastConnectedLocal { get; set; }

        public string LastConnectedUtcString
        {
            get
            {
                return LastConnectedUtc?.ToString();
            }
        }

        public string LastConnectedLocalString
        {
            get
            {
                return LastConnectedLocal?.ToString();
            }
        }

        public string LastConnectedString
        {
            get
            {
                return LastConnectedUtc.HasValue ?
                    string.Format("<br/>{0} (Device Local Time)<br/>{1} (UTC)", LastConnectedLocalString, LastConnectedUtcString) : "";
            }
        }

        public bool IsOnline
        {
            get
            {
                if (!LastConnectedUtc.HasValue) return false;
                var totalSeconds = (DateTime.UtcNow - LastConnectedUtc.Value).TotalSeconds;
                return (totalSeconds >= 0 && totalSeconds <= 30);
            }
        }
    }
}
