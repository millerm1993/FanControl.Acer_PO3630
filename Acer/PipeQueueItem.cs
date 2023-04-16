using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.Acer_PO3630.Acer
{
    internal class PipeQueueItem
    {
        public long Result { get; set; } = -1;
        public byte Identifier { get; set; }
        public byte[] DataBytes { get; set; }
    }
}
