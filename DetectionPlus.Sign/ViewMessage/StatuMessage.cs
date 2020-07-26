using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Sign
{
    public class TestMessage : InitMessage
    {
        public string File { get; set; }

        public TestMessage(string file)
        {
            this.File = file;
        }
    }

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

    public class InitMessage
    {
        public DependencyObject Obj { get; set; }
    }
    public class HistroyInitMessage : InitMessage { }
    public class CommInitMessage : InitMessage { }
    public class PictureMessage : InitMessage { }
    public class ResetMessage : InitMessage { }
    public class OpenMessage : InitMessage { }
    public class SaveMessage : InitMessage { }
    public class ModelMessage : InitMessage { }
    public class RingMessage : InitMessage { }
}
