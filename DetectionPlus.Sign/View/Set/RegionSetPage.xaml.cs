using GalaSoft.MvvmLight.Messaging;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace DetectionPlus.Sign
{
    /// <summary>
    /// 识别区域
    /// </summary>
    public partial class RegionSetPage : Page
    {
        public RegionSetPage()
        {
            InitializeComponent();
            this.Loaded += MonitorPage_Loaded;
        }
        private void MonitorPage_Loaded(object sender, RoutedEventArgs e)
        {
            frame1.Content = ViewlLocator.GetViewInstance<HToolPage>();
            frame1.Refresh();
        }
    }
}
