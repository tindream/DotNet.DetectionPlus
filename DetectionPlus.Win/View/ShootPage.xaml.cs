using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
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
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Messenger.Default.Send(new ShootRefreshMessage() { Obj = listView1 });
        }
    }
}
