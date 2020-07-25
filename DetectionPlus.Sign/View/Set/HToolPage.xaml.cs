using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// 图像显示
    /// </summary>
    public partial class HToolPage : Page
    {
        public HToolPage()
        {
            InitializeComponent();
            this.Loaded += HToolPage_Loaded;
        }
        private void HToolPage_Loaded(object sender, RoutedEventArgs e)
        {
            Method.DoEvents();
            Method.BeginInvoke(this, () =>
            {
                if (hWindowTool.HalconWindow.Handle != (IntPtr)0)
                {
                    Method.BeginInvoke(this, () => { hWindowTool.Repaint(); });
                }
            });
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            hWindowTool.Width = hWindowTool.ActualHeight * 4 / 3;
        }
    }
}
