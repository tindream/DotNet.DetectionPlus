﻿using GalaSoft.MvvmLight.Messaging;
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

namespace DetectionPlus.Sign
{
    /// <summary>
    /// 结果输出-通讯设置
    /// </summary>
    public partial class ResultSetPage : Page
    {
        public ResultSetPage()
        {
            InitializeComponent();
            this.Loaded += SystemSetPage_Loaded;
        }
        private void SystemSetPage_Loaded(object sender, RoutedEventArgs e)
        {
            tbAddress.Focus();
            Messenger.Default.Send(new CommInitMessage() { Obj = listView1 });
        }
    }
}
