using System;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Device
{
    public class TimeBoxPM
    {
        public int Id { get; set; }

        public int DeviceId { get; set; }

        public int ChannelId { get; set; }

        [LocalizedName("Week Day")]
        public string WeekDay { get; set; }

        [LocalizedName("From Hour")]
        public int FromHour { get; set; }

        [LocalizedName("From Minute")]
        public int FromMinute { get; set; }

        [LocalizedName("To Hour")]
        public int ToHour { get; set; }

        [LocalizedName("To Minute")]
        public int ToMinute { get; set; }

        public bool? Flag { get; set; }

        public string ChannelName { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public bool PlayAdvertisement { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan UsedTime { get; set; }

        public TimeSpan FreeTime  { get; set; }

        public string FreeTimeString
        {
            get
            {
                return FreeTime.ToString();
            }
        }

        public string UsedTimeString
        {
            get
            {
                return UsedTime.ToString();
            }
        }

        public string DurationString
        {
            get
            {
                return Duration.ToString();
            }
        }
    }
}
