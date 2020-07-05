using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Reflection;
using System.Windows.Input;

namespace DetectionPlus.Sign
{
    public class CameraSetViewModel : ViewModelPlus
    {
        #region 属性
        public string Text { get { return Config.Text; } }
        private string desc = "准备就绪";
        public string Desc
        {
            get { return desc; }
            set { desc = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private ICommand about;
        public ICommand About
        {
            get
            {
                return about ?? (about = new RelayCommand<WindowEXT>(wd =>
                {
                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    Method.Toast(wd, $"v{version}");
                }));
            }
        }
        private ICommand executeCommand;
        public ICommand ExecuteCommand
        {
            get
            {
                return executeCommand ?? (executeCommand = new RelayCommand<ButtonEXT>(btn =>
                {
                    if (Expand.Run(out int result, "B"))
                    {
                        Method.Toast(btn, "Hello, " + result);
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

        public CameraSetViewModel()
        {
        }
    }
}