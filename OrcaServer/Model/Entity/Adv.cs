namespace OrcaServer.Model.Entity
{
    public class Adv
    {
        public ulong AdvId { get; set; }
        public uint AdvCreationTime { get; set; }
        public uint AdvExpirationTime { get; set; }
        public byte[] AdvDivPaper { get; set; }
        public int AdvDivPaperLen { get; set; }
        public byte[] AdvWpaper1 { get; set; }
        public int AdvWpaper1Len { get; set; }
        public byte[] AdvWpaper2 { get; set; }
        public int AdvWpaper2Len { get; set; }
    }
}
