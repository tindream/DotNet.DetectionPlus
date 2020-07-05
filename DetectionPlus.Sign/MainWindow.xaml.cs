using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DetectionPlus.dll");
            var a = Assembly.LoadFile(file);
            var b = a.GetTypes();
            var c1 = b.Where(c => c.IsInterface && c.Name == nameof(ISign)).FirstOrDefault();
            if (c1 != null)
            {
                var c2 = b.Where(c => c1.IsAssignableFrom(c)).FirstOrDefault();
                if (c2 != null)
                {
                    var sign = Activator.CreateInstance(c2);
                    var r = Method.ExecuteMethod(sign, nameof(ISign.Result), out object result, 4);
                    var s = sign as ISign;
                }
            }
        }
    }
    public interface ISign
    {
        int Result(int i);
    }
}
