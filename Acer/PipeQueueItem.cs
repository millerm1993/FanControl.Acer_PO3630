namespace FanControl.Acer_PO3630.Acer
{
    internal class PipeQueueItem
    {
        public long Result { get; set; } = -1;
        public byte Identifier { get; set; }
        public byte[] DataBytes { get; set; }
    }
}
