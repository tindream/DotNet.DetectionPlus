using DetectionPlus.Camera;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;

namespace DetectionPlus.Sign
{
    public class CameraSetViewModel : ViewModelPlus
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
        private ICommand connect;
        public ICommand Connect
        {
            get
            {
                return connect ?? (connect = new RelayCommand<Page>(pg =>
                {
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
                }));
            }
        }

        #endregion

        public CameraSetViewModel() { }
    }
}