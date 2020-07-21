using Paway.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DetectionPlus
{
    public class TCPClient : DeviceBase, IDevice
    {
        private volatile bool IAlone;
        private volatile bool connected;

        private TcpClient client;
        private readonly string host;
        private readonly int port;
        public event Action ConnectEvent;

        public TCPClient(string host, int port)
        {
            Init();
            this.host = host;
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
        public bool Connected
        {
            get { return connected; }
            set
            {
                if (connected != value)
                {
                    connected = value;
                    try
                    {
                        ConnectEvent?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }
                }
            }
        }
        public void Open()
        {
            if (IStop) return;

            Task.Run(() =>
            {
                if (IAlone) return;
                try
                {
                    IAlone = true;
                    lock (objLock)
                    {
                        client.Connect(host, port);
                        Connected = client.Connected;
                    }
                }
                catch (ObjectDisposedException)
                {
                    ReConnect(new SocketException((int)SocketError.IsConnected));
                }
                catch (SocketException e)
                {
                    ReConnect(e);
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
                finally
                {
                    IAlone = false;
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
            IStop = true;
            lock (objLock)
            {
                client?.Close();
            }
        }

        #endregion

        #region 收发消息
        public ReadReponseMessage Send(ReadMessage msg)
        {
            try
            {
                lock (objLock)
                {
                    if (client.Client == null || client.Connected != Connected) ReConnect(new SocketException((int)SocketError.IsConnected));
                    if (client.Client == null || !client.Connected || !Connected) throw new WarningException("未连接");
                    string ip = client.Client.RemoteEndPoint.ToString();
                    var stream = client.GetStream();
                    Send(ip, stream, msg);

                    byte[] heardBuffer = Recevid(stream, Config.HeardSize);

                    byte[] dataBuffer = Recevid(stream, 2);
                    var length = BitConverter.ToInt16(dataBuffer.Reverse().ToArray(), 0);

                    dataBuffer = Recevid(stream, length);
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
                    if (client.Client == null || client.Connected != Connected) ReConnect(new SocketException((int)SocketError.IsConnected));
                    if (client.Client == null || !client.Connected || !Connected) throw new WarningException("未连接");
                    string ip = client.Client.RemoteEndPoint.ToString();
                    var stream = client.GetStream();
                    Send(ip, stream, msg);

                    byte[] heardBuffer = Recevid(stream, Config.HeardSize);

                    var dataBuffer = Recevid(stream, 4 + (msg as WriteMessage).Count * 2);
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

        private byte[] Recevid(NetworkStream stream, int length)
        {
            return base.Recevid(length, stream.Read);
        }
        private void Send(string ip, NetworkStream stream, MessageBase msg)
        {
            base.Send(ip, msg, stream.Write);
        }

        #endregion
    }
    public class DeviceBase
    {
        protected volatile bool IStop;
        protected readonly object objLock = new object();

        protected void RecevidLog(string ip, byte[] buffer)
        {
            DeviceLog.Log($">>>>>>>{ip}>接收数据：{BitConverter.ToString(buffer)}");
        }
        protected byte[] Recevid(int length, Func<byte[], int, int, int> action)
        {
            var buffer = new byte[length];
            length = 0;
            while (true)
            {
                length += action(buffer, length, buffer.Length - length);// 接收包头，防止粘包
                if (length == buffer.Length) break;
            }
            return buffer;
        }
        protected void Send(string ip, MessageBase msg, Action<byte[], int, int> action)
        {
            byte[] buffer = msg.Buffer();
            action(buffer, 0, buffer.Length);
            DeviceLog.Log($"<<<<<<<{ip}<发送数据：{BitConverter.ToString(buffer)}");
        }
    }
}
