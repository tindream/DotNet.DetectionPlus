using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// 消息框显示的配置
    /// </summary>
    public class MessageConfig
    {
        public string Name;

        /// <summary>
        /// 消息
        /// </summary>
        public string Message;

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
        /// 绑定方位
        /// </summary>
        public AnchorType AnchorType;

        /// <summary>
        /// 同等位
        /// </summary>
        public CoordSystemType CoordSystemType;

        /// <summary>
        /// 是否显示边框
        /// </summary>
        public bool IsBox;

        /// <summary>
        /// 构造函数(消息，颜色，位置X，位置Y，绑定方位，同等位要求，是否显示边框)
        /// </summary>
        public MessageConfig(string name, string msg, Color color, int x, int y, AnchorType anchorType, CoordSystemType coordType, bool isBox)
        {
            this.Name = name;
            this.Message = msg;
            this.Color = color;
            this.ColorStr = HalconConfig.ColorToStr(color);
            this.X = x;
            this.Y = y;
            this.AnchorType = anchorType;
            this.CoordSystemType = coordType;
            this.IsBox = isBox;
        }
    }
}
