using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    public class COMClient : IDevice
    {
        private string host;
        public event Action ConnectEvent;

        public COMClient(string host)
        {
            this.host = host;
        }

        #region 公开接口
        public bool Connected => throw new NotImplementedException();
        public void Open()
        {
            throw new NotImplementedException();
        }
        public void Close()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 收发消息
        public ReadReponseMessage Send(ReadMessage msg)
        {
            throw new NotImplementedException();
        }
        public WriteReponseMessage Send(WriteMessage msg)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
