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
    public class ROICircleRing : ROI
    {
        #region ViewROI原程序

        private double inner_Radius;
        private double out_Radius;
        private double inner_Row1, inner_Col1;  // first handle
        private double out_Row1, out_Col1;  // first handle


        private double midR, midC;  // second handle


        public ROICircleRing()
        {
            NumHandles = 3; // one at corner of circle + midpoint
            ActiveHandleIdx = 1;
        }



        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void CreateROI(double midX, double midY)
        {
            midR = midY;
            midC = midX;

            out_Radius = RoiDrawConfig.Height / 2;
            out_Row1 = midR;
            out_Col1 = midC + out_Radius;


            inner_Radius = out_Radius - 20;
            inner_Row1 = midR;
            inner_Col1 = midC + inner_Radius;

        }

        /// <summary>Paints the ROI into the supplied window</summary>
        /// <param name="window">HALCON window</param>
        public override void Draw(HalconDotNet.HWindow window)
        {
            window.DispCircle(midR, midC, out_Radius);
            window.DispCircle(midR, midC, inner_Radius);
            window.DispRectangle2(out_Row1, out_Col1, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
            window.DispRectangle2(inner_Row1, inner_Col1, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
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

            val[0] = HMisc.DistancePp(y, x, out_Row1, out_Col1); // border handle 
            val[1] = HMisc.DistancePp(y, x, inner_Row1, inner_Col1); // border handle 

            val[2] = HMisc.DistancePp(y, x, midR, midC); // midpoint 

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
        public override void DisplayActive(HalconDotNet.HWindow window)
        {

            switch (ActiveHandleIdx)
            {
                case 0:
                    window.DispRectangle2(out_Row1, out_Col1, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
                    break;
                case 1:
                    window.DispRectangle2(inner_Row1, inner_Col1, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
                    break;
                case 2:
                    window.DispRectangle2(midR, midC, 0, RoiDrawConfig.PaneWidth, RoiDrawConfig.PaneWidth);
                    break;
            }
        }

        /// <summary>Gets the HALCON region described by the ROI</summary>
        public override HObject GetRegion()
        {
            HObject region = new HObject();
            HObject region_Out = new HObject();
            HObject region_Inner = new HObject();

            //region.GenCircle(midR, midC, radius);
            HOperatorSet.GenCircle(out region_Out, midR, midC, out_Radius);
            HOperatorSet.GenCircle(out region_Inner, midR, midC, inner_Radius);

            HOperatorSet.Difference(region_Out, region_Inner, out region);

            return region;
        }

        public override double GetDistanceFromStartPoint(double row, double col)
        {
            //double sRow = midR; // assumption: we have an angle starting at 0.0
            //double sCol = midC + 1 * radius;

            //double angle = HMisc.AngleLl(midR, midC, sRow, sCol, midR, midC, row, col);

            //if (angle < 0)
            //	angle += 2 * Math.PI;

            //return (radius * angle);

            return 0;

        }

        /// <summary>
        /// Gets the model information described by 
        /// the  ROI
        /// </summary> 
        public override HTuple GetModelData()
        {
            if (ActiveHandleIdx == 0)
                return new HTuple(new double[] { midR, midC, out_Radius });
            else
                return new HTuple(new double[] { midR, midC, inner_Radius });
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

                    out_Row1 = newY;
                    out_Col1 = newX;
                    HOperatorSet.DistancePp(new HTuple(out_Row1), new HTuple(out_Col1),
                                            new HTuple(midR), new HTuple(midC),
                                            out distance);

                    out_Radius = distance[0].D;
                    break;
                case 1: // handle at circle border

                    inner_Row1 = newY;
                    inner_Col1 = newX;
                    HOperatorSet.DistancePp(new HTuple(inner_Row1), new HTuple(inner_Col1),
                                            new HTuple(midR), new HTuple(midC),
                                            out distance);

                    inner_Radius = distance[0].D;
                    break;
                case 2: // midpoint 

                    shiftY = midR - newY;
                    shiftX = midC - newX;

                    midR = newY;
                    midC = newX;

                    out_Row1 -= shiftY;
                    out_Col1 -= shiftX;

                    inner_Row1 -= shiftY;
                    inner_Col1 -= shiftX;

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
    }//end of class
}//end of namespace
