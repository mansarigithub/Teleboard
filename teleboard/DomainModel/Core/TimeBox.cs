using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Script.Serialization;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.DomainModel.Core
{
    public class TimeBox
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int ChannelId { get; set; }

        [LocalizedName("Week Day")]
        public string WeekDay { get; set; }

        [LocalizedName("From Hour")]
        [RequiredField]
        public int FromHour { get; set; }

        [LocalizedName("From Minute")]
        [RequiredField]
        public int FromMinute { get; set; }

        [LocalizedName("To Hour")]
        [RequiredField]
        public int ToHour { get; set; }

        [LocalizedName("To Minute")]
        [RequiredField]
        public int ToMinute { get; set; }

        public bool? Flag { get; set; }

        public bool PlayAdvertisement { get; set; }

        [ScriptIgnore]
        public virtual Device Device { get; set; }

        [ScriptIgnore]
        public virtual Channel Channel { get; set; }

        [NotMapped]
        public TimeSpan Duration
        {
            get
            {
                return new TimeSpan(ToHour, ToMinute, 0) - new TimeSpan(FromHour, FromMinute, 0);
            }
        }
    }
}