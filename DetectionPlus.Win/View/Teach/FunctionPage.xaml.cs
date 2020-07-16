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

namespace DetectionPlus.Win
{
    /// <summary>
    /// FunctionPage.xaml 的交互逻辑
    /// </summary>
    public partial class FunctionPage : Page
    {
        public FunctionPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            radioList.Children.Clear();
            var list = TMethod.List<FunctionType>();
            for (int i = 1; i < list.Count; i++)
            {
                var radio = new RadioButtonEXT();
                radio.Checked += Radio_Checked;
                radio.IsChecked = i == 1;
                radio.Content = list[i].Description();
                radioList.Children.Add(radio);
            }
        }
        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}
