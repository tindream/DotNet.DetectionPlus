using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// Roi的绘制配置
    /// </summary>
    public class RoiDrawConfig
    {
        /// <summary>
        /// Roi调整方格宽度
        /// </summary>
        public double PaneWidth = 5;

        /// <summary>
        /// Roi初始生成位置
        /// </summary>
        public double CreateX = 100;
        /// <summary>
        /// Roi初始生成位置
        /// </summary>
        public double CreateY = 100;

        /// <summary>
        /// Roi的宽
        /// </summary>
        public double Width = 100;
        /// <summary>
        /// Roi的高
        /// </summary>
        public double Height = 100;

        /// <summary>
        /// 指示是否显示ROI下标
        /// </summary>
        public bool IsDrawIndex = true;

        /// <summary>
        /// 指示是否显示待生成ROI的预览
        /// </summary>
        public bool IsDrawPrepare = true;
    }
}
