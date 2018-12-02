namespace OrcaServer.Model.Entity
{
    public class ShowLog
    {
        public ShowLog()
        {
        }

        public ShowLog(ulong showLogId, uint showLogDispTime, string showLogMacAddr, ulong showLogAdvId, uint showLogPictureLen, byte[] showLogPicture)
        {
            ShowLogId = showLogId;
            ShowLogDispTime = showLogDispTime;
            ShowLogMacAddr = showLogMacAddr;
            ShowLogAdvId = showLogAdvId;
            ShowLogPictureLen = showLogPictureLen;
            ShowLogPicture = showLogPicture;
        }

        public ulong ShowLogId { get; set; }
        public uint ShowLogDispTime { get; set; }
        public string ShowLogMacAddr { get; set; }
        public ulong ShowLogAdvId { get; set; }
        public uint ShowLogPictureLen { get; set; }
        public byte[] ShowLogPicture { get; set; }
    }
}
