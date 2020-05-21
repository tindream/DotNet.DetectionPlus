using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Win.ViewModel
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
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //Colors.Transparent
        }

        private List<MenuButtonModel> menuList;
        public List<MenuButtonModel> MenuList
        {
            get
            {
                if (menuList == null) menuList = new List<MenuButtonModel>();
                menuList.Add(new MenuButtonModel("教导")
                {
                    Image = new ImageEXT(@"pack://application:,,,/DetectionPlus.Win;component/Images/1.png", @"pack://application:,,,/DetectionPlus.Win;component/Images/1_1.png")
                });
                menuList.Add(new MenuButtonModel("取相")
                {
                    Image = new ImageEXT(@"pack://application:,,,/DetectionPlus.Win;component/Images/1.png", @"pack://application:,,,/DetectionPlus.Win;component/Images/1_1.png")
                });
                menuList.Add(new MenuButtonModel("检测")
                {
                    Image = new ImageEXT(@"pack://application:,,,/DetectionPlus.Win;component/Images/1.png", @"pack://application:,,,/DetectionPlus.Win;component/Images/1_1.png")
                });
                menuList.Add(new MenuButtonModel("测试")
                {
                    Image = new ImageEXT(@"pack://application:,,,/DetectionPlus.Win;component/Images/1.png", @"pack://application:,,,/DetectionPlus.Win;component/Images/1_1.png")
                });
                menuList.Add(new MenuButtonModel("设置")
                {
                    Image = new ImageEXT(@"pack://application:,,,/DetectionPlus.Win;component/Images/1.png", @"pack://application:,,,/DetectionPlus.Win;component/Images/1_1.png")
                });
                return menuList;
            }
        }

        private string title = "Hello MVVMLight,Hello C#";
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }

        private ICommand m_ChangeTitleCmd;
        public ICommand ChangeTitleCmd
        {
            get
            {
                return m_ChangeTitleCmd ?? (m_ChangeTitleCmd = new RelayCommand(UpdateTitle));
            }
        }
        private void UpdateTitle()
        {
            Title = "chanage title command";
        }
    }
}