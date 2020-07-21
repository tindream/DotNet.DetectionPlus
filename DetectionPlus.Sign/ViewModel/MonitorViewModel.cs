using DetectionPlus.Camera;
using DetectionPlus.HWindowTool;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using Paway.Helper;
using Paway.WPF;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DetectionPlus.Sign
{
    public class MonitorViewModel : ViewModelPlus
    {
        #region 属性
        private string desc;
        public string Desc
        {
            get { return desc; }
            set { desc = value; RaisePropertyChanged(); }
        }
        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }
        private int success;
        public int Success
        {
            get { return success; }
            set { success = value; RaisePropertyChanged(); }
        }
        private int total;
        public int Total
        {
            get { return total; }
            set { total = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private HWindowTool.HWindowTool hWindowTool;
        private ICommand run;
        public ICommand Run
        {
            get
            {
                return run ?? (run = new RelayCommand<ButtonEXT>(btnRun =>
                {
                    if (!Config.IListener)
                    {
                        Method.Toast(btnRun, "未注册", true);
                        return;
                    }
                    Method.Find(btnRun, out hWindowTool);
                    Method.Progress(btnRun, () =>
                    {
                        if (!Config.Camera.IsGrabbing)
                        {
                            if (!Config.Camera.IsOpen)
                            {
                                Config.Camera.Connect();
                                Config.Camera.ScreenEvent -= Camera_ScreenEvent;
                                Config.Camera.ScreenEvent += Camera_ScreenEvent;
                            }
                            Config.Camera.SetTriggerMode(Config.Admin.IsTrigger);
                            Config.Camera.ContinueShot();
                            Method.Invoke(btnRun, () =>
                            {
                                btnRun.BackgroundImage = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/stop.png")),
                                    new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/stop_s.png")));
                            });
                        }
                        else
                        {
                            if (Config.Camera.IsOpen)
                            {
                                Config.Camera.CameraStop();
                            }
                            Method.Invoke(btnRun, () =>
                            {
                                btnRun.BackgroundImage = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/run.png")),
                                    new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/run_s.png")));
                            });
                        }
                    });
                }));
            }
        }
        private void Camera_ScreenEvent(BitmapInfo obj)
        {
            Total++;
            var info = new HistroyInfo();
            if (obj == null)
            {
                info.Description = "拍照失败";
                Method.Toast(Config.Window, info.Description);
            }
            else
            {
                //Image = Imaging.CreateBitmapSourceFromHBitmap(obj.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //tool.DisplayImage(obj.Bitmap);
                try
                {
                    var iResult = Test(obj.Bitmap);
                    if (Config.Admin.Result == iResult)
                    {
                        var result = Config.Manager.Result();
                        info.Description = $"输出{(result ? "成功" : "失败")}: " + Convert.ToString(Config.Admin.Value, 2).PadLeft(8, '0');
                    }
                    if (iResult) Success++;
                    info.Result = iResult;
                }
                catch (Exception ex)
                {
                    info.Description = ex.Message();
                }
            }
            DataService.Default.Insert(info);
            Method.Invoke(Config.Window, () =>
            {
                this.MessengerInstance.Send(new HistroyMessage(info));
            });
        }
        private int index;
        private bool Test(System.Drawing.Bitmap bitmap)
        {
            string regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".Ring.reg");
            var ho_CheckRegion = HalconHelper.ReadRegion(regionPath, hWindowTool.HalconWindow, false);
            if (ho_CheckRegion == null) throw new WarningException("未设置区域");

            HTuple hv_ModelID = new HTuple();
            string modelPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".Rect.shm");
            HOperatorSet.ReadShapeModel(modelPath, out hv_ModelID);
            if (hv_ModelID == null) throw new WarningException("未设置模板");

            ModelConfig modelConfig = new ModelConfig();
            modelConfig.ModelRow = Config.Admin.CenterY;
            modelConfig.ModelColumn = Config.Admin.CenterX;
            modelConfig.ModelID = hv_ModelID;

            hWindowTool.ClearWindow();
            if (++index > 12) index = 1;
            var ho_Image = hWindowTool.GetImage(@"D:/Test/CCD1/" + index + ".bmp");
            //var ho_Image = tool.GetImage(bitmap);
            if (ho_Image == null) throw new WarningException("图像加载失败");

            hWindowTool.DisplayImage(ho_Image);
            bool result = ImageHandle.CheckFunction(hWindowTool, ho_Image, ho_CheckRegion, modelConfig, 0, 180, 0.8, 5000);
            return result;
        }

        private string ExpandPath
        {
            get
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.Admin.Expand);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        #endregion

        public MonitorViewModel() { }
    }
}