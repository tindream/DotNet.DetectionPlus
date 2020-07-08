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
        public AdminInfo Admin { get { return Config.Admin; } }
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
                    //曝光
                    var value = (float)slider.Value;
                    Config.Camera.InitExposureTime = value;
                    Config.Camera.ExposureTime = value;
                    Config.Admin.ExposureTime = value;
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
                        Config.Camera.CameraName = Config.Admin.CameraName;
                        if (!Config.Camera.IsOpen)
                        {
                            Config.Camera.Connect();
                            Config.Admin.ExposureTime = Config.Camera.ExposureTime;
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
        private ICommand save;
        public ICommand Save
        {
            get
            {
                return save ?? (save = new RelayCommand<Page>(pg =>
                {
                    DataService.Default.Update(nameof(Config.Admin.CameraName));
                    DataService.Default.Update(nameof(Config.Admin.IsTrigger));
                    DataService.Default.Update(nameof(Config.Admin.ExposureTime));
                    Method.Toast(pg, "保存成功");
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
                        if (bitmap == null)
                        {
                            Method.Toast(pg, "拍照失败");
                            return;
                        }
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