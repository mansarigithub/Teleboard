using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class Device
    {
        public Device()
        {
            this.TimeBoxes = new List<TimeBox>();
        }

        public int Id { get; set; }

        public int TenantId { get; set; }

        public string Name { get; set; }

        //[Index(IsUnique =true)]
        public string DeviceId { get; set; }

        [LocalizedName("Description")]
        public string Description { get; set; }

        public int? Latitude { get; set; }

        public int? Longitude { get; set; }

        public string Version { get; set; }

        public int? ConnectionTypeId { get; set; }

        public DateTime? LastConnectedUtc { get; set; }

        public DateTime RegisteredOnUtc { get; set; }
        public bool PlayAdvertisement { get; set; }

        public string TimeZoneId { get; set; }

        [NotMapped]
        public DateTime? LastConnectedLocal
        {
            get
            {
                if (!LastConnectedUtc.HasValue) return null;
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(LastConnectedUtc.Value, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
            }
        }

        [NotMapped]
        public DateTime RegisteredOnLocal
        {
            get
            {
                return  TimeZoneInfo.ConvertTimeFromUtc(RegisteredOnUtc, TimeZoneInfo.FindSystemTimeZoneById(this.TimeZoneId));
            }
        }

        public virtual Tenant Tenant { get; set; }

        [ScriptIgnore]
        public virtual ICollection<TimeBox> TimeBoxes { get; set; }

        [ScriptIgnore]
        public virtual ConnectionType ConnectionType { get; set; }

    }
}