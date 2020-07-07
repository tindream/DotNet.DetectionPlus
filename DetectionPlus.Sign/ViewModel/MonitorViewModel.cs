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
                return run ?? (run = new RelayCommand<Page>(pg =>
                {
                    if (Expand.Run(out int result, "B"))
                    {
                        Method.Toast(pg, "Hello, " + result);
                    }
                    if (Config.Camera.CameraName == null) Config.Camera.CameraName = "ABC";
                    Method.Progress(pg, () =>
                    {
                        if (!Config.Camera.IsOpen)
                        {
                            Config.Camera.Connect();
                            Config.Camera.ScreenEvent -= Camera_ScreenEvent;
                            Config.Camera.ScreenEvent += Camera_ScreenEvent;
                        }
                        Config.Camera.SetTriggerMode(false);//软触发
                        Config.Camera.ContinueShot();
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