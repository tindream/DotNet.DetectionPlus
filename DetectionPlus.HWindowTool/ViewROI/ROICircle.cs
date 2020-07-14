using System;
using HalconDotNet;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// This class demonstrates one of the possible implementations for a 
    /// circular ROI. ROICircle inherits from the base class ROI and 
    /// implements (besides other auxiliary methods) all virtual methods 
    /// defined in ROI.cs.
    /// </summary>
    public class ROICircle : ROI
    {
        #region ViewROI原程序
        private double radius;
        private double row1, col1;  // first handle
        private double midR, midC;  // second handle

        public ROICircle()
        {
            NumHandles = 2; // one at corner of circle + midpoint
            ActiveHandleIdx = 1;
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void CreateROI(double midX, double midY)
        {
            midR = midY;
            midC = midX;

            radius = RoiDrawConfig.Height / 2;

            row1 = midR;
            col1 = midC + radius;
        }

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void Draw(HWindow window)
        {
            window.DispCircle(midR, midC, radius);
            window.DispRectangle2(row1, col1, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
            window.DispRectangle2(midR, midC, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
        }

        /// <summary> 
        /// Returns the distance of the ROI handle being
        /// closest to the image point(x,y)
        /// </summary>
        public override double DistToClosestHandle(double x, double y)
        {
            double max = 10000;
            double[] val = new double[NumHandles];

            val[0] = HMisc.DistancePp(y, x, row1, col1); // border handle 
            val[1] = HMisc.DistancePp(y, x, midR, midC); // midpoint 

            for (int i = 0; i < NumHandles; i++)
            {
                if (val[i] < max)
                {
                    max = val[i];
                    ActiveHandleIdx = i;
                }
            }// end of for 
            return val[ActiveHandleIdx];
        }

        /// <summary> 
        /// Paints the active handle of the ROI object into the supplied window 
        /// </summary>
        public override void DisplayActive(HWindow window)
        {

            switch (ActiveHandleIdx)
            {
                case 0:
                    window.DispRectangle2(row1, col1, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
                    break;
                case 1:
                    window.DispRectangle2(midR, midC, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
                    break;
            }
        }

        /// <summary>Gets the HALCON region described by the ROI</summary>
        public override HObject GetRegion()
        {
            HObject region;
            //region.GenCircle(midR, midC, radius);
            HOperatorSet.GenCircle(out region, midR, midC, radius);
            return region;
        }

        public override double GetDistanceFromStartPoint(double row, double col)
        {
            double sRow = midR; // assumption: we have an angle starting at 0.0
            double sCol = midC + 1 * radius;

            double angle = HMisc.AngleLl(midR, midC, sRow, sCol, midR, midC, row, col);

            if (angle < 0)
                angle += 2 * Math.PI;

            return (radius * angle);
        }

        /// <summary>
        /// Gets the model information described by 
        /// the  ROI
        /// </summary> 
        public override HTuple GetModelData()
        {
            return new HTuple(new double[] { midR, midC, radius });
        }

        /// <summary> 
        /// Recalculates the shape of the ROI. Translation is 
        /// performed at the active handle of the ROI object 
        /// for the image coordinate (x,y)
        /// </summary>
        public override void MoveByHandle(double newX, double newY)
        {
            HTuple distance;
            double shiftX, shiftY;

            switch (ActiveHandleIdx)
            {
                case 0: // handle at circle border

                    row1 = newY;
                    col1 = newX;
                    HOperatorSet.DistancePp(new HTuple(row1), new HTuple(col1),
                                            new HTuple(midR), new HTuple(midC),
                                            out distance);

                    radius = distance[0].D;
                    break;
                case 1: // midpoint 

                    shiftY = midR - newY;
                    shiftX = midC - newX;

                    midR = newY;
                    midC = newX;

                    row1 -= shiftY;
                    col1 -= shiftX;
                    break;
            }
        }

        #endregion

        #region 自编新增功能
        /// <summary>
        /// 返回ROI中心点
        /// </summary>
        public override HalconPoint GetCenter()
        {
            return new HalconPoint(midC, midR);
        }

        #endregion
    }
}
