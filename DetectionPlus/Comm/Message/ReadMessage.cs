using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    public class ReadMessage : MessageBase
    {
        /// <summary>
        /// 寄存器起始地址(高位、低位)
        /// </summary>
        public short Start { get; set; }
        /// <summary>
        /// 寄存器数量(高位、低位)
        /// </summary>
        public short Count { get; set; }

        public ReadMessage() : base(Config.Read) { }
        public ReadMessage(int start, int count = 1) : this()
        {
            this.Start = (short)start;
            this.Count = (short)count;
        }
    }
    public class ReadReponseMessage : MessageBase
    {
        /// <summary>
        /// 寄存器值列表(高位、低位)
        /// </summary>
        public List<short> List { get; set; } = new List<short>();

        public ReadReponseMessage() : base(Config.Read) { }
    }
}
