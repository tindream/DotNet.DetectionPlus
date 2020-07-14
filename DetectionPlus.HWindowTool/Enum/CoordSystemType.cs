using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// 同等位模式
    /// </summary>
    public enum CoordSystemType
    {
        /// <summary>
        /// 以窗体控件为主要坐标点
        /// </summary>
        window = 0,
        /// <summary>
        /// 以图像尺寸为主要坐标点
        /// </summary>
        image = 1,
    }
}
