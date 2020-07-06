using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

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
                        Config.Camera = new HKCamera();
                        { //关闭
                            Config.Camera.CameraStop();
                            Config.Camera.CameraClose();
                        }
                        {//曝光
                            Config.Camera.InitExposureTime = 1;
                            Config.Camera.ExposureTime = 2;
                        }
                        {//拍照
                            var bitmap = Config.Camera.CurrentImage();
                            if (bitmap != null)
                            {
                                //hWindowTool1.DisplayImage(bitmap);
                                //ReadRegion();
                            }
                        }
                    }
                }));
            }
        }

        #endregion

        public MonitorViewModel() { }
    }
}