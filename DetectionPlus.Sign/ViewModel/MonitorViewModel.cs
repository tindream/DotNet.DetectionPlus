using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HalconDotNet;
using Paway.Helper;
using Paway.WPF;
using System;
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
        private HWindowTool.HWindowTool tool;
        private ICommand run;
        public ICommand Run
        {
            get
            {
                return run ?? (run = new RelayCommand<ButtonEXT>(btnRun =>
                {
                    Method.Find(btnRun, out tool);
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
        private bool Test(System.Drawing.Bitmap bitmap)
        {
            HObject ho_CheckRegion;
            HOperatorSet.GenEmptyObj(out ho_CheckRegion);
            ho_CheckRegion.Dispose();
            HOperatorSet.ReadRegion(out ho_CheckRegion, @"D:/Test/CircleRing.reg");
            HTuple hv_ModelID = new HTuple();
            HOperatorSet.ReadShapeModel(@"D:/Test/Model.shm", out hv_ModelID);
            ModelConfig modelConfig = new ModelConfig();
            modelConfig.ModelRow = 569.5;
            modelConfig.ModelColumn = 913.5;
            modelConfig.ModelID = hv_ModelID;

            //var ho_Image = tool.GetImage(bitmap);
            var ho_Image = tool.GetImage(@"D:/Test/CCD1/1.bmp");
            tool.DisplayImage(ho_Image);
            if (ho_Image == null) return false;
            bool b = ImageHandle.CheckFunction(tool, ho_Image, ho_CheckRegion, modelConfig, 0, 180, 0.8, 5000);
            return b;

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