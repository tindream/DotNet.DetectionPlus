using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System;
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

        #endregion

        #region 命令
        private ICommand run;
        public ICommand Run
        {
            get
            {
                return run ?? (run = new RelayCommand<ButtonEXT>(btnRun =>
                {
                    if (Expand.Run(out int result, "B"))
                    {
                        Method.Toast(btnRun, "Hello, " + result);
                    }
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
            Method.Invoke(Config.Window, () =>
            {
                Image = Imaging.CreateBitmapSourceFromHBitmap(obj.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            });
        }

        #endregion

        public MonitorViewModel() { }
    }
}