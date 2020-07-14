using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// ROI合并模式
    /// </summary>
    public enum ROIMergeType
    {
        /// <summary>
        /// 叠加
        /// <para>设置ROI线条为实线</para>
        /// </summary>
        Add = 21,
        /// <summary>
        /// 删减
        /// <para>设置ROI线条为虚线</para>
        /// </summary>
        Sub = 22,
        /// <summary>
        /// 常规
        /// <para>设置ROI线条为实线（halcon默认，原因不详）</para>
        /// </summary>
        Not = 23,
    }
}
