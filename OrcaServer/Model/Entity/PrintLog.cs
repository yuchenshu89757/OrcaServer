namespace OrcaServer.Model.Entity
{
    public class PrintLog
    {
        public ulong PrintLogId { get; set; }
        public uint PrintLogDispTime { get; set; }
        public string PrintLogPcName { get; set; }
        public string PrintLogMacAddr { get; set; }
        public string PrintLogIpAddr { get; set; }
        public ulong PrintLogAdvId { get; set; }
    }
}
