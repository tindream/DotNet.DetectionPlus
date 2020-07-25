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
    public class HToolViewModel : ViewModelPlus
    {
        #region 消息
        private void Save(DependencyObject obj)
        {
            Method.Invoke(obj, () =>
            {
                if (Method.Find(obj, out HWindowTool.HWindowTool hWindowTool))
                {
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
                }
            });
        }
        private void Ring(DependencyObject obj)
        {
            Method.Invoke(obj, () =>
            {
                if (Method.Find(obj, out HWindowTool.HWindowTool hWindowTool))
                {
                    var list = hWindowTool.ViewController.RoiController.ROIList;
                    list.RemoveAll(c => c is ROICircleRing);
                    hWindowTool.SetROIShape(new ROICircleRing());
                }
            });
        }
        private void Model(DependencyObject obj)
        {
            Method.Invoke(obj, () =>
            {
                if (Method.Find(obj, out HWindowTool.HWindowTool hWindowTool))
                {
                    var list = hWindowTool.ViewController.RoiController.ROIList;
                    list.RemoveAll(c => c is ROIRectangle2);
                    hWindowTool.SetROIShape(new ROIRectangle2());
                }
            });
        }
        private void Open(DependencyObject obj)
        {
            Method.Invoke(obj, () =>
            {
                if (Method.Find(obj, out HWindowTool.HWindowTool hWindowTool))
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
                }
            });
        }
        private void Reset(DependencyObject obj)
        {
            Method.Invoke(obj, () =>
            {
                if (Method.Find(obj, out HWindowTool.HWindowTool hWindowTool))
                {
                    hWindowTool.ResetWindow();
                }
            });
        }
        private void Picture(DependencyObject obj)
        {
            Method.Progress(obj, () =>
            {
                var bitmap = Config.Camera.CurrentImage();
                if (bitmap == null)
                {
                    Method.Toast(obj, "拍照失败");
                    return;
                }
                Method.Invoke(obj, () =>
                {
                    if (Method.Find(obj, out HWindowTool.HWindowTool hWindowTool))
                    {
                        hWindowTool.DisplayImage(bitmap);
                        ReadRegion(hWindowTool);
                        //Image = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                });
            });
        }
        private void ReadRegion(HWindowTool.HWindowTool hWindowTool)
        {
            string regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".Ring.reg");
            HalconHelper.ReadRegion(regionPath, hWindowTool.HalconWindow);
            regionPath = Path.Combine(Config.Template, Config.Admin.CameraName + ".Rect.reg");
            HalconHelper.ReadRegion(regionPath, hWindowTool.HalconWindow);
        }

        #endregion

        public HToolViewModel()
        {
            this.MessengerInstance.Register<PictureMessage>(this, msg => Picture(msg.Obj));
            this.MessengerInstance.Register<ResetMessage>(this, msg => Reset(msg.Obj));
            this.MessengerInstance.Register<OpenMessage>(this, msg => Open(msg.Obj));
            this.MessengerInstance.Register<ModelMessage>(this, msg => Model(msg.Obj));
            this.MessengerInstance.Register<RingMessage>(this, msg => Ring(msg.Obj));
            this.MessengerInstance.Register<SaveMessage>(this, msg => Save(msg.Obj));
        }
    }
}