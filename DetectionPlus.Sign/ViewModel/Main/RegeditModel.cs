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
    public class RegeditModel : ViewModelPlus
    {
        #region 属性
        public string Machine
        {
            get { return Config.MacId; }
        }
        private string regedit;
        public string Regedit
        {
            get { return regedit; }
            set { regedit = value; RaisePropertyChanged(); }
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
                    if (Regedit == EncryptHelper.MD5(Config.MacId + TConfig.Suffix))
                    {
                        Config.IListener = true;
                        Config.Admin.Listener = Regedit;
                        DataService.Default.Update(nameof(Config.Admin.Listener));
                        wd.DialogResult = true;
                    }
                    else
                    {
                        Method.Toast(wd, "注册失败", true);
                    }
                }));
            }
        }

        #endregion

        public RegeditModel() { }
    }
}