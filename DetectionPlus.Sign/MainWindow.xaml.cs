using DetectionPlus.Camera;
using GalaSoft.MvvmLight.Messaging;
using HalconDotNet;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
                try
                {
                    DataService.Default.Load();
                    Config.Camera = new HKCamera()
                    {
                        CameraName = Config.Admin.CameraName,
                        InitExposureTime = Config.Admin.ExposureTime
                    };
                    Config.MacId = HardWareHelper.GetCpuId() + HardWareHelper.GetMainHardDiskId();
                    Config.MacId = EncryptHelper.MD5(Config.MacId + TConfig.Suffix);
                    Config.IListener = Config.Admin.Listener == EncryptHelper.MD5(Config.MacId + TConfig.Suffix);
                    Config.Manager = new DeviceManager(Config.Admin);
                    Config.Manager.ConnectEvent += delegate
                    {
                        Messenger.Default.Send(new StatuMessage($"{Config.Manager.Info.Host}{(Config.Manager.Connected ? "已连接" : "已断开")}"));
                    };
                    if (!Config.IListener)
                    {
                        Method.Toast(this, "未注册", true);
                    }
                    Method.BeginInvoke(this, () =>
                    {//预加载
                     //new HWindowControlWPF();
                    });
                }
                catch (Exception ex)
                {
                    ex.Log();
                    Messenger.Default.Send(new StatuMessage(ex.Message()));
                    Method.Show(this, ex.Message(), LeveType.Error);
                }
            }, () =>
            {
                Messenger.Default.Send(new StatuMessage("加载完成"));
                frame.Content = ViewlLocator.GetViewInstance<MonitorPage>();
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
