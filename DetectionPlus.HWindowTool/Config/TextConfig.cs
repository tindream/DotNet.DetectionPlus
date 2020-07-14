using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// 文本显示的配置
    /// </summary>
    public class TextConfig
    {
        public string Name;

        /// <summary>
        /// 文本
        /// </summary>
        public string Text;

        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color;

        /// <summary>
        /// 颜色代码
        /// </summary>
        public string ColorStr;

        /// <summary>
        /// 位置X
        /// </summary>
        public int X;
        /// <summary>
        /// 位置Y
        /// </summary>
        public int Y;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextConfig(string name, string text, Color color, int x, int y)
        {
            this.Name = name;
            this.Text = text;
            this.Color = color;
            this.ColorStr = HalconConfig.ColorToStr(color);
            this.X = x;
            this.Y = y;
        }
    }
}
