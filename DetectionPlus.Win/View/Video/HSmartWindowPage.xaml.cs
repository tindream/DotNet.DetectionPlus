using HalconDotNet;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DetectionPlus.Win
{
    /// <summary>
    /// HalconWindowPage.xaml 的交互逻辑
    /// </summary>
    public partial class HSmartWindowPage : Page
    {
        HObject ho_Image = null;
        HTuple hv_AcqHandle = null;
        Thread showThread;

        public HSmartWindowPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//开始
        {
            showThread = new Thread(showFrame);
            showThread.Start();
        }

        private void button2_Click(object sender, EventArgs e)//停止
        {
            showThread.Abort();
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);
        }

        private void showFrame()//采集
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.OpenFramegrabber("DirectShow", 1, 1, 0, 0, 0, 0, "default", 8, "gray",
                -1, "false", "default", "[0] ", 0, -1, out hv_AcqHandle);
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
            while ((int)(1) != 0)
            {
                ho_Image.Dispose();
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                HOperatorSet.DispObj(ho_Image, hWindowControl1.HalconWindow);
            }
        }
    }
}
