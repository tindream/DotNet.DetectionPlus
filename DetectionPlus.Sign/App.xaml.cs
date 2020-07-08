using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Sign
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log4net.Config.XmlConfigurator.Configure();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            $"v{version} ({Environment.MachineName})".Log();

            DataService.Default.Init();
        }
    }
}
