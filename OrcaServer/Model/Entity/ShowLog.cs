namespace OrcaServer.Model.Entity
{
    public class ShowLog
    {
        public ulong ShowLogId { get; set; }
        public uint ShowLogDispTime { get; set; }
        public string ShowLogMacAddr { get; set; }
        public ulong ShowLogAdvId { get; set; }
        public uint ShowLogPictureLen { get; set; }
        public byte[] ShowLogPicture { get; set; }
    }
}
