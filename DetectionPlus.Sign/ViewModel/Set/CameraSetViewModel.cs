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
        private ICommand roi;
        public ICommand ROI
        {
            get
            {
                return roi ?? (roi = new RelayCommand<HWindowTool.HWindowTool>(hWindowTool =>
                {
                    //hWindowTool.AddRoi(new ROIRectangle1());
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
                        LoadImage(opnDlg.FileName, hWindowTool);
                        ReadShapeModel();
                    }
                }));
            }
        }
        private void LoadImage(string file, HWindowTool.HWindowTool hWindowTool)
        {
            if (!File.Exists(file)) return;
            HObject ho_ModelImage;
            HOperatorSet.ReadImage(out ho_ModelImage, file);
            hWindowTool.DisplayImage(ho_ModelImage);
            hWindowTool.Repaint(); //刷新显示
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

                    var list = hWindowTool.ViewController.RoiController.ROIList;
                    if (list.Count > 0)
                    {
                        var roi = list[0];
                        string modelIDPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".shm");
                        string modelRegionPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".reg");
                        try
                        {
                            ShapeModel.Create_shape_model(roi.Image, roi.GetRegion(), modelIDPath, modelRegionPath);
                        }
                        catch (Exception ex)
                        {
                            ex.Log();
                            Method.Toast(hWindowTool, ex.Message(), true);
                            return;
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
                            ReadShapeModel();
                            //Image = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        });
                    });
                }));
            }
        }
        private void ReadShapeModel()
        {
            string modelIDPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".shm");
            if (File.Exists(modelIDPath))
            {
                HTuple hv_ModelID;
                HOperatorSet.ReadShapeModel(modelIDPath, out hv_ModelID);
            }
        }

        #endregion

        public CameraSetViewModel() { }
    }
}