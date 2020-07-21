using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus
{
    public class Config
    {
        #region 常量
        public const string Text = "相机模式";

        #region modbus通讯
        /// <summary>
        /// modbus/TCP头长度
        /// </summary>
        public const int HeardSize = 8;
        /// <summary>
        /// modbus/TCP 固定报文
        /// </summary>
        public const string Header = "00 00 00 00 00 05";
        /// <summary>
        /// 寄存器类型-读功能码
        /// </summary>
        public const byte Read = 3;
        /// <summary>
        /// 寄存器类型-写功能码
        /// </summary>
        public const byte Write = 6;

        #endregion

        #endregion

        #region 全局数据
        public static Window Window { get; set; }

        #region 注册
        /// <summary>
        /// 本机唯一编码
        /// </summary>
        public static string MacId { get; set; }
        public static bool IListener { get; set; }

        #endregion

        #endregion

        public static string Images
        {
            get
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }
    }
    /// <summary>
    /// 通讯日志
    /// </summary>
    public class DeviceLog
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Log(string msg)
        {
            log.Debug(msg);
        }
    }
}
