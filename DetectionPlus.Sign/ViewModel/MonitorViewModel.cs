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
        private ICommand run;
        public ICommand Run
        {
            get
            {
                return run ?? (run = new RelayCommand<ButtonEXT>(btnRun =>
                {
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
                if (obj == null)
                { }
                else
                {
                    Total++;
                    Image = Imaging.CreateBitmapSourceFromHBitmap(obj.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    var iResult = Expand.Run(out int result, "B");
                    if (iResult) Success++;
                    var info = new HistroyInfo(iResult);
                    this.MessengerInstance.Send(new HistroyMessage(info));
                }

            });
        }

        #endregion

        public MonitorViewModel() { }
    }
}