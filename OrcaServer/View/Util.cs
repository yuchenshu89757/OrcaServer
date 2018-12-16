using System;
using Com.Xiaoman;
using OrcaServer.Model.Entity;

namespace OrcaServer.View
{
    public static class Util
    {
        public static DateTime ToDatetime(uint timestamp)
        {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1).AddSeconds(timestamp));
        }

        public static uint ToTimestamp(DateTime dateTime)
        {
            return (uint)((dateTime.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000);
        }
    }
}
