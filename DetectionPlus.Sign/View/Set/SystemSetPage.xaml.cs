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

namespace DetectionPlus.Sign
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public partial class SystemSetPage : Page
    {
        public SystemSetPage()
        {
            InitializeComponent();
            this.Loaded += SystemSetPage_Loaded;
        }
        private void SystemSetPage_Loaded(object sender, RoutedEventArgs e)
        {
            tbInterval.Focus();
        }
    }
}
