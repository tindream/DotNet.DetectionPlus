using System;
using System.Collections;
using HalconDotNet;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Input;

namespace DetectionPlus.HWindowTool
{
    /// <summary>
    /// This class works as a wrapper class for the HALCON window
    /// HWindow. ViewController is in charge of the visualization.
    /// You can move and zoom the visible image part by using GUI component 
    /// inputs or with the mouse. The class ViewController uses a graphics stack 
    /// to manage the iconic objects for the display. Each object is linked 
    /// to a graphical context, which determines how the object is to be drawn.
    /// The context can be changed by calling changeGraphicSettings().
    /// The graphical "modes" are defined by the class GraphicsContext and 
    /// map most of the dev_set_* operators provided in HDevelop.
    /// </summary>
    public class ViewController
    {
        #region 常量
        /// <summary>
        /// No action is performed on mouse events
        /// 窗口操作模式为无
        /// </summary>
        public const int MODE_VIEW_NONE = 10;

        /// <summary>
        /// Zoom is performed on mouse events
        /// </summary>
        public const int MODE_VIEW_ZOOM = 11;

        /// <summary>
        /// Move is performed on mouse event
        /// 窗口操作模式为移动
        /// </summary>
        public const int MODE_VIEW_MOVE = 12;

        /// <summary>
        /// Magnification is performed on mouse events
        /// </summary>
        public const int MODE_VIEW_ZOOMWINDOW = 13;

        public const int MODE_INCLUDE_ROI = 1;

        public const int MODE_EXCLUDE_ROI = 2;

        /// <summary>
        /// Constant describes delegate message to signal new image
        /// </summary>
        public const int EVENT_UPDATE_IMAGE = 31;
        /// <summary>
        /// Constant describes delegate message to signal error
        /// when reading an image from file
        /// </summary>
        public const int ERR_READING_IMG = 32;
        /// <summary> 
        /// Constant describes delegate message to signal error
        /// when defining a graphical context
        /// </summary>
        public const int ERR_DEFINING_GC = 33;

        /// <summary> 
        /// Maximum number of HALCON objects that can be put on the graphics 
        /// stack without loss. For each additional object, the first entry 
        /// is removed from the stack again.
        /// </summary>
        private const int MAXNUMOBJLIST = 50;

        #endregion

        #region 私有变量
        private int stateView;
        private bool mousePressed = false;
        private double startX, startY;

        /// <summary>
        /// HALCON window
        /// </summary>
        private readonly HWindowControlWPF viewPort;

        /// <summary>
        /// /* dispROI is a flag to know when to add the ROI models to the 
        ///   paint routine and whether or not to respond to mouse events for 
        ///   ROI objects */
        /// </summary>
        private readonly int dispROI;

        /// <summary>
        /// Image coordinates, which describe the image part that is displayed in the HALCON window
        /// </summary>
        private double ImgY, ImgX, ImgHeight, ImgWidth;

        private HWindow ZoomWindow;
        private double zoomWndFactor;
        private readonly double zoomAddOn;
        private readonly int zoomWndSize;

        /// <summary> 
        /// List of HALCON objects to be drawn into the HALCON window. 
        /// The list shouldn't contain more than MAXNUMOBJLIST objects, 
        /// otherwise the first entry is removed from the list.
        /// </summary>
        private readonly List<HObject> HObjList = new List<HObject>(20);
        /// <summary>
        /// 显示消息列表
        /// </summary>
        private readonly List<MessageConfig> MessageList = new List<MessageConfig>();
        /// <summary>
        /// 显示文本列表
        /// </summary>
        private readonly List<TextConfig> TextList = new List<TextConfig>();
        /// <summary>
        /// 显示Region和XLD的列表
        /// </summary>
        private readonly List<HObjectConfig> HObjectList = new List<HObjectConfig>();

        #endregion

        #region ROI合并区域属性
        /// <summary>
        /// ROI合并的区域
        /// </summary>
        public HObject GatherRegion = new HObject();
        /// <summary>
        /// 合并ROI的数量，主要用于检测有无合并过区域
        /// </summary>
        public int GatherRegionCount;
        /// <summary>
        /// 指示是否显示合并ROI的区域
        /// </summary>
        public bool IsGatherRegionShow;
        /// <summary>
        /// 合并ROI的区域的显示线宽
        /// </summary>
        public int GatherRegionLineWidth = 3;
        /// <summary>
        /// 合并ROI的区域的显示颜色
        /// </summary>
        public Color GatherRegionColor = Colors.Blue;
        /// <summary>
        /// 合并ROI的区域的显示颜色代码
        /// </summary>
        public string GatherRegionColorStr;

        #endregion

        #region 公共属性
        /// <summary>
        ///  Basic parameters, like dimension of window and displayed image part
        /// </summary>
        public int ImageWidth;
        public int ImageHeight;
        /// <summary>
        /// 显示线程锁定标志
        /// </summary>
        public object LockDisplay = new object();
        /// <summary>
        /// 指示是否显示十字线
        /// </summary>
        public bool IsDisplayCross;
        /// <summary>
        /// Instance of ROIController, which manages ROI interaction
        /// </summary>
        public readonly ROIController RoiController;

        /// <summary>
        /// 被选中的ROI变化时发生
        /// </summary>
        public event EventHandler HWndActiveRoiChanged;

        #endregion

        #region 构造
        /// <summary> 
        /// Initializes the image dimension, mouse delegation, and the 
        /// graphical context setup of the instance.
        /// </summary>
        /// <param name="viewPort"> HALCON window </param>
        public ViewController(HWindowControlWPF viewPort)
        {
            this.viewPort = viewPort;
            this.RoiController = new ROIController(this);

            stateView = MODE_VIEW_NONE;

            zoomWndFactor = (double)ImageWidth / this.viewPort.ActualWidth;
            zoomAddOn = Math.Pow(0.9, 5);
            zoomWndSize = 150;

            dispROI = MODE_INCLUDE_ROI;

            this.viewPort.HMouseUp += ViewPort_HMouseUp;
            this.viewPort.HMouseDown += this.MouseDown;
            this.viewPort.HMouseMove += this.MouseMove;
            //建立鼠标离开控件的可见部分触发事件
            this.viewPort.MouseLeave += this.ViewPort_MouseLeave;
            //建立键盘按下触发事件
            this.viewPort.KeyDown += this.ViewPort_KeyDown;

            //设置窗口操作模式为移动
            SetViewState(ViewController.MODE_VIEW_MOVE);
        }

        #endregion

        #region 设置与重置
        /// <summary>
        /// Read dimensions of the image to adjust own window settings
        /// </summary>
        /// <param name="image">HALCON image</param>
        private void SetImagePart(HImage image)
        {
            string s;
            int w, h;

            image.GetImagePointer1(out s, out w, out h);
            SetImagePart(0, 0, h, w);
        }
        /// <summary>
        /// Adjust window settings by the values supplied for the left 
        /// upper corner and the right lower corner
        /// </summary>
        /// <param name="x">y coordinate of left upper corner</param>
        /// <param name="y">x coordinate of left upper corner</param>
        /// <param name="h">y coordinate of right lower corner</param>
        /// <param name="w">x coordinate of right lower corner</param>
        private void SetImagePart(int x, int y, int h, int w)
        {
            ImgX = x;
            ImgY = y;
            ImgWidth = ImageWidth = w;
            ImgHeight = ImageHeight = h;

            var rect = viewPort.ImagePart;
            rect.X = (int)ImgX;
            rect.Y = (int)ImgY;
            rect.Height = (int)ImageHeight;
            rect.Width = (int)ImageWidth;
            viewPort.ImagePart = rect;
        }

        /// <summary>
        /// Sets the view mode for mouse events in the HALCON window
        /// (zoom, move, magnify or none).
        /// </summary>
        /// <param name="mode">One of the MODE_VIEW_* constants</param>
        public void SetViewState(int mode)
        {
            stateView = mode;

            if (RoiController != null) RoiController.ResetROI();
        }

        /// <summary>
        /// Resets all parameters that concern the HALCON window display 
        /// setup to their initial values and clears the ROI list.
        /// </summary>
        public void ResetAll()
        {
            ResetWindow();
            if (RoiController != null) RoiController.Reset();
        }
        public void ResetWindow()
        {
            ImgY = 0;
            ImgX = 0;
            ImgHeight = ImageHeight;
            ImgWidth = ImageWidth;

            zoomWndFactor = (double)ImageWidth / viewPort.ActualWidth;

            var rect = viewPort.ImagePart;
            rect.X = (int)ImgX;
            rect.Y = (int)ImgY;
            rect.Width = (int)ImageWidth;
            rect.Height = (int)ImageHeight;
            viewPort.ImagePart = rect;
        }

