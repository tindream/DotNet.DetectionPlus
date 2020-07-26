using GalaSoft.MvvmLight.Messaging;
using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Sign
{
    public class DeviceManager
    {
        private bool IStop;
        public AdminInfo Info { get; set; }
        public IDevice Device { get; set; }
        public event Action<string> ConnectEvent;

        public DeviceManager(AdminInfo info)
        {
            this.Info = info;
            Update(info);
        }
        public void Update(AdminInfo info)
        {
            if (Device != null)
            {
                Device.Close();
                Device.ConnectEvent -= Device_ConnectEvent;
            }
            if (info.Host == null) return;
            if (info.Host.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            {
                Device = new COMClient(info.Host);
            }
            else
            {
                Device = new TCPClient(info.Host, info.Port);
            }
            Device.ConnectEvent += Device_ConnectEvent;
        }
        public void Close()
        {
            IStop = true;
            Device.Close();
        }
        private void Device_ConnectEvent()
        {
            ConnectEvent?.Invoke(Device.Host);
        }

        #region 设备状态
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected { get { return Device.Connected; } }

        #endregion

        #region 通讯集成
        /// <summary>
        /// 输出结果
        /// </summary>
        public bool Result()
        {
            if (IStop) return false;
            return Execute(Info.Address, Info.Value);
        }
        public bool Execute(int address, int value, DependencyObject obj = null, string desc = null)
        {
            var result = Device.Send(new WriteMessage(address) { List = new List<short> { (short)value } });
            var iResult = result.List[0] == value;
            if (obj != null) Method.Toast(obj, desc + (iResult ? "成功" : "失败"));
            return iResult;
        }

        #endregion
    }
}
