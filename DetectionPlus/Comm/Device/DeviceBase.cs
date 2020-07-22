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
    public class DeviceBase
    {
        protected volatile bool IStop;
        private volatile bool IAlone;
        protected readonly object objLock = new object();

        private volatile bool connected;
        public event Action ConnectEvent;

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
        public virtual void Open() { }
        public void Open(Action action, Action<Exception> error)
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
                        action();
                    }
                }
                catch (Exception ex)
                {
                    error(ex);
                }
                finally
                {
                    IAlone = false;
                }
            });
        }
        protected void Close(Action action)
        {
            IStop = true;
            lock (objLock)
            {
                action();
            }
        }

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
