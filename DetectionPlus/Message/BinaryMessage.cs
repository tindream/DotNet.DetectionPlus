using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionPlus.Message
{
    public class BinaryMessage
    {
        public double Value { get; set; }

        public BinaryMessage(double value)
        {
            this.Value = value;
        }
    }
}
