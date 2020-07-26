using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
    public class ResultSetViewModel : ViewModelPlus
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
                                value += 1 << item.Text.ToInt();
                            }
                        }
                        Info.Value = value;
                    }
                }));
            }
        }
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
                                Save(listView1);
                                break;
                        }
                        Messenger.Default.Send(new StatuMessage(info.Text));
                    }
                    listView1.SelectedIndex = -1;
                }));
            }
        }
        private void Save(ListViewEXT listView1)
        {
            if (Method.Find(listView1, out TextBoxEXT tbAddress, "tbAddress"))
            {
                if (Validation.GetHasError(tbAddress))
                {
                    tbAddress.Focus();
                    return;
                }
            }
            UpdateValue(nameof(Config.Admin.Result));
            UpdateValue(nameof(Config.Admin.Value));
            UpdateValue(nameof(Config.Admin.Address));
            UpdateValue(nameof(Config.Admin.ISuccess));
            UpdateValue(nameof(Config.Admin.IFail));
            if (UpdateValue(nameof(Config.Admin.Host)))
            {
                Method.Progress(listView1, () =>
                {
                    Config.Manager.Update(Config.Admin);
                });
            }
            Method.Toast(listView1, "保存成功");
        }
        private bool UpdateValue(string name)
        {
            var adminValue = Config.Admin.GetValue(name);
            var infoValue = Info.GetValue(name);
            if (!adminValue.Equals(infoValue))
            {
                Config.Admin.SetValue(name, infoValue);
                DataService.Default.Update(name);
                return true;
            }
            return false;
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
                        var value = 1 << item.Text.ToInt();
                        item.IsSelected = (Info.Value & value) == value;
                    }
                }
            }
        }

        #endregion

        public ResultSetViewModel()
        {
            this.Info = Config.Admin.Clone();
            this.MessengerInstance.Register<CommInitMessage>(this, msg => CommInit(msg.Obj));
        }
    }
}