using System;
using Teleboard.Localization.Attribute;
using Teleboard.Validation.Attribute;

namespace Teleboard.PresentationModel.Model.Device
{
    public class TimeBoxForAdvertisementPM
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
        public bool PlayAdvertisement { get; set; }

        public TimeSpan To
        {
            get
            {
                return new TimeSpan(ToHour, ToMinute, 0);
            }
        }

        public TimeSpan From
        {
            get
            {
                return new TimeSpan(FromHour, FromMinute, 0);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return To - From;
            }
        }

        public int UsedTimeSeconds { get; set; }

        public TimeSpan UsedTime
        {
            get
            {
                return new TimeSpan(0, 0, UsedTimeSeconds);
            }
        }

        public TimeSpan FreeTime
        {
            get
            {
                return Duration - UsedTime;
            }
        }

        public int UsedTimePercentage
        {
            get
            {
                return (int)((UsedTime.TotalSeconds / Duration.TotalSeconds) * 100);
            }
        }

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

        public new string ToString
        {
            get
            {
                return To.ToString();
            }
        }

        public string FromString
        {
            get
            {
                return From.ToString();
            }
        }


    }
}
