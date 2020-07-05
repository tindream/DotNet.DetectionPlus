using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace DetectionPlus.Sign
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelPlus
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
        private ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand<ListViewEXT>(listView1 =>
                {
                    if (listView1.SelectedItem is IListView info)
                    {
                        switch (info.Text)
                        {
                            case "教导":
                                break;
                            case "取相":
                                break;
                        }
                        listView1.SelectedIndex = -1;
                    }
                }));
            }
        }
        private ICommand cameraSet;
        public ICommand CameraSet
        {
            get
            {
                return cameraSet ?? (cameraSet = new RelayCommand<Frame>(frame =>
                {
                    frame.Content = ViewlLocator.GetViewInstance<CameraSetPage>();
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

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }
    }
}