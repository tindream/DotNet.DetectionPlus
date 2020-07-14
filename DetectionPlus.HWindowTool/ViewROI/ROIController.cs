using System;
using HalconDotNet;
using System.Collections;
using System.Collections.Generic;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// This class creates and manages ROI objects. It responds 
    /// to  mouse device inputs using the methods mouseDownAction and 
    /// mouseMoveAction. You don't have to know this class in detail when you 
    /// build your own C# project. But you must consider a few things if 
    /// you want to use interactive ROIs in your application: There is a
    /// quite close connection between the ROIController and the ViewController 
    /// class, which means that you must 'register' the ROIController
    /// with the ViewController, so the ViewController knows it has to forward user input
    /// (like mouse events) to the ROIController class.  
    /// The visualization and manipulation of the ROI objects is done 
    /// by the ROIController.
    /// This class provides special support for the matching
    /// applications by calculating a model region from the list of ROIs. For
    /// this, ROIs are added and subtracted according to their sign.
    /// </summary>
    public class ROIController
    {
        #region 常量
        /// <summary>
        /// Constant for setting the ROI mode: positive ROI sign.
        /// </summary>
        public const int MODE_ROI_POS = 21;

        /// <summary>
        /// Constant for setting the ROI mode: negative ROI sign.
        /// </summary>
        public const int MODE_ROI_NEG = 22;

        /// <summary>
        /// Constant for setting the ROI mode: no model region is computed as
        /// the sum of all ROI objects.
        /// </summary>
        public const int MODE_ROI_NONE = 23;

        #endregion

        #region 私有变量
        private double currX, currY;
        /// <summary>
        /// Reference to the ViewController, the ROI Controller is registered to
        /// </summary>
        private readonly ViewController viewController;

        #endregion

        #region 公共属性
        public ROI ROI;
        /// <summary>
        /// Index of the active ROI object
        /// </summary>
        public int ActiveROIidx = -1;
        /// <summary>
        /// List containing all created ROI objects so far
        /// </summary>
        public readonly List<ROI> ROIList = new List<ROI>();
        public string ActiveCol = "green";
        public string ActiveHdlCol = "red";
        public string InactiveCol = "yellow";
        /// <summary>
        /// 绘制配置
        /// </summary>
        public RoiDrawConfig RoiDrawConfig = new RoiDrawConfig();

        #endregion

        #region ViewROI原程序
        /// <summary>Constructor</summary>
        public ROIController(ViewController view)
        {
            this.viewController = view;
            ActiveROIidx = -1;
            currX = currY = -1;
        }

        /// <summary>
        /// To create a new ROI object the application class initializes a 
        /// 'seed' ROI instance and passes it to the ROIController.
        /// The ROIController now responds by manipulating this new ROI
        /// instance.
        /// </summary>
        /// <param name="r">
        /// 'Seed' ROI object forwarded by the application forms class.
        /// </param>
        public void SetROIShape(ROI r)
        {
            ROI = r;
            ROI.OperatorFlag = MODE_ROI_NONE;
        }
        /// <summary>
        /// Removes the ROI object that is marked as active. 
        /// If no ROI object is active, then nothing happens. 
        /// </summary>
        public void RemoveActive()
        {
            if (ActiveROIidx != -1)
            {
                ROIList.RemoveAt(ActiveROIidx);
                //activeROIidx = -1;
                ActiveROIidx = ROIList.Count - 1;

                viewController.OperationGatherRegion(); //计算ROI合并区域，并判断是否刷新显示
                if (viewController.GatherRegionCount == 0) viewController.Repaint();  //刷新显示(强制刷新一次)
            }
        }
        /// <summary>
        /// Clears all variables managing ROI objects
        /// </summary>
        public void Reset()
        {
            ROIList.Clear();
            ActiveROIidx = -1;
            ROI = null;
        }
        /// <summary>
        /// Deletes this ROI instance if a 'seed' ROI object has been passed
        /// to the ROIController by the application class.
        /// 
        /// </summary>
        public void ResetROI()
        {
            ActiveROIidx = -1;
            ROI = null;
        }
        /// <summary>
        /// Paints all objects from the ROIList into the HALCON window
        /// </summary>
        /// <param name="window">HALCON window</param>
        public void PaintData(HWindow window)
        {
            window.SetDraw("margin");
            window.SetLineWidth(1);

            if (ROIList.Count > 0)
            {
                window.SetColor(InactiveCol);
                //window.SetDraw("margin");

                for (int i = 0; i < ROIList.Count; i++)
                {
                    if (ActiveROIidx != i)
                    {
                        //显示ROI
                        window.SetLineStyle(ROIList[i].flagLineStyle);
                        ROIList[i].Draw(window);

                        //显示序号
                        if (RoiDrawConfig.IsDrawIndex)
                        {
                            ROI roi = ROIList[i];
                            var point = roi.GetCenter();
                            double row = point.Y + RoiDrawConfig.PaneWidth;
                            double column = point.X + RoiDrawConfig.PaneWidth;
                            HOperatorSet.SetTposition(window, row, column);
                            HOperatorSet.WriteString(window, i.ToString());
                        }
                    }
                }

                if (ActiveROIidx != -1)
                {
                    //显示选中的ROI
                    window.SetColor(ActiveCol);
                    window.SetLineStyle(ROIList[ActiveROIidx].flagLineStyle);
                    ROIList[ActiveROIidx].Draw(window);

                    //显示选中的ROI序号
                    if (RoiDrawConfig.IsDrawIndex)
                    {
                        ROI roi = ROIList[ActiveROIidx];
                        var point = roi.GetCenter();
                        var row = point.Y + RoiDrawConfig.PaneWidth;
                        var column = point.X + RoiDrawConfig.PaneWidth;
                        HOperatorSet.SetTposition(window, row, column);
                        HOperatorSet.WriteString(window, ActiveROIidx.ToString());
                    }

                    //显示选中的小方框
                    window.SetColor(ActiveHdlCol);
                    ROIList[ActiveROIidx].DisplayActive(window);
                }
            }
        }

        /// <summary>
        /// Reaction of ROI objects to the 'mouse button down' event: changing
        /// the shape of the ROI and adding it to the ROIList if it is a 'seed'
        /// ROI.
        /// </summary>
        /// <param name="imgX">x coordinate of mouse event</param>
        /// <param name="imgY">y coordinate of mouse event</param>
        /// <returns></returns>
        public int MouseDownAction(double imgX, double imgY)
        {
            int idxROI = -1;
            double max = 10000, dist;
            //maximal shortest distance to one of
            //the handles
            //选中范围=ROI方格中心线长
            double epsilon = RoiDrawConfig.PaneWidth * Math.Sqrt(2);
            //选中范围缩小到10，并按缩放比例自动调整，最低Math.Sqrt(15)
            //if (epsilon < Math.Sqrt(15)) epsilon = Math.Sqrt(15);

            if (ROI != null)             //either a new ROI object is created
            {
                ROI.CreateROI(imgX, imgY);
                ROIList.Add(ROI);
                ROI = null;
                ActiveROIidx = ROIList.Count - 1;
                viewController.Repaint();
            }
            else if (ROIList.Count > 0)     // ... or an existing one is manipulated
            {
                //判断是否点击已被选中的ROI
                if (ActiveROIidx != -1)
                {
                    dist = ROIList[ActiveROIidx].DistToClosestHandle(imgX, imgY);
                    if ((dist < max) && (dist < epsilon))
                    {
                        return ActiveROIidx;
                    }
                }

                //扫描全部，改为倒序扫描
                ActiveROIidx = -1;
                //for (int i = 0; i < ROIList.Count; i++)
                for (int i = ROIList.Count - 1; i >= 0; i--)
                {
                    dist = ROIList[i].DistToClosestHandle(imgX, imgY);
                    if ((dist < max) && (dist < epsilon))
                    {
                        max = dist;
                        idxROI = i;
                    }
                }//end of for

                if (idxROI >= 0)
                {
                    ActiveROIidx = idxROI;
                }

                viewController.Repaint();
            }
            return ActiveROIidx;
        }

        /// <summary>
        /// Reaction of ROI objects to the 'mouse button move' event: moving
        /// the active ROI.
        /// </summary>
        /// <param name="newX">x coordinate of mouse event</param>
        /// <param name="newY">y coordinate of mouse event</param>
        public void MouseMoveAction(double newX, double newY)
        {
            if ((newX == currX) && (newY == currY))
                return;

            ROIList[ActiveROIidx].MoveByHandle(newX, newY);
            viewController.Repaint();
            currX = newX;
            currY = newY;
        }

        #endregion

        #region 自编新增功能
        /// <summary>
        /// 设置绘制配置（配置）
        /// </summary>
        public void SetDrawConfig(RoiDrawConfig roiDrawConfig)
        {
            this.RoiDrawConfig = roiDrawConfig;
        }

        #endregion
    }
}
