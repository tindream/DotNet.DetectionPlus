﻿using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            DebugShow();
        }
        [Conditional("DEBUG")]
        private void DebugShow()
        {
            this.WindowState = WindowState.Normal;
            this.ResizeMode = ResizeMode.CanResize;
        }
    }
}
