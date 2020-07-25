﻿using GalaSoft.MvvmLight.Messaging;
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
    /// 相机设置
    /// </summary>
    public partial class CameraSetPage : Page
    {
        public CameraSetPage()
        {
            InitializeComponent();
            this.Loaded += MonitorPage_Loaded;
        }
        private void MonitorPage_Loaded(object sender, RoutedEventArgs e)
        {
            frame1.Content = ViewlLocator.GetViewInstance<HToolPage>();
            frame1.Refresh();
        }
    }
}
