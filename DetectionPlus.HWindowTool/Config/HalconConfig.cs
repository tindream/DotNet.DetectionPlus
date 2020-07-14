using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DetectionPlus.HWindowTool
{
    public class HalconConfig
    {
        /// <summary>
        /// 转换颜色代码
        /// </summary>
        public static string ColorToStr(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