        #endregion

        #region 鼠标点击与缩放
        /// <summary>
        /// 鼠标离开控件的可见部分触发事件
        /// </summary>
        private void ViewPort_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //取消鼠标被按下指示
            mousePressed = false;
        }
        /// <summary>
        /// 键盘按下触发事件
        /// </summary>
        private void ViewPort_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                RoiController.RemoveActive();
                //将事件传递的方法
                HWndActiveRoiChanged?.Invoke(this, new EventArgs());
            }
        }

        private void MouseDown(object sender, HMouseEventArgsWPF e)
        {
            mousePressed = true;
            int activeROIidx = -1;
            double scale;

            if (RoiController != null && (dispROI == MODE_INCLUDE_ROI))
            {
                activeROIidx = RoiController.MouseDownAction(e.X, e.Y);
            }

            if (activeROIidx == -1)
            {
                switch (stateView)
                {
                    case MODE_VIEW_MOVE:
                        startX = e.X;
                        startY = e.Y;
                        break;
                    case MODE_VIEW_ZOOM:
                        if (e.Button == MouseButton.Left)
                            scale = 0.9;
                        else
                            scale = 1 / 0.9;
                        ZoomImage(e.X, e.Y, scale);
                        break;
                    case MODE_VIEW_NONE:
                        break;
                    case MODE_VIEW_ZOOMWINDOW:
                        ActivateZoomWindow((int)e.X, (int)e.Y);
                        break;
                    default:
                        break;
                }
            }
            //end of if
        }
        private void ActivateZoomWindow(int X, int Y)
        {
            double posX, posY;
            int zoomZone;

            if (ZoomWindow != null)
                ZoomWindow.Dispose();

            HOperatorSet.SetSystem("border_width", 10);
            ZoomWindow = new HWindow();

            posX = ((X - ImgX) / (ImgWidth - ImgX)) * viewPort.ActualWidth;
            posY = ((Y - ImgY) / (ImgHeight - ImgY)) * viewPort.ActualHeight;

            zoomZone = (int)((zoomWndSize / 2) * zoomWndFactor * zoomAddOn);
            ZoomWindow.OpenWindow((int)posY - (zoomWndSize / 2), (int)posX - (zoomWndSize / 2),
                                   zoomWndSize, zoomWndSize,
                                   viewPort.HalconID, "visible", "");
            ZoomWindow.SetPart(Y - zoomZone, X - zoomZone, Y + zoomZone, X + zoomZone);
            Repaint(ZoomWindow);
            ZoomWindow.SetColor("black");
        }

        private void ViewPort_HMouseUp(object sender, HMouseEventArgsWPF e)
        {
            mousePressed = false;

            if (RoiController != null
                && (RoiController.ActiveROIidx != -1)
                && (dispROI == MODE_INCLUDE_ROI))
            {
                //计算ROI合并区域，并判断是否刷新显示
                OperationGatherRegion();

                //将事件传递的方法
                HWndActiveRoiChanged?.Invoke(this, new EventArgs());

            }
            else if (stateView == MODE_VIEW_ZOOMWINDOW)
            {
                ZoomWindow.Dispose();
            }
        }

        private void MouseMove(object sender, HMouseEventArgsWPF e)
        {
            double motionX, motionY;
            double posX, posY;
            double zoomZone;

            //判断是否有待生成的ROI
            if (RoiController.ROI != null)
            {
                RoiController.ROI.CreateROI(e.X, e.Y); //设置ROI位置
                Repaint();  //刷新显示
            }

            if (!mousePressed) return;

            if (RoiController != null && (RoiController.ActiveROIidx != -1) && (dispROI == MODE_INCLUDE_ROI))
            {
                RoiController.MouseMoveAction(e.X, e.Y);
            }
            else if (stateView == MODE_VIEW_MOVE)
            {
                motionX = ((e.X - startX));
                motionY = ((e.Y - startY));

                if (((int)motionX != 0) || ((int)motionY != 0))
                {
                    MoveImage(motionX, motionY);
                    startX = e.X - motionX;
                    startY = e.Y - motionY;
                }
            }
            else if (stateView == MODE_VIEW_ZOOMWINDOW)
            {
                HSystem.SetSystem("flush_graphic", "false");
                ZoomWindow.ClearWindow();


                posX = ((e.X - ImgX) / (ImgWidth - ImgX)) * viewPort.ActualWidth;
                posY = ((e.Y - ImgY) / (ImgHeight - ImgY)) * viewPort.ActualHeight;
                zoomZone = (zoomWndSize / 2) * zoomWndFactor * zoomAddOn;

                ZoomWindow.SetWindowExtents((int)posY - (zoomWndSize / 2),
                                            (int)posX - (zoomWndSize / 2),
                                            zoomWndSize, zoomWndSize);
                ZoomWindow.SetPart((int)(e.Y - zoomZone), (int)(e.X - zoomZone),
                                   (int)(e.Y + zoomZone), (int)(e.X + zoomZone));
                Repaint(ZoomWindow);

                HSystem.SetSystem("flush_graphic", "true");
                ZoomWindow.DispLine(-100.0, -100.0, -100.0, -100.0);
            }
        }
        private void MoveImage(double motionX, double motionY)
        {
            ImgY += -motionY;
            ImgHeight += -motionY;

            ImgX += -motionX;
            ImgWidth += -motionX;

            var rect = viewPort.ImagePart;
            rect.X = (int)Math.Round(ImgX);
            rect.Y = (int)Math.Round(ImgY);
            viewPort.ImagePart = rect;

            Repaint();
        }

        /// <summary>
        /// Zooms the image by the value valF supplied by the GUI component
        /// </summary>
        public void ZoomByGUIHandle(double valF)
        {
            var x = (ImgX + (ImgWidth - ImgX) / 2);
            var y = (ImgY + (ImgHeight - ImgY) / 2);

            var prevScaleC = (double)((ImgWidth - ImgX) / ImageWidth);
            var scale = ((double)1.0 / prevScaleC * (100.0 / valF));

            ZoomImage(x, y, scale);
        }
        private void ZoomImage(double x, double y, double scale)
        {
            double lengthC, lengthR;
            double percentC, percentR;
            int lenC, lenR;

            percentC = (x - ImgX) / (ImgWidth - ImgX);
            percentR = (y - ImgY) / (ImgHeight - ImgY);

            lengthC = (ImgWidth - ImgX) * scale;
            lengthR = (ImgHeight - ImgY) * scale;

            ImgX = x - lengthC * percentC;
            ImgWidth = x + lengthC * (1 - percentC);

            ImgY = y - lengthR * percentR;
            ImgHeight = y + lengthR * (1 - percentR);

            lenC = (int)Math.Round(lengthC);
            lenR = (int)Math.Round(lengthR);

            var rect = viewPort.ImagePart;
            rect.X = (int)Math.Round(ImgX);
            rect.Y = (int)Math.Round(ImgY);
            rect.Width = (lenC > 0) ? lenC : 1;
            rect.Height = (lenR > 0) ? lenR : 1;
            viewPort.ImagePart = rect;

            zoomWndFactor *= scale;
            //repaint();
        }

        #endregion

        #region 刷新
        /// <summary>
        /// Triggers a repaint of the HALCON window
        /// </summary>
        public void Repaint()
        {
            Repaint(viewPort.HalconWindow);
        }
        /// <summary>
        /// Repaints the HALCON window 'window'
        /// </summary>
        private void Repaint(HWindow window)
        {
            //线程锁定
            lock (LockDisplay)
            {
                HObject obj;

                HSystem.SetSystem("flush_graphic", "false");
                window.ClearWindow();

                for (int i = 0; i < HObjList.Count; i++)
                {
                    obj = HObjList[i];
                    window.DispObj(obj);
                }

                //显示十字线
                if (IsDisplayCross) DisplayCross(window);

                //显示Region和XLD
                ShowHObjectList(window);

                //显示所有ROI的合并区域
                if (IsGatherRegionShow) DisplayGatherRegion(window);

                if (RoiController != null && (dispROI == MODE_INCLUDE_ROI))
                    RoiController.PaintData(window);

                //显示文本
                ShowTextList(window);

                //显示消息
                ShowMessageList(window);

                //显示待生成ROI的预览
                if (RoiController.ROI != null && RoiController.RoiDrawConfig.IsDrawPrepare)
                {
                    window.SetColor(RoiController.ActiveCol);  //设置颜色
                    window.SetLineStyle(new HTuple(new int[] { 2, 2 }));    //设置显示为虚线
                    RoiController.ROI.Draw(window);    //显示待生成的ROI
                }

                HSystem.SetSystem("flush_graphic", "true");

                window.SetColor("black");
                window.DispLine(-100.0, -100.0, -101.0, -101.0);
            }
        }

        #endregion

        #region 显示图像
        /// <summary>
        /// Clears all entries from the graphics stack 
        /// </summary>
        public void ClearList()
        {
            HObjList.Clear();
        }
        /// <summary>
        /// Adds an iconic object to the graphics stack similar to the way
        /// it is defined for the HDevelop graphics stack.
        /// </summary>
        /// <param name="obj">Iconic object</param>
        public void AddIconicVar(HObject obj)
        {
            if (obj == null) return;

            if (obj is HImage)
            {
                double r, c;
                int h, w, area;
                string s;

                area = ((HImage)obj).GetDomain().AreaCenter(out r, out c);
                ((HImage)obj).GetImagePointer1(out s, out w, out h);

                if (area == (w * h))
                {
                    ClearList();

                    if ((h != ImageHeight) || (w != ImageWidth))
                    {
                        ImageHeight = h;
                        ImageWidth = w;
                        zoomWndFactor = (double)ImageWidth / viewPort.ActualWidth;
                        SetImagePart(0, 0, h, w);
                    }
                }//if
            }//if
            HObjList.Add(obj);
            if (HObjList.Count > MAXNUMOBJLIST) HObjList.RemoveAt(1);
        }

        #endregion

        #region 新增功能-ROI合并区域
        /// <summary>
        /// 显示ROI合并的区域
        /// </summary>
        public void DisplayGatherRegion(HWindow window)
        {
            if (GatherRegionCount == 0) return;

            //显示线的格式为直线
            window.SetLineStyle(new HTuple());
            //显示颜色
            window.SetColor(GatherRegionColorStr);
            //显示线宽
            window.SetLineWidth(GatherRegionLineWidth);
            //显示区域
            window.DispObj(GatherRegion);
        }
        /// <summary>
        /// 计算ROI合并区域，并判断是否刷新显示（ROICircularArc和ROILine不参与）
        /// </summary>
        public void OperationGatherRegion()
        {
            //将ROI区域相加
            GatherRegionCount = 0;
            GatherRegion = new HObject();
            HOperatorSet.GenEmptyObj(out GatherRegion);
            foreach (ROI roi in RoiController.ROIList)
            {
                if (roi.ROIMergeType == ROIMergeType.Add)
                {
                    if (!(roi is ROICircularArc) && !(roi is ROILine))
                    {
                        HOperatorSet.Union2(GatherRegion, roi.GetRegion(), out GatherRegion);
                        GatherRegionCount++;
                    }
                }
            }
            //将ROI区域相减
            foreach (ROI roi in RoiController.ROIList)
            {
                if (roi.ROIMergeType == ROIMergeType.Sub)
                {
                    if (roi.ToString() != new ROICircularArc().ToString() && roi.ToString() != new ROILine().ToString())
                    {
                        HOperatorSet.Difference(GatherRegion, roi.GetRegion(), out GatherRegion);
                        GatherRegionCount++;
                    }
                }
            }
            if (GatherRegionCount != 0) Repaint();  //刷新显示
        }

        #endregion

        #region 新增功能-显示十字线
        /// <summary>
        /// 显示十字线
        /// </summary>
        public void DisplayCross(HWindow window)
        {
            //显示颜色
            window.SetColor("red");
            //显示线
            window.DispLine((double)ImageHeight / 2, 0, (double)ImageHeight / 2, ImageWidth);
            window.DispLine(0, (double)ImageWidth / 2, ImageHeight, (double)ImageWidth / 2);
        }

        #endregion

        #region 新增功能-Region和XLD显示
        /// <summary>
        /// 获取显示的Region或XLD列表
        /// </summary>
        public List<HObjectConfig> GetHObjectList()
        {
            return HObjectList;
        }
        /// <summary>
        /// 获取显示的Region或XLD（指定序号）
        /// </summary>
        public HObjectConfig GetHObject(int index)
        {
            return HObjectList[index];
        }
        /// <summary>
        /// 清空所有Region和XLD
        /// </summary>
        public void ClearHObject()
        {
            HObjectList.Clear();
        }
        /// <summary>
        /// 添加Region和XLD显示(Region和XLD，颜色，填充模式)
        /// </summary>
        public void AddHObject(string name, HObject hObject, Color color, DrawModelType drawModel)
        {
            HObject hobj = new HObject(hObject);
            HObjectList.Add(new HObjectConfig(name, hobj, color, drawModel));
        }
        public bool RemoveHObject(string name)
        {
            var count = HObjectList.RemoveAll(c => c.Name == name);
            return count > 0;
        }
        /// <summary>
        /// 显示Region和XLD
        /// </summary>
        private void ShowHObjectList(HWindow window)
        {
            foreach (HObjectConfig hObjectConfig in HObjectList)
            {
                //区域填充模式
                window.SetDraw(hObjectConfig.DrawModelType.ToString());
                //显示颜色
                window.SetColor(hObjectConfig.ColorStr);
                //显示Region和XLD
                window.DispObj(hObjectConfig.HObject);
            }
        }

        #endregion

        #region 显示文本
        /// <summary>
        /// 清空所有文本
        /// </summary>
        public void ClearText()
        {
            TextList.Clear();
        }
        /// <summary>
        /// 添加文本显示
        /// </summary>
        public void AddText(string name, string text, Color color, int x, int y)
        {
            TextList.Add(new TextConfig(name, text, color, x, y));
        }
        public bool RemoveText(string name)
        {
            var count = TextList.RemoveAll(c => c.Name == name);
            return count > 0;
        }
        /// <summary>
        /// 显示文本
        /// </summary>
        private void ShowTextList(HWindow window)
        {
            foreach (TextConfig textConfig in TextList)
            {
                //显示颜色
                window.SetColor(textConfig.ColorStr);
                //显示位置
                window.SetTposition(textConfig.Y, textConfig.X);
                //显示文本
                window.WriteString(textConfig.Text);
            }
        }

        #endregion

        #region 在窗体显示消息框
        /// <summary>
        /// 清空所有消息
        /// </summary>
        public void ClearMessage()
        {
            MessageList.Clear();
        }
        public bool RemoveMessage(string name)
        {
            var count = MessageList.RemoveAll(c => c.Name == name);
            return count > 0;
        }
        /// <summary>
        /// 添加消息显示(消息，颜色，位置X，位置Y，绑定方位，同等位要求，是否显示边框)
        /// </summary>
        public void AddMessage(string name, string str, Color color, int x, int y, AnchorType anchor, CoordSystemType coord, bool isBox)
        {
            MessageList.Add(new MessageConfig(name, str, color, x, y, anchor, coord, isBox));
        }
        /// <summary>
        /// 显示消息
        /// </summary>
        private void ShowMessageList(HWindow window)
        {
            foreach (MessageConfig mes in MessageList)
            {
                if (mes.AnchorType == AnchorType.LeftTop) Disp_message(window, mes.Message, mes.CoordSystemType.ToString(), mes.Y, mes.X, mes.ColorStr, mes.IsBox);
                else Disp_continue_message(window, mes.Message, mes.CoordSystemType.ToString(), mes.Y, mes.X, mes.ColorStr, mes.IsBox);
            }
        }
        /// <summary>
        /// 在窗体显示消息框
        /// </summary>
        /// <param name="WindowHandle"></param>
        /// <param name="String"></param>
        /// <param name="coordSystem">0:以窗体为坐标  1:以图片为坐标</param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="color"></param>
        /// <param name="box">是否显示边框</param>
        private void Disp_message(HTuple WindowHandle, string String, string coordSystem, int Row, int Column, string Color, bool box)
        {
            HTuple hv_WindowHandle = WindowHandle;
            HTuple hv_String = String;
            HTuple hv_CoordSystem;
            hv_CoordSystem = coordSystem;
            HTuple hv_Row = Row;
            HTuple hv_Column = Column;
            HTuple hv_Color = Color;
            HTuple hv_Box = new HTuple(box.ToString().ToLower());

            // Local control variables 

            HTuple hv_Red, hv_Green, hv_Blue, hv_Row1Part;
            HTuple hv_Column1Part, hv_Row2Part, hv_Column2Part, hv_RowWin;
            HTuple hv_ColumnWin, hv_WidthWin, hv_HeightWin;
            HTuple hv_MaxAscent, hv_MaxDescent, hv_MaxWidth, hv_MaxHeight;
            HTuple hv_R1, hv_C1, hv_FactorRow;
            HTuple hv_FactorColumn, hv_Width;
            HTuple hv_Index, hv_Ascent, hv_Descent;
            HTuple hv_W, hv_H, hv_FrameHeight;
            HTuple hv_FrameWidth, hv_R2;
            HTuple hv_C2, hv_DrawMode, hv_Exception;
            HTuple hv_CurrentColor;

            HTuple hv_Color_COPY_INP_TMP = hv_Color.Clone();
            HTuple hv_Column_COPY_INP_TMP = hv_Column.Clone();
            HTuple hv_Row_COPY_INP_TMP = hv_Row.Clone();
            HTuple hv_String_COPY_INP_TMP = hv_String.Clone();

            // Initialize local and output iconic variables 

            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Column: The column coordinate of the desired text position
            //   If set to -1, a default value of 12 is used.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for each new textline.
            //Box: If set to 'true', the text is written within a white box.
            //
            //prepare window
            HOperatorSet.GetRgb(hv_WindowHandle, out hv_Red, out hv_Green, out hv_Blue);
            HOperatorSet.GetPart(hv_WindowHandle, out hv_Row1Part, out hv_Column1Part,
                out hv_Row2Part, out hv_Column2Part);
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_RowWin, out hv_ColumnWin,
                out hv_WidthWin, out hv_HeightWin);
            HOperatorSet.SetPart(hv_WindowHandle, 0, 0, hv_HeightWin - 1, hv_WidthWin - 1);
            //
            //default settings
            if (new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1)) != 0)
            {
                hv_Row_COPY_INP_TMP = 12;
            }
            if (new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1)) != 0)
            {
                hv_Column_COPY_INP_TMP = 12;
            }
            if (new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(new HTuple())) != 0)
            {
                hv_Color_COPY_INP_TMP = "";
            }
            //
            hv_String_COPY_INP_TMP = ("" + hv_String_COPY_INP_TMP + "").TupleSplit("\n");
            //
            //Estimate extentions of text depending on font size.
            HOperatorSet.GetFontExtents(hv_WindowHandle, out hv_MaxAscent, out hv_MaxDescent,
                out hv_MaxWidth, out hv_MaxHeight);
            if (new HTuple(hv_CoordSystem.TupleEqual("window")) != 0)
            {
                hv_R1 = hv_Row_COPY_INP_TMP.Clone();
                hv_C1 = hv_Column_COPY_INP_TMP.Clone();
            }
            else
            {
                //transform image to window coordinates
                hv_FactorRow = 1.0 * hv_HeightWin / (hv_Row2Part - hv_Row1Part + 1);
                hv_FactorColumn = (1.0 * hv_WidthWin) / ((hv_Column2Part - hv_Column1Part) + 1);
                hv_R1 = (hv_Row_COPY_INP_TMP - hv_Row1Part + 0.5) * hv_FactorRow;
                hv_C1 = (hv_Column_COPY_INP_TMP - hv_Column1Part + 0.5) * hv_FactorColumn;
            }
            //
            //display text box depending on text size
            if (new HTuple(hv_Box.TupleEqual("true")) != 0)
            {
                //calculate box extents
                hv_String_COPY_INP_TMP = " " + hv_String_COPY_INP_TMP + " ";
                hv_Width = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)(new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ) - 1); hv_Index = (int)hv_Index + 1)
                {
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                        hv_Index), out hv_Ascent, out hv_Descent, out hv_W, out hv_H);
                    hv_Width = hv_Width.TupleConcat(hv_W);
                }
                hv_FrameHeight = hv_MaxHeight * (new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                    ));
                hv_FrameWidth = new HTuple(0).TupleConcat(hv_Width).TupleMax();
                hv_R2 = hv_R1 + hv_FrameHeight;
                hv_C2 = hv_C1 + hv_FrameWidth;
                //display rectangles
                HOperatorSet.GetDraw(hv_WindowHandle, out hv_DrawMode);
                HOperatorSet.SetDraw(hv_WindowHandle, "fill");
                HOperatorSet.SetColor(hv_WindowHandle, "light gray");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1 + 3, hv_C1 + 3, hv_R2 + 3,
                    hv_C2 + 3);
                HOperatorSet.SetColor(hv_WindowHandle, "white");
                HOperatorSet.DispRectangle1(hv_WindowHandle, hv_R1, hv_C1, hv_R2, hv_C2);
                HOperatorSet.SetDraw(hv_WindowHandle, hv_DrawMode);
            }
            else if (new HTuple(hv_Box.TupleNotEqual("false")) != 0)
            {
                hv_Exception = "Wrong value of control parameter Box";
                throw new HalconException(hv_Exception);
            }
            //Write text.
            for (hv_Index = 0; (int)hv_Index <= (int)(new HTuple(hv_String_COPY_INP_TMP.TupleLength()
                ) - 1); hv_Index = (int)hv_Index + 1)
            {
                hv_CurrentColor = hv_Color_COPY_INP_TMP.TupleSelect(hv_Index % new HTuple(hv_Color_COPY_INP_TMP.TupleLength()));
                if (new HTuple(hv_CurrentColor.TupleNotEqual("")).TupleAnd(new HTuple(hv_CurrentColor.TupleNotEqual(
                    "auto"))) != 0)
                {
                    HOperatorSet.SetColor(hv_WindowHandle, hv_CurrentColor);
                }
                else
                {
                    HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
                }
                hv_Row_COPY_INP_TMP = hv_R1 + (hv_MaxHeight * hv_Index);
                HOperatorSet.SetTposition(hv_WindowHandle, hv_Row_COPY_INP_TMP, hv_C1);
                HOperatorSet.WriteString(hv_WindowHandle, hv_String_COPY_INP_TMP.TupleSelect(
                    hv_Index));
            }
            //reset changed window settings
            HOperatorSet.SetRgb(hv_WindowHandle, hv_Red, hv_Green, hv_Blue);
            HOperatorSet.SetPart(hv_WindowHandle, hv_Row1Part, hv_Column1Part, hv_Row2Part,
                hv_Column2Part);
        }
        /// <summary>
        /// 在窗体低右部为原点，显示消息框
        /// </summary>
        /// <param name="WindowHandle"></param>
        /// <param name="String"></param>
        /// <param name="coordSystem">0:以窗体为坐标  1:以图片为坐标</param>
        /// <param name="Row"></param>
        /// <param name="Column"></param>
        /// <param name="color"></param>
        /// <param name="Box">是否显示边框</param>
        private void Disp_continue_message(HTuple WindowHandle, string String, string coordSystem, int Row, int Column, string Color, bool Box)
        {
            HTuple hv_WindowHandle = WindowHandle;

            // Local control variables 

            HTuple hv_ContinueMessage, hv_Row, hv_Column;
            HTuple hv_Width, hv_Height, hv_Ascent, hv_Descent, hv_TextWidth;
            HTuple hv_TextHeight;

            // Initialize local and output iconic variables 

            //This procedure displays 'Press Run (F5) to continue' in the
            //lower right corner of the screen.
            //It uses the procedure disp_message.
            //
            //Input parameters:
            //WindowHandle: The window, where the text shall be displayed
            //Color: defines the text color.
            //   If set to '' or 'auto', the currently set color is used.
            //Box: If set to 'true', the text is displayed in a box.
            //
            hv_ContinueMessage = String;
            HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_Row, out hv_Column,
                out hv_Width, out hv_Height);
            HOperatorSet.GetStringExtents(hv_WindowHandle, (" " + hv_ContinueMessage) + " ",
                out hv_Ascent, out hv_Descent, out hv_TextWidth, out hv_TextHeight);

            Disp_message(hv_WindowHandle, hv_ContinueMessage, coordSystem, (hv_Height - hv_TextHeight) - Row, (hv_Width - hv_TextWidth) - Column, Color, Box);
        }

        #endregion
    }
}
