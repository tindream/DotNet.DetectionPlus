using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// Region或XLD配置
    /// </summary>
    public class HObjectConfig
    {
        public string Name;

        /// <summary>
        /// Region和XLD
        /// </summary>
        public HObject HObject;

        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color;

        /// <summary>
        /// 颜色代码
        /// </summary>
        public string ColorStr;

        /// <summary>
        /// 区域填充模式
        /// </summary>
        public DrawModelType DrawModelType;

        /// <summary>
        /// 构造函数(Region和XLD，颜色，填充模式)
        /// </summary>
        public HObjectConfig(string name, HObject hObject, Color color, DrawModelType drawModelType)
        {
            this.Name = name;
            this.HObject = hObject;
            this.Color = color;
            this.ColorStr = HalconConfig.ColorToStr(color);
            this.DrawModelType = drawModelType;
        }
    }
}
