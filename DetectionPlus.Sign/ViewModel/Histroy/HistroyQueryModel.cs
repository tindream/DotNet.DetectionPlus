using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Sign
{
    public class HistroyQueryModel : ViewModelPlus
    {
        #region 属性
        private bool iStart = true;
        public bool IStart
        {
            get { return iStart; }
            set { iStart = value; RaisePropertyChanged(); }
        }
        private DateTime start = DateTime.Now;
        public DateTime Start
        {
            get { return start; }
            set { start = value; RaisePropertyChanged(); }
        }
        private bool iEnd = true;
        public bool IEnd
        {
            get { return iEnd; }
            set { iEnd = value; RaisePropertyChanged(); }
        }
        private DateTime end = DateTime.Now;
        public DateTime End
        {
            get { return end; }
            set { end = value; RaisePropertyChanged(); }
        }

        #endregion

        #region 命令
        private ICommand cancel;
        public ICommand Cancel
        {
            get
            {
                return cancel ?? (cancel = new RelayCommand<Window>(wd =>
                {
                    wd.DialogResult = false;
                }));
            }
        }
        private ICommand save;
        public ICommand Save
        {
            get
            {
                return save ?? (save = new RelayCommand<Window>(wd =>
                {
                    wd.DialogResult = true;
                }));
            }
        }

        #endregion

        public HistroyQueryModel() { }
    }
}