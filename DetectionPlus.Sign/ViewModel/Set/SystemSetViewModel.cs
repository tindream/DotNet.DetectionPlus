using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
        private ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand<ListViewEXT>(listView1 =>
                {
                    if (listView1.SelectedItem is IListView info)
                    {
                        switch (info.Text)
                        {
                            case "Save":
                                Config.Admin.Expand = this.Info.Expand;
                                DataService.Default.Update(nameof(Config.Admin.Expand));
                                Method.Toast(listView1, "保存成功");
                                break;
                        }
                        Messenger.Default.Send(new StatuMessage(info.Text));
                    }
                    listView1.SelectedIndex = -1;
                }));
            }
        }
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

        #endregion

        public SystemSetViewModel()
        {
            this.Info = Config.Admin.Clone();
        }
    }
}