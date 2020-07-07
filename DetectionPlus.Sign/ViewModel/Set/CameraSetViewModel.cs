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
    public class CameraSetViewModel : ViewModelPlus
    {
        #region 属性
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }
        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private ICommand valueChanged;
        public ICommand ValueChanged
        {
            get
            {
                return valueChanged ?? (valueChanged = new RelayCommand<SliderEXT>(slider =>
                {
                    Method.Progress(slider, () =>
                    {
                        //曝光
                        var value = (float)(slider.Value * 500);
                        Config.Camera.InitExposureTime = value;
                        Config.Camera.ExposureTime = value;
                    });
                }));
            }
        }
        private ICommand connect;
        public ICommand Connect
        {
            get
            {
                return connect ?? (connect = new RelayCommand<ButtonEXT>(btnConnect =>
                {
                    Method.Progress(btnConnect, () =>
                    {
                        Config.Camera.CameraName = Name;
                        if (!Config.Camera.IsOpen)
                        {
                            Config.Camera.Connect();
                            Method.Invoke(btnConnect, () =>
                            {
                                btnConnect.BackgroundImage = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/disconnect.png")),
                                    new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/disconnect_s.png")));
                            });
                        }
                        else
                        {
                            Config.Camera.CameraStop();
                            Config.Camera.CameraClose();
                            Method.Invoke(btnConnect, () =>
                            {
                                btnConnect.BackgroundImage = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/connect.png")),
                                    new BitmapImage(new Uri("pack://application:,,,/DetectionPlus.Sign;component/Images/connect_s.png")));
                            });
                        }
                    });
                }));
            }
        }
        private ICommand picture;
        public ICommand Picture
        {
            get
            {
                return picture ?? (picture = new RelayCommand<Page>(pg =>
                {
                    Method.Progress(pg, () =>
                    {
                        var bitmap = Config.Camera.CurrentImage();
                        Method.Invoke(pg, () =>
                        {
                            Image = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        });
                    });
                }));
            }
        }

        #endregion

        public CameraSetViewModel() { }
    }
}