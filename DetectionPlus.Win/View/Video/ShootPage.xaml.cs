using CommonServiceLocator;
using DetectionPlus.Win.ViewModel;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DetectionPlus.Win
{
    /// <summary>
    /// ShootPage.xaml 的交互逻辑
    /// </summary>
    public partial class ShootPage : Page
    {
        public ShootPage()
        {
            InitializeComponent();
            frame3.Content = ViewlLocator.GetViewInstance<ShootOnePage>($"{TMethod.Random()}-{ViewModelLocator.Default.Shoot.CarameList[0].Content}");
        }
    }
}
