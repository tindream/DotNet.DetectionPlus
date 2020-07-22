using DetectionPlus.Camera;
using DetectionPlus.HWindowTool;
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
using System.Windows.Forms;
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
        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private ICommand reset;
        public ICommand Reset
        {
            get
            {
                return reset ?? (reset = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    hWindowTool.ResetWindow();
                    //hWindowTool.ClearWindow();
                    //hWindowTool.Repaint(); //刷新显示
                }));
            }
        }
        private ICommand rect;
        public ICommand Rect
        {
            get
            {
                return rect ?? (rect = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    var list = hWindowTool.ViewController.RoiController.ROIList;
                    list.RemoveAll(c => c is ROIRectangle2);
                    hWindowTool.SetROIShape(new ROIRectangle2());
                }));
            }
        }
        private ICommand ring;
        public ICommand Ring
        {
            get
            {
                return ring ?? (ring = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    var list = hWindowTool.ViewController.RoiController.ROIList;
                    list.RemoveAll(c => c is ROICircleRing);
                    hWindowTool.SetROIShape(new ROICircleRing());
                }));
            }
        }
        private ICommand open;
        public ICommand Open
        {
            get
            {
                return open ?? (open = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    var opnDlg = new OpenFileDialog
                    {
                        Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;" +
                        "*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf",
                        Title = "打开图像文件",
                        ShowHelp = true,
                        Multiselect = false
                    };
                    if (opnDlg.ShowDialog() == DialogResult.OK)
                    {
                        hWindowTool.LoadImage(opnDlg.FileName);
                        ReadRegion(hWindowTool);
                    }
                }));
            }
        }

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
                            Config.Camera.SetTriggerMode(Config.Admin.IsTrigger);
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
                return save ?? (save = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    DataService.Default.Update(nameof(Config.Admin.CameraName));
                    DataService.Default.Update(nameof(Config.Admin.IsTrigger));
                    DataService.Default.Update(nameof(Config.Admin.ExposureTime));

                    foreach (var item in hWindowTool.ViewController.RoiController.ROIList)
                    {
                        if (item is ROICircleRing ring)
                        {
                            try
                            {
                                string regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + $".Ring.reg");
                                HalconHelper.WriteRegion(item.GetRegion(), regionPath);
                            }
                            catch (Exception ex)
                            {
                                ex.Log();
                                Method.Toast(hWindowTool, ex.Message(), true);
                                return;
                            }
                        }
                        if (item is ROIRectangle2)
                        {
                            Config.Admin.CenterX = item.GetCenter().X;
                            Config.Admin.CenterY = item.GetCenter().Y;
                            DataService.Default.Update(nameof(Config.Admin.CenterX));
                            DataService.Default.Update(nameof(Config.Admin.CenterY));

                            string regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + $".Rect.reg");
                            string modelPath = Path.Combine(Config.Template, Config.Admin.CameraName + $".Rect.shm");
                            try
                            {
                                HalconHelper.CreateShapModel(item.Image, item.GetRegion(), regionPath, modelPath);
                            }
                            catch (Exception ex)
                            {
                                ex.Log();
                                Method.Toast(hWindowTool, ex.Message(), true);
                                return;
                            }
                        }
                    }
                    Method.Toast(hWindowTool, "保存成功");
                }));
            }
        }
        private ICommand picture;
        public ICommand Picture
        {
            get
            {
                return picture ?? (picture = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    Method.Progress(hWindowTool, () =>
                    {
                        var bitmap = Config.Camera.CurrentImage();
                        if (bitmap == null)
                        {
                            Method.Toast(hWindowTool, "拍照失败");
                            return;
                        }
                        Method.Invoke(hWindowTool, () =>
                        {
                            hWindowTool.DisplayImage(bitmap);
                            ReadRegion(hWindowTool);
                            //Image = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        });
                    });
                }));
            }
        }
        private void ReadRegion(HWindowTool.HWindowTool hWindowTool)
        {
            string regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".Ring.reg");
            HalconHelper.ReadRegion(regionPath, hWindowTool.HalconWindow);
            regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".Rect.reg");
            HalconHelper.ReadRegion(regionPath, hWindowTool.HalconWindow);
        }

        #endregion

        public CameraSetViewModel() { }
    }
}