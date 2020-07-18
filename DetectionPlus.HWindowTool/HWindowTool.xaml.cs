using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// HWindowTool.xaml 的交互逻辑
    /// </summary>
    public partial class HWindowTool : UserControl
    {
        #region 私有变量
        /// <summary>
        /// Roi的绘制配置（根据显示的图像自动计算）
        /// </summary>
        private readonly RoiDrawConfig roiDrawConfig = new RoiDrawConfig();

        /// <summary>
        /// 指示显示绘制已初始化完成
        /// </summary>
        private bool isDrawInit = false;

        /// <summary>
        /// 鼠标最近一次点击控件时间
        /// </summary>
        private DateTime mouseDownTime = new DateTime();

        /// <summary>
        /// 最近一次显示的图像
        /// </summary>
        public HImage Image { get; private set; }

        /// <summary>
        /// 双击间隔
        /// </summary>
        private readonly int interval = 500;
        /// <summary>
        /// 检索当前双击鼠标的时间
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetDoubleClickTime();

        #endregion

        #region 公共属性
        /// <summary>
        /// halcon ViewROI的显示实例
        /// </summary>
        public readonly ViewController ViewController;

        #endregion

        #region ROI属性
        /// <summary>
        /// 被选中的ROI颜色
        /// </summary>
        [Description("被选中的ROI颜色"), Category("ROI属性")] //显示在属性设计视图中的描述
        public Color ActiveRoiColor
        {
            set
            {
                _ActiveRoiColor = value;
                ViewController.RoiController.ActiveCol = HalconConfig.ColorToStr(_ActiveRoiColor);
            }
            get
            {
                return _ActiveRoiColor;
            }
        }
        public Color _ActiveRoiColor = Colors.Lime;

        /// <summary>
        /// 被选中的ROI操作方格颜色
        /// </summary>
        [Description("被选中的ROI操作方格颜色"), Category("ROI属性")] //显示在属性设计视图中的描述
        public Color ActiveRoiHdiColor
        {
            set
            {
                _ActiveRoiHdiColor = value;
                ViewController.RoiController.ActiveHdlCol = HalconConfig.ColorToStr(_ActiveRoiHdiColor);
            }
            get
            {
                return _ActiveRoiHdiColor;
            }
        }
        public Color _ActiveRoiHdiColor = Colors.Red;

        /// <summary>
        /// 非被选中的ROI颜色
        /// </summary>
        [Description("非被选中的ROI颜色"), Category("ROI属性")] //显示在属性设计视图中的描述
        public Color InactiveRoiColor
        {
            set
            {
                _InactiveRoiColor = value;
                ViewController.RoiController.InactiveCol = HalconConfig.ColorToStr(_InactiveRoiColor);
            }
            get
            {
                return _InactiveRoiColor;
            }
        }
        public Color _InactiveRoiColor = Colors.Yellow;

        /// <summary>
        /// 指示是否显示ROI序号
        /// </summary>
        [Description("指示是否显示ROI序号"), Category("ROI属性")] //显示在属性设计视图中的描述
        public bool IsDrawRoiIndex
        {
            set
            {
                if (_IsDrawRoiIndex == value) return;
                _IsDrawRoiIndex = value;
                roiDrawConfig.IsDrawIndex = _IsDrawRoiIndex;
                ViewController.RoiController.SetDrawConfig(roiDrawConfig); //输入ROI绘制配置
                if (isDrawInit) Repaint();  //刷新显示
            }
            get
            {
                return _IsDrawRoiIndex;
            }
        }
        private bool _IsDrawRoiIndex = true;

        /// <summary>
        /// 指示是否显示待生成ROI的预览
        /// </summary>
        [Description("指示是否显示待生成ROI的预览"), Category("ROI属性")] //显示在属性设计视图中的描述
        public bool IsDrawPrepareRoi
        {
            set
            {
                if (_IsDrawPrepareRoi == value) return;
                _IsDrawPrepareRoi = value;
                roiDrawConfig.IsDrawPrepare = _IsDrawPrepareRoi;
                ViewController.RoiController.SetDrawConfig(roiDrawConfig); //输入ROI绘制配置
                if (isDrawInit) Repaint();  //刷新显示
            }
            get
            {
                return _IsDrawPrepareRoi;
            }
        }
        private bool _IsDrawPrepareRoi = true;

        /// <summary>
        /// ROI合并模式
        /// </summary>
        [Description("ROI合并模式"), Category("ROI属性")] //显示在属性设计视图中的描述
        public ROIMergeType DrawModeRoi { set; get; } = ROIMergeType.Not;

        /// <summary>
        /// 操作句柄
        /// </summary>
        public HWindow HalconWindow
        {
            get
            {
                return viewPort.HalconWindow;
            }
        }

        #endregion

        #region 控件属性
        private double _zoomViewPercentage = 100;
        /// <summary>
        /// 图像的显示缩放倍率（默认为100，修改后自动刷新显示）
        /// </summary>
        [Description("图像的显示缩放倍率（默认为100，修改后自动刷新显示）"), Category("控件属性")] //显示在属性设计视图中的描述
        public double ZoomViewPercentage
        {
            set
            {
                if (_zoomViewPercentage == value) return;
                if (value <= ZoomViewPercentage_Min) _zoomViewPercentage = ZoomViewPercentage_Min;
                else if (value >= ZoomViewPercentage_Max) _zoomViewPercentage = ZoomViewPercentage_Max;
                else _zoomViewPercentage = value;
                ViewController.ZoomByGUIHandle(_zoomViewPercentage);  //设置显示倍率
                if (isDrawInit) Repaint();  //刷新显示
                ZoomChanged?.Invoke(this, new EventArgs());//将事件传递的方法
            }
            get
            {
                return _zoomViewPercentage;
            }
        }

        private double _zoomViewPercentage_Max = 3000;
        /// <summary>
        /// 图像的显示缩放的最大设置倍率
        /// </summary>
        [Description("图像的显示缩放的最大设置倍率"), Category("控件属性")] //显示在属性设计视图中的描述
        public double ZoomViewPercentage_Max
        {
            get { return _zoomViewPercentage_Max; }
            set
            {
                if (value > 3000) _zoomViewPercentage_Max = 3000;
                else _zoomViewPercentage_Max = value;
            }
        }

        private double _zoomViewPercentage_Min = 5;
        /// <summary>
        /// 图像的显示缩放的最小设置倍率
        /// </summary>
        [Description("图像的显示缩放的最小设置倍率"), Category("控件属性")] //显示在属性设计视图中的描述
        public double ZoomViewPercentage_Min
        {
            get { return _zoomViewPercentage_Min; }
            set
            {
                if (value < 5) _zoomViewPercentage_Min = 5;
                else _zoomViewPercentage_Min = value;
            }
        }

        /// <summary>
        /// 指示是否允许鼠标控制显示图像的缩放
        /// </summary>
        [Description("指示是否允许鼠标控制显示图像的缩放"), Category("控件属性")] //显示在属性设计视图中的描述
        public bool IsZoomImageEnabled { set; get; } = true;

        private bool _IsMoveImageEnabled = true;
        /// <summary>
        /// 指示是否允许鼠标控制显示图像的移动
        /// </summary>
        [Description("指示是否允许鼠标控制显示图像的移动"), Category("控件属性")] //显示在属性设计视图中的描述
        public bool IsMoveImageEnabled
        {
            set
            {
                if (_IsMoveImageEnabled == value) return;
                _IsMoveImageEnabled = value;

                //设置窗口操作模式为移动
                if (_IsMoveImageEnabled) ViewController.SetViewState(ViewController.MODE_VIEW_MOVE);
                //设置窗口操作模式为无
                else ViewController.SetViewState(ViewController.MODE_VIEW_NONE);
            }
            get
            {
                return _IsMoveImageEnabled;
            }
        }

        /// <summary>
        /// 指示是否显示十字线
        /// </summary>
        [Description("指示是否显示十字线"), Category("控件属性")] //显示在属性设计视图中的描述
        public bool IsDisplayCross
        {
            set
            {
                ViewController.IsDisplayCross = value;
            }
            get
            {
                return ViewController.IsDisplayCross;
            }
        }

        #endregion

        #region 合并区域属性
        /// <summary>
        /// 指示是否显示合并ROI的区域
        /// </summary>
        [Description("指示是否显示合并ROI的区域"), Category("合并区域属性")] //显示在属性设计视图中的描述
        public bool IsGatherRegionShow
        {
            get { return ViewController.IsGatherRegionShow; }
            set { ViewController.IsGatherRegionShow = value; }
        }

        /// <summary>
        /// 合并ROI的区域的显示线宽
        /// </summary>
        [Description("合并ROI的区域的显示线宽"), Category("合并区域属性")] //显示在属性设计视图中的描述
        public int GatherRegionLineWidth
        {
            get { return ViewController.GatherRegionLineWidth; }
            set { ViewController.GatherRegionLineWidth = value; }
        }

        /// <summary>
        /// 合并ROI的区域的显示颜色
        /// </summary>
        [Description("合并ROI的区域的显示颜色"), Category("合并区域属性")] //显示在属性设计视图中的描述
        public Color GatherRegionColor
        {
            get { return ViewController.GatherRegionColor; }
            set
            {
                ViewController.GatherRegionColor = value;
                ViewController.GatherRegionColorStr = HalconConfig.ColorToStr(value);
            }
        }

        #endregion

        #region 事件
        /// <summary>
        /// 图像显示倍率属性变更时发生
        /// </summary>
        [Description("图像显示倍率属性变更时发生"), Category("自定义事件")]
        public event EventHandler ZoomChanged;

        /// <summary>
        /// 被选中的ROI变化时发生
        /// </summary>
        [Description("被选中的ROI变化时发生"), Category("自定义事件")]
        public event EventHandler ActiveRoiChanged;

        /// <summary>
        /// 鼠标单击控件时发生
        /// </summary>
        [Description("鼠标单击控件时发生"), Category("自定义事件")]
        public event EventHandler HMouseDown;

        /// <summary>
        /// 鼠标双击控件时发生
        /// </summary>
        [Description("鼠标双击控件时发生"), Category("自定义事件")]
        public event EventHandler HMouseDoubleDown;

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public HWindowTool()
        {
            InitializeComponent();
            ViewController = new ViewController(viewPort);
            this.interval = GetDoubleClickTime();
            viewPort.HMouseDown += ViewPort_HMouseDown;
            viewPort.SizeChanged += ViewPort_Resize;

            //建立鼠标滚轮触发事件
            viewPort.HMouseWheel += ViewPort_HMouseWheel;
            //建立被选中ROI变化时触发事件（用于传递）
            ViewController.HWndActiveRoiChanged += new System.EventHandler(this.ViewController_HWndActiveRoiChanged);
        }

        #endregion

        #region 动作事件
        /// <summary>
        /// 被选中ROI变化时触发事件（用于传递）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewController_HWndActiveRoiChanged(object sender, EventArgs e)
        {
            ActiveRoiChanged?.Invoke(this, new EventArgs());//将事件传递的方法
        }

        /// <summary>
        /// 鼠标单击控件时触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPort_HMouseDown(object sender, HMouseEventArgsWPF e)
        {
            HMouseDown?.Invoke(this, new EventArgs());//将事件传递的方法

            //判断双击
            if (DateTime.Now.Subtract(mouseDownTime).TotalMilliseconds <= this.interval)
            {
                mouseDownTime = new DateTime();
                HMouseDoubleDown?.Invoke(this, new EventArgs());//将事件传递的方法
            }
            else
            {
                mouseDownTime = DateTime.Now;
            }

        }

        /// <summary>
        /// 控制大小发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPort_Resize(object sender, EventArgs e)
        {
            if (isDrawInit) Repaint();//显示
        }

        #endregion

        #region 缩放
        /// <summary>
        /// 鼠标滚轮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPort_HMouseWheel(object sender, HMouseEventArgsWPF e)
        {
            //图像缩放使能
            if (!IsZoomImageEnabled) return;

            //向前滚
            if (e.Delta > 0)
            {
                ZoomViewPercentage *= 1 / 0.9;
            }
            //向后滚
            else
            {
                ZoomViewPercentage *= 0.9;
            }
        }

        /// <summary>
        /// 放大显示图像
        /// </summary>
        public void ZoomInImage()
        {
            ZoomViewPercentage *= 1 / 0.9;
        }

        /// <summary>
        /// 缩小显示图像
        /// </summary>
        public void ZoomOutImage()
        {
            ZoomViewPercentage *= 0.9;
        }

        /// <summary>
        /// 重置显示图像位置和尺寸(适应窗口)
        /// </summary>
        public void ResetWindow()
        {
            //重置图像显示
            ViewController.ResetWindow();
            //重置显示倍率，并刷新显示
            if (ZoomViewPercentage != 100) ZoomViewPercentage = 100;
            else Repaint();  //刷新显示

        }

        #endregion

        #region 绘制ROI
        /// <summary>
        /// 添加ROI
        /// </summary>
        /// <param name="roi"></param>
        public void AddRoi(ROI roi)
        {
            ROI roiMode = roi;
            roiMode.SetDrawConfig(this.Image, roiDrawConfig, DrawModeRoi); //输入ROI绘制配置
            ViewController.RoiController.ROIList.Add(roiMode); //添加ROI
            ViewController.RoiController.ActiveROIidx = ViewController.RoiController.ROIList.Count - 1;   //设置被选中ROI
            ViewController.OperationGatherRegion(); //计算ROI合并区域，并判断是否刷新显示
            if (ViewController.GatherRegionCount == 0) Repaint();  //刷新显示(强制刷新一次)
            ActiveRoiChanged?.Invoke(this, new EventArgs());//将事件传递的方法
        }

        /// <summary>
        /// 设置点击生成ROI的样式(通过鼠标点击后生成)
        /// </summary>
        /// <param name="roi"></param>
        public void SetROIShape(ROI roi)
        {
            ROI roiMode = roi;
            ViewController.RoiController.SetROIShape(roiMode);//设置生成ROI样式
            roiMode.SetDrawConfig(this.Image, roiDrawConfig, DrawModeRoi); //输入ROI绘制配置
            ViewController.RoiController.ActiveROIidx = -1;   //取消ROI选中
            Repaint();  //刷新显示
        }

        /// <summary>
        /// 取消点击生成ROI
        /// </summary>
        public void ResetROI()
        {
            ViewController.RoiController.ResetROI();
            Repaint();  //刷新显示
        }

        /// <summary>
        /// 删除选中的ROI
        /// </summary>
        public void RemoveActiveRoi()
        {
            ViewController.RoiController.RemoveActive();
            ActiveRoiChanged?.Invoke(this, new EventArgs());//将事件传递的方法
        }

        /// <summary>
        /// 设置选中ROI（指定序号）
        /// </summary>
        /// <param name="index"></param>
        public void SetActiveRoi(int index)
        {
            if (index >= ViewController.RoiController.ROIList.Count)
            {
                throw new Exception("序号超出列表界限");
            }

            if (ViewController.RoiController.ActiveROIidx == index) return;

            ViewController.RoiController.ActiveROIidx = index;
            Repaint();  //刷新显示

            ActiveRoiChanged?.Invoke(this, new EventArgs());//将事件传递的方法
        }

        #endregion

        #region 显示
        /// <summary>
        /// 显示(图像)
        /// </summary>
        public HObject DisplayImage(string file)
        {
            if (!File.Exists(file)) return null;
            HOperatorSet.ReadImage(out HObject ho_Image, file);
            DisplayImage(ho_Image);
            Repaint();
            return ho_Image;
        }
        /// <summary>
        /// 显示(图像)
        /// </summary>
        public HObject DisplayImage(System.Drawing.Bitmap bitmap)
        {
            BitmapData bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            /* Get the pointer to the bitmap's buffer. */
            IntPtr ptrBmp = bmpData.Scan0;
            HOperatorSet.GenImage1(out HObject ho_Image, "byte", bitmap.Width, bitmap.Height, ptrBmp);
            bitmap.UnlockBits(bmpData);

            DisplayImage(ho_Image);
            Repaint();
            return ho_Image;
        }
        /// <summary>
        /// 显示(图像)
        /// </summary>
        public void DisplayImage(HObject image)
        {
            //线程锁定
            lock (ViewController.LockDisplay)
            {
                ROI roi = new ROIRectangle1();

                //转换图像
                Image = new HImage(image);

                //计算Roi的绘制配置
                int h, w;
                string s;
                Image.GetImagePointer1(out s, out w, out h);
                roiDrawConfig.PaneWidth = Math.Round((double)h / 100);
                roiDrawConfig.CreateX = Math.Round((double)w / 5);
                roiDrawConfig.CreateY = Math.Round((double)h / 5);
                roiDrawConfig.Width = Math.Round((double)w / 5);
                roiDrawConfig.Height = Math.Round((double)h / 5);
                ViewController.RoiController.SetDrawConfig(roiDrawConfig); //输入ROI绘制配置

                //加入图像
                ViewController.AddIconicVar(Image);

                //设置显示倍率和显示
                ViewController.ZoomByGUIHandle(ZoomViewPercentage);  //设置显示倍率
            }
        }

        /// <summary>
        /// 清空所有显示
        /// </summary>
        public void ClearWindow()
        {
            ClearRoi();
            ClearImage();
            ViewController.ClearHObject();
            ViewController.ClearText();
            ViewController.ClearMessage();
        }

        /// <summary>
        /// 清空所有ROI
        /// </summary>
        public void ClearRoi()
        {
            ViewController.RoiController.Reset();
        }

        /// <summary>
        /// 清空所有图像
        /// </summary>
        public void ClearImage()
        {
            //线程锁定
            lock (ViewController.LockDisplay)
            {
                ViewController.ClearList();
                Image = null;
            }
        }

        /// <summary>
        /// 刷新显示
        /// </summary>
        public void Repaint()
        {
            ViewController.Repaint();  //显示
            isDrawInit = true;  //显示绘制初始完成
        }

        #endregion

        #region 公开内部方法
        /// <summary>
        /// 获取全部ROI的合并区域（ROICircularArc和ROILine不参与）
        /// </summary>
        /// <returns></returns>
        public HObject GetGatherRegion()
        {
            if (ViewController.GatherRegionCount == 0) return null;
            else return ViewController.GatherRegion;
        }
        /// <summary>
        /// 获取被选中的ROI序号
        /// </summary>
        /// <returns></returns>
        public int GetActiveRoiIndex()
        {
            return ViewController.RoiController.ActiveROIidx;
        }
        /// <summary>
        /// 获取ROI数量
        /// </summary>
        /// <returns></returns>
        public int GetRoiCount()
        {
            return ViewController.RoiController.ROIList.Count;
        }
        /// <summary>
        /// 获取ROI的类型列表
        /// </summary>
        public List<string> GetRoiTypeList()
        {
            List<string> list = new List<string>();
            foreach (ROI roi in ViewController.RoiController.ROIList)
            {
                list.Add(roi.ToString().Replace("ViewROI.ROI", ""));
            }
            return list;
        }
        /// <summary>
        /// 获取ROI的类型（指定序号）
        /// </summary>
        public string GetRoiType(int index)
        {
            return ViewController.RoiController.ROIList[index].ToString().Replace("ViewROI.ROI", "");
        }
        /// <summary>
        /// 获取ROI的合并模式列表
        /// </summary>
        public List<ROIMergeType> GetRoiModeList()
        {
            List<ROIMergeType> list = new List<ROIMergeType>();
            foreach (ROI roi in ViewController.RoiController.ROIList)
            {
                list.Add(roi.ROIMergeType);
            }
            return list;
        }
        /// <summary>
        /// 获取ROI的合并模式（指定序号）
        /// </summary>
        public ROIMergeType GetRoiMode(int index)
        {
            return ViewController.RoiController.ROIList[index].ROIMergeType;
        }
        /// <summary>
        /// 获取ROI的中心点坐标
        /// </summary>
        public List<HalconPoint> GetRoiCenterList()
        {
            var list = new List<HalconPoint>();
            foreach (ROI roi in ViewController.RoiController.ROIList)
            {
                var point = roi.GetCenter();
                list.Add(point);
            }
            return list;
        }
        /// <summary>
        /// 获取ROI的中心点坐标（指定序号）
        /// </summary>
        public HalconPoint GetRoiCenter(int index)
        {
            var point = ViewController.RoiController.ROIList[index].GetCenter();
            return point;
        }
        /// <summary>
        /// 获取ROI的Region或XLD
        /// </summary>
        public List<HObject> GetRoiHObjectList()
        {
            List<HObject> list = new List<HObject>();
            foreach (ROI roi in ViewController.RoiController.ROIList)
            {
                list.Add(roi.GetRegion());
            }
            return list;
        }
        /// <summary>
        /// 获取ROI的Region或XLD（指定序号）
        /// </summary>
        public HObject GetRoiHObject(int index)
        {
            return ViewController.RoiController.ROIList[index].GetRegion();
        }

        #endregion

        #region 新增功能-Region和XLD显示
        /// <summary>
        /// 显示(Region和XLD，颜色，填充模式)
        /// </summary>
        public void DisplayHObject(HObject obj, Color color, DrawModelType drawModel)
        {
            //显示
            ViewController.AddHObject(obj.GetHashCode().ToString(), obj, color, drawModel);
        }

        #endregion
    }
}
