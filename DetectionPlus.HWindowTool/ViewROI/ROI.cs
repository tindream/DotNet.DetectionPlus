using System;
using HalconDotNet;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// This class is a base class containing virtual methods for handling
    /// ROIs. Therefore, an inheriting class needs to define/override these
    /// methods to provide the ROIController with the necessary information on
    /// its (= the ROIs) shape and position. The example project provides 
    /// derived ROI shapes for rectangles, lines, circles, and circular arcs.
    /// To use other shapes you must derive a new class from the base class 
    /// ROI and implement its methods.
    /// </summary>    
    public class ROI
    {
        #region ViewROI原程序
        protected HTuple posOperation = new HTuple();
        protected HTuple negOperation = new HTuple(new int[] { 2, 2 });

        // class members of inheriting ROI classes
        public int NumHandles;
        public int ActiveHandleIdx;

        private int operatorFlag;
        /// <summary>
        /// Flag to define the ROI to be 'positive' or 'negative'.
        /// </summary>
        public int OperatorFlag
        {
            get { return operatorFlag; }
            set
            {
                operatorFlag = value;
                switch (operatorFlag)
                {
                    case ROIController.MODE_ROI_POS:
                        flagLineStyle = posOperation;
                        break;
                    case ROIController.MODE_ROI_NEG:
                        flagLineStyle = negOperation;
                        break;
                    default:
                        flagLineStyle = posOperation;
                        break;
                }
            }
        }

        /// <summary>
        /// Parameter to define the line style of the ROI.
        /// </summary>
        public HTuple flagLineStyle;

        /// <summary>
        /// ROI合并模式
        /// </summary>
        public ROIMergeType ROIMergeType = ROIMergeType.Not;
        /// <summary>
        /// 绘制配置
        /// </summary>
        public RoiDrawConfig RoiDrawConfig = new RoiDrawConfig();
        /// <summary>
        /// 最近一次显示的图像
        /// </summary>
        public HImage Image { get; private set; }

        /// <summary>Constructor of abstract ROI class.</summary>
        public ROI() { }

        /// <summary>Creates a new ROI instance at the mouse position.</summary>
        /// <param name="midX">
        /// x (=column) coordinate for ROI
        /// </param>
        /// <param name="midY">
        /// y (=row) coordinate for ROI
        /// </param>
        public virtual void CreateROI(double midX, double midY) { }

        /// <summary>
        /// Paints the ROI into the supplied window.
        /// <param name="window">HALCON window</param>
        /// </summary>
        public virtual void Draw(HWindow window) { }

        /// <summary> 
        /// Returns the distance of the ROI handle being
        /// closest to the image point(x,y)
        /// </summary>
        /// <param name="x">x (=column) coordinate</param>
        /// <param name="y">y (=row) coordinate</param>
        /// <returns> 
        /// Distance of the closest ROI handle.
        /// </returns>
        public virtual double DistToClosestHandle(double x, double y)
        {
            return 0.0;
        }

        /// <summary> 
        /// Paints the active handle of the ROI object into the supplied window. 
        /// </summary>
        /// <param name="window">HALCON window</param>
        public virtual void DisplayActive(HWindow window) { }

        /// <summary> 
        /// Recalculates the shape of the ROI. Translation is 
        /// performed at the active handle of the ROI object 
        /// for the image coordinate (x,y).
        /// </summary>
        /// <param name="x">x (=column) coordinate</param>
        /// <param name="y">y (=row) coordinate</param>
        public virtual void MoveByHandle(double x, double y) { }

        /// <summary>Gets the HALCON region described by the ROI.</summary>
        public virtual HObject GetRegion()
        {
            return null;
        }

        public virtual double GetDistanceFromStartPoint(double row, double col)
        {
            return 0.0;
        }
        /// <summary>
        /// Gets the model information described by 
        /// the ROI.
        /// </summary> 
        public virtual HTuple GetModelData()
        {
            return null;
        }

        #endregion

        #region 自编新增功能
        /// <summary>
        /// 返回ROI中心点X
        /// </summary>
        public virtual HalconPoint GetCenter()
        {
            return new HalconPoint(0, 0);
        }
        /// <summary>
        /// 设置绘制配置（配置）
        /// </summary>
        public void SetDrawConfig(HImage image, RoiDrawConfig roiDrawConfig, ROIMergeType modeRoi)
        {
            this.Image = image;
            this.RoiDrawConfig = roiDrawConfig;
            this.ROIMergeType = modeRoi;
            OperatorFlag = (int)modeRoi;
            CreateROI(roiDrawConfig.CreateX, roiDrawConfig.CreateY);    //设置ROI位置
        }

        #endregion
    }
}
