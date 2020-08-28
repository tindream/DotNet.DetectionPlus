using Paway.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DetectionPlus
{
    public class TCPClient : DeviceBase, IDevice
    {
        private TcpClient client;
        public string Host { get; private set; }
        private readonly int port;

        public TCPClient(string host, int port)
        {
            Init();
            this.Host = host;
            this.port = port;
            Open();
        }
        private void Init()
        {
            if (IStop) return;
            client = new TcpClient();
            client.SendTimeout = 3 * 1000;
            client.ReceiveTimeout = 3 * 1000;
        }

        #region 公开接口
        public override void Open()
        {
            Open(() =>
            {
                client.Connect(Host, port);
                Connected = client.Connected;
            }, ex =>
            {
                if (ex is ObjectDisposedException)
                {
                    ReConnect(new SocketException((int)SocketError.IsConnected));
                }
                else if (ex is SocketException e)
                {
                    ReConnect(e);
                }
                else
                {
                    ex.Log();
                }
            });
        }
        private void ReConnect(SocketException e)
        {
            if (IStop) return;

            Connected = false;
            Task.Run(() =>
            {
                switch (e.SocketErrorCode)
                {
                    case SocketError.HostUnreachable:
                    case SocketError.ConnectionAborted:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionRefused:
                        Thread.Sleep(5000);
                        break;
                    case SocketError.TimedOut:
                    case SocketError.IsConnected:
                        lock (objLock)
                        {
                            if (client.Client != null) client.Close();
                            Init();
                        }
                        Thread.Sleep(1000);
                        break;
                    default:
                        Thread.Sleep(1000);
                        break;
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
            try
            {
                lock (objLock)
                {
                    SendMessage(msg);

                    string ip = client.Client.RemoteEndPoint.ToString();
                    var stream = client.GetStream();
                    byte[] heardBuffer = Recevid(Config.HeardSize, stream.Read);

                    byte[] dataBuffer = Recevid(2, stream.Read);
                    var length = BitConverter.ToInt16(dataBuffer.Reverse().ToArray(), 0);

                    dataBuffer = Recevid(length, stream.Read);
                    RecevidLog(ip, heardBuffer.Concat(dataBuffer).ToArray());

                    var data = BitConverter.ToString(dataBuffer);
                    var readMsg = new ReadReponseMessage();
                    readMsg.Parse(data);
                    return readMsg;
                }
            }
            catch (SocketException e)
            {
                ReConnect(e);
                throw;
            }
            catch (Exception e)
            {
                if (e.InnerException() is SocketException ex)
                {
                    ReConnect(ex);
                }
                throw;
            }
        }
        public WriteReponseMessage Send(WriteMessage msg)
        {
            try
            {
                lock (objLock)
                {
                    SendMessage(msg);

                    string ip = client.Client.RemoteEndPoint.ToString();
                    var stream = client.GetStream();
                    byte[] heardBuffer = Recevid(Config.HeardSize, stream.Read);

                    var dataBuffer = Recevid(4, stream.Read);
                    RecevidLog(ip, heardBuffer.Concat(dataBuffer).ToArray());

                    var data = BitConverter.ToString(dataBuffer);
                    var writeMsg = new WriteReponseMessage();
                    writeMsg.Parse(data);
                    return writeMsg;
                }
            }
            catch (SocketException e)
            {
                ReConnect(e);
                throw;
            }
            catch (Exception e)
            {
                if (e.InnerException() is SocketException ex)
                {
                    ReConnect(ex);
                }
                throw;
            }
        }
        private void SendMessage(MessageBase msg)
        {
            if (client.Client == null || client.Connected != Connected) ReConnect(new SocketException((int)SocketError.IsConnected));
            if (client.Client == null || !client.Connected || !Connected) throw new WarningException("未连接");
            string ip = client.Client.RemoteEndPoint.ToString();
            var stream = client.GetStream();
            Send(ip, msg, stream.Write);
        }

        #endregion
    }
}
