using System;
using com.xiaoman;
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

        public static Adv CreateAdvWithoutId(OrcaAdv adv)
        {
            return new Adv()
            {
                AdvCreationTime = adv.creation_time,
                AdvExpirationTime = adv.expiration_time,
                AdvWpaper1 = adv.wall_paper_4_to_3,
                AdvWpaper1Len = adv.wall_paper_4_to_3.Length,
                AdvWpaper2 = adv.wall_paper_16_to_9,
                AdvWpaper2Len = adv.wall_paper_16_to_9.Length,
                AdvDivPaper = adv.div_paper,
                AdvDivPaperLen = adv.div_paper.Length
            };
        }

        public static byte[] StringToBytes(string str)
        {
            return Convert.FromBase64String(str);
        }

        public static string BytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
