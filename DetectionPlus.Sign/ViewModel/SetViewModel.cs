using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Sign
{
    public class SetViewModel : ViewModelPlus
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
                            case "相机":
                                if (Method.Child(listView1, out Frame frame))
                                {
                                    frame.Content = ViewlLocator.GetViewInstance<CameraSetPage>();
                                }
                                break;
                            case "系统设置":
                                if (Method.Child(listView1, out frame))
                                {
                                    frame.Content = ViewlLocator.GetViewInstance<SystemSetPage>();
                                }
                                break;
                        }
                    }
                }));
            }
        }

        #endregion

        public SetViewModel() { }
    }
}