using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// 坐标点类库
    /// </summary>
    public class HalconPoint
    {
        public double X;
        public double Y;

        public HalconPoint() { }
        public HalconPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
