using System;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Device
{
    public class DevicePM
    {
        public int Id { get; set; }

        [RequiredField]
        [LocalizedName("Name")]
        [StringLengthRange(1, 50)]
        public string Name { get; set; }

        [LocalizedName("Description")]
        [StringLengthRange(1, 200)]
        public string Description { get; set; }

        [LocalizedName("DeviceId")]
        [RequiredField]
        [StringLengthRange(1, 200)]
        public string DeviceId { get; set; }

        public int TenantId { get; set; }

        [LocalizedName("Tenant")]
        public string TenantName { get; set; }

        [LocalizedName]
        public int? Latitude { get; set; }

        [LocalizedName]
        public int? Longitude { get; set; }

        [StringLengthRange(1, 20)]
        public string Version { get; set; }
        public string CurrentPlayingChannelName { get; set; }
        public int? CurrentPlayingChannelId { get; set; }

        public int? ConnectionTypeId { get; set; }
        public string ConnectionTypeName { get; set; }

        [LocalizedName("LastConnectivity")]
        public DateTime? LastConnectedUtc { get; set; }

        [LocalizedName("RegisterationDate")]
        public DateTime RegisteredOnUtc { get; set; }

        [LocalizedName]
        [RequiredField]
        public string TimeZoneId { get; set; }

        public DateTime? LastConnectedLocal { get; set; }

        public bool IsAdvertisementsEnabled { get; set; }

        [LocalizedName]
        public bool PlayAdvertisement { get; set; }

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
                    string.Format("<br/>{0} (Local Time)<br/>{1} (UTC)", LastConnectedLocalString, LastConnectedUtcString) : "";
            }
        }


        public bool IsOnline
        {
            get
            {
                if (!LastConnectedUtc.HasValue) return false;
                return (DateTime.UtcNow - LastConnectedUtc.Value).TotalSeconds < 30;
            }
        }
    }
}
