﻿using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    public class WriteMessage : MessageBase
    {
        /// <summary>
        /// 寄存器起始地址(高位、低位)
        /// </summary>
        public short Start { get; set; }
        /// <summary>
        /// 寄存器值列表(高位、低位)
        /// </summary>
        public List<short> List { get; set; } = new List<short>();

        public WriteMessage() : base(Config.Write) { }
        public WriteMessage(int start) : this()
        {
            this.Start = (short)start;
        }
    }
    public class WriteReponseMessage : MessageBase
    {
        /// <summary>
        /// 寄存器起始地址(高位、低位)
        /// </summary>
        public short Start { get; set; }
        /// <summary>
        /// 寄存器值列表(高位、低位)
        /// </summary>
        public List<short> List { get; set; } = new List<short>();

        public WriteReponseMessage() : base(Config.Write) { }
    }
}
