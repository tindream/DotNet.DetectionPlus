using DetectionPlus.Camera;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowEXT
    {
        public MainWindow()
        {
            InitializeComponent();
            Config.Window = this;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Method.Progress(this, () =>
            {
                Config.Camera = new HKCamera();
            });
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (Config.Camera != null)
            {
                Method.ProgressSync(Config.Window, () =>
                {
                    if (Config.Camera.IsOpen)
                    {
                        Config.Camera.CameraStop();
                    }
                    Config.Camera.CameraClose();
                });
            }
            base.OnClosing(e);
        }
    }
}
