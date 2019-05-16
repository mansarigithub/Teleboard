using System;

namespace Teleboard.Common.ExtensionMethod
{
    public static class DateTimeExtension
    {
        public static DateTime FromUtcToLocal(this DateTime utcDateTime, string timeZoneId)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }
    }
}
