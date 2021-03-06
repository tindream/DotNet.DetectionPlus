﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DetectionPlus.Win
{
    public class ShootOneViewModel : ViewModelPlus
    {
        #region 属性
        private ImageSource image;
        public ImageSource Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        #endregion

        /// <summary>
        /// 二值化
        /// </summary>
        private void Binary(BinaryMessage binary)
        {
            var file = Path.Combine(Config.Images, "F1.png");
            Image = Method.Binary(file, 100 - binary.Value);
        }
        public ShootOneViewModel()
        {
            image = new BitmapImage(new Uri("pack://application:,,,/Images/F1.png"));
            this.MessengerInstance.Register<BinaryMessage>(this, Binary);
        }
    }
}