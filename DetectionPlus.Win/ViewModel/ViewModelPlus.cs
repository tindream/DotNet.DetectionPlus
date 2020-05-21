﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Windows.Input;

namespace DetectionPlus.Win.ViewModel
{
    /// <summary>
    /// ViewModelBase扩展
    /// </summary>
    public class ViewModelPlus : ViewModelBase
    {
        public void RaisePropertyChanged()
        {
            RaisePropertyChanged(Method.GetLastModelName());
        }
    }
}