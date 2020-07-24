using Paway.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DetectionPlus
{
    public class COMClient : DeviceBase, IDevice
    {

        private SerialPort client;
        private readonly string host;

        public COMClient(string host)
        {
            Init();
            this.host = host;
            Open();
        }
        private void Init()
        {
            if (IStop) return;
            client = new SerialPort();
            if (this.host.IsEmpty()) return;
            client.PortName = this.host;
            client.WriteTimeout = 3 * 1000;
            client.ReadTimeout = 3 * 1000;

            client.BaudRate = 9600;
            client.Parity = Parity.None;
            client.StopBits = StopBits.One;
            client.DataBits = 8;
            client.Handshake = Handshake.None;
        }

        #region 公开接口
        public override void Open()
        {
            Open(() =>
            {
                client.Open();
                Connected = client.IsOpen;
            }, ex =>
            {
                ReConnect();
                ex.Log();
            });
        }
        private void ReConnect()
        {
            if (IStop) return;

            Connected = false;
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                lock (objLock)
                {
                    client.Close();
                    Init();
                }
                Open();
            });
        }
        public void Close()
        {
            Close(() => { client?.Close(); });
        }

        #endregion

        #region 收发消息
        public ReadReponseMessage Send(ReadMessage msg)
        {
            throw new NotImplementedException();
        }
        public WriteReponseMessage Send(WriteMessage msg)
        {
            try
            {
                lock (objLock)
                {
                    if (client == null || client.IsOpen != Connected) ReConnect();
                    if (client == null || !client.IsOpen || !Connected) return new WriteReponseMessage() { List = new List<short> { 0 } };
                    Send(client.PortName, msg, client.Write);

                    byte[] heardBuffer = Recevid(Config.HeardSize, client.Read);

                    var dataBuffer = Recevid(4 + (msg as WriteMessage).Count * 2, client.Read);
                    RecevidLog(client.PortName, heardBuffer.Concat(dataBuffer).ToArray());

                    var data = BitConverter.ToString(dataBuffer);
                    var writeMsg = new WriteReponseMessage();
                    writeMsg.Parse(data);
                    return writeMsg;
                }
            }
            catch (Exception)
            {
                ReConnect();
                throw;
            }
        }

        #endregion
    }
}
