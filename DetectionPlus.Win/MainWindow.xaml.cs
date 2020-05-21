using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DetectionPlus.Win
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowEXT
    {
        public MainWindow()
        {
            InitializeComponent();
            listView1.SelectionChanged += ListView1_SelectionChanged;
        }

        private void ListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView1.SelectedItem is IListViewInfo info)
            {
                switch (info.Content)
                {
                    case "教导":
                        new TeachWindow().ShowDialog();
                        listView1.SelectedIndex = -1;
                        break;
                }
            }
        }
    }
}
