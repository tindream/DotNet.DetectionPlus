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
    /// 显示图像
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow(string file)
        {
            InitializeComponent();
            this.Title = file;
            hWindowTool.LoadImage(file);
        }
    }
}
