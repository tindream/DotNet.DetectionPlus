using DetectionPlus.Camera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Sign
{
    public class Config : DetectionPlus.Config
    {
        #region 常量
        public new const string Text = "相机模式";

        #endregion

        #region 全局数据
        public static ICamera Camera { get; set; }
        public static AdminInfo Admin { get; set; }
        /// <summary>
        /// 模板保存路径
        /// </summary>
        public static string Template
        {
            get
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        #endregion
    }
}
