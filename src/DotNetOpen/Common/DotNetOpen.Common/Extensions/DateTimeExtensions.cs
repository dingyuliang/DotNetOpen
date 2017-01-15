using System;

namespace DotNetOpen.Common
{
    public static class DateTimeExtensions
    {
        static readonly DateTime UnixStartTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

        /// <summary>
        /// Convert Datetime to Unix
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static double ToUnixTime(this DateTime datetime)
        {
            return (TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.Utc) - UnixStartTimeUtc).TotalSeconds;
        }

        /// <summary>
        /// Convert to Windows Time
        /// </summary>
        /// <param name="unixTicks"></param>
        /// <returns></returns>
        public static DateTime ToWindowsUtcTime(this double unixTicks)
        {
            return UnixStartTimeUtc.AddSeconds(unixTicks);
        }
    }
}
