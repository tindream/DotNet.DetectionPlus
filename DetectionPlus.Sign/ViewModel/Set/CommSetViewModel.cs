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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Sign
{
    public class CommSetViewModel : ViewModelPlus
    {
        #region 属性
        public AdminInfo Info { get; set; }

        #endregion

        #region 命令
        private ICommand selectionLineCommand;
        public ICommand SelectionLineCommand
        {
            get
            {
                return selectionLineCommand ?? (selectionLineCommand = new RelayCommand<SelectionChangedEventArgs>(e =>
                {
                    if (e.Source is ListViewEXT listView1)
                    {
                        var value = 0;
                        for (int i = 0; i < listView1.Items.Count; i++)
                        {
                            if (listView1.Items[i] is IListView item && item.IsSelected)
                            {
                                value += 1 << (item.Text.ToInt() - 1);
                            }
                        }
                        Info.Value = value;
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
                    if (Method.Find(btnSave, out TextBoxEXT textBox, "tbAddress"))
                    {
                        var value = textBox.Text.ToInt();
                        if (value < 0 || value > 255)
                        {
                            Method.Toast(btnSave, "地址范围0-255", true);
                            return;
                        }
                    }
                    if (Config.Admin.Result != Info.Result)
                    {
                        Config.Admin.Result = Info.Result;
                        DataService.Default.Update(nameof(Config.Admin.Result));
                    }
                    if (Config.Admin.Value != Info.Value)
                    {
                        Config.Admin.Value = Info.Value;
                        DataService.Default.Update(nameof(Config.Admin.Value));
                    }
                    if (Config.Admin.Address != Info.Address)
                    {
                        Config.Admin.Address = Info.Address;
                        DataService.Default.Update(nameof(Config.Admin.Address));
                    }
                    if (Config.Admin.Host != Info.Host)
                    {
                        Config.Admin.Host = Info.Host;
                        DataService.Default.Update(nameof(Config.Admin.Host));
                        Method.Progress(btnSave, () =>
                        {
                            Config.Manager.Update(Config.Admin);
                        });
                    }
                    Method.Toast(btnSave, "保存成功");
                }));
            }
        }

        #endregion

        #region 消息
        private void CommInit(DependencyObject obj)
        {
            if (obj is ListViewEXT listView1)
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (listView1.Items[i] is IListView item)
                    {
                        var value = 1 << (item.Text.ToInt() - 1);
                        item.IsSelected = (Info.Value & value) == value;
                    }
                }
            }
        }

        #endregion

        public CommSetViewModel()
        {
            this.Info = Config.Admin.Clone();
            this.MessengerInstance.Register<CommInitMessage>(this, msg => CommInit(msg.Obj));
        }
    }
}