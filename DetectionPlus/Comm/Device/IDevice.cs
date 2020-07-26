using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    public interface IDevice
    {
        bool Connected { get; }
        string Host { get; }
        event Action ConnectEvent;

        void Open();
        void Close();

        ReadReponseMessage Send(ReadMessage msg);
        WriteReponseMessage Send(WriteMessage msg);
    }
}
