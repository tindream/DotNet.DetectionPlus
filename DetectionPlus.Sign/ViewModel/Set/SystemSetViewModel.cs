using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Sign
{
    public class SystemSetViewModel : ViewModelPlus
    {
        #region 属性

        #endregion

        #region 命令
        private ICommand save;
        public ICommand Save
        {
            get
            {
                return save ?? (save = new RelayCommand<ButtonEXT>(btnSave =>
                {
                    Method.Toast(btnSave, "保存成功");
                }));
            }
        }


        #endregion

        public SystemSetViewModel()
        {
        }
    }
}