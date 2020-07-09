using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Sign
{
    public class SystemSetViewModel : ViewModelPlus
    {
        #region 属性
        public AdminInfo Info { get; set; }

        #endregion

        #region 命令
        private ICommand open;
        public ICommand Open
        {
            get
            {
                return open ?? (open = new RelayCommand<ButtonEXT>(btnSave =>
                {
                    var fbd = new FolderBrowserDialog
                    {
                        Description = "选择扩展dll所在目录(限根目录下文件夹)",
                    };
                    var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.Info.Expand);
                    if (!System.IO.Directory.Exists(path)) path = AppDomain.CurrentDomain.BaseDirectory;
                    fbd.SelectedPath = path;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        this.Info.Expand = new System.IO.DirectoryInfo(fbd.SelectedPath).Name;
                    }
                }));
            }
        }
        private ICommand save;
        public ICommand Save
        {
            get
            {
                return save ?? (save = new RelayCommand<ButtonEXT>(btnSave =>
                {
                    Config.Admin.Expand = this.Info.Expand;
                    DataService.Default.Update(nameof(Config.Admin.Expand));
                    Method.Toast(btnSave, "保存成功");
                }));
            }
        }

        #endregion

        public SystemSetViewModel()
        {
            this.Info = Config.Admin.Clone();
        }
    }
}