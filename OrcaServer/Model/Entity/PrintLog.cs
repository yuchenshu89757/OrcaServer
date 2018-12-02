namespace OrcaServer.Model.Entity
{
    public class PrintLog
    {
        public PrintLog()
        {
        }
        public PrintLog(
                ulong printLogId, 
                uint printLogDispTime, 
                string printLogPcName, 
                string printLogMacAddr,
                string printLogIpAddr, 
                ulong printLogAdvId)
        {
            PrintLogId = printLogId;
            PrintLogDispTime = printLogDispTime;
            PrintLogPcName = printLogPcName;
            PrintLogMacAddr = printLogMacAddr;
            PrintLogIpAddr = printLogIpAddr;
            PrintLogAdvId = printLogAdvId;
        }

        public ulong PrintLogId { get; set; }
        public uint PrintLogDispTime { get; set; }
        public string PrintLogPcName { get; set; }
        public string PrintLogMacAddr { get; set; }
        public string PrintLogIpAddr { get; set; }
        public ulong PrintLogAdvId { get; set; }
    }
}
