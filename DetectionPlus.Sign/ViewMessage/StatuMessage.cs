using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Sign
{
    public class InitMessage
    {
        public DependencyObject Obj { get; set; }
    }
    public class HistroyInitMessage : InitMessage { }

    public class HistroyMessage
    {
        public HistroyInfo Info { get; set; }

        public HistroyMessage(HistroyInfo info)
        {
            this.Info = info;
        }
    }

    public class StatuMessage
    {
        public string Message { get; set; }

        public StatuMessage(string msg)
        {
            this.Message = msg;
        }
    }
}
