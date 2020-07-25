using DetectionPlus.Camera;
using DetectionPlus.HWindowTool;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HalconDotNet;
using Paway.Helper;
using Paway.WPF;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DetectionPlus.Sign
{
    public class ModelSetViewModel : ViewModelPlus
    {
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
                            case "方向矩形":
                                Messenger.Default.Send(new ModelMessage() { Obj = listView1 });
                                break;
                            case "Save":
                                Messenger.Default.Send(new SaveMessage() { Obj = listView1 });
                                Method.Toast(listView1, "保存成功");
                                break;
                        }
                        Messenger.Default.Send(new StatuMessage(info.Text));
                    }
                    listView1.SelectedIndex = -1;
                }));
            }
        }

        #endregion

        public ModelSetViewModel() { }
    }
}