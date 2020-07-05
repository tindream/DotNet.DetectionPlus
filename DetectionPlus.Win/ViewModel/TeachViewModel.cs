using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Win
{
    public class TeachViewModel : ViewModelPlus
    {
        #region 命令
        private ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand<ListViewEXT>(listView1 =>
                {
                    LoadControl(listView1);
                }));
            }
        }
        private void LoadControl(ListViewEXT listView1)
        {
            if (listView1.SelectedItem is IListView info)
            {
                switch (info.Text)
                {
                    case "检测功能":
                        if (Method.Child<Frame>(listView1, out Frame frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<FunctionPage>();
                        }
                        break;
                    case "物件形状":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<ShapePage>();
                        }
                        break;
                    case "背景差异":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<BackgroundPage>();
                        }
                        break;
                    case "二值化调整":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<BinaryPage>();
                        }
                        break;
                    case "框选物件":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<SelectionPage>();
                        }
                        break;
                    case "框选边缘":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<WdgePage>();
                        }
                        break;
                    case "检测项目":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<ProjectPage>();
                        }
                        break;
                    case "基本功能":
                        if (Method.Child<Frame>(listView1, out frame, "frame"))
                        {
                            frame.Content = ViewlLocator.GetViewInstance<BasalPage>();
                        }
                        break;
                }
            }
        }

        #endregion

        public TeachViewModel()
        {
            this.MessengerInstance.Register<TeachRefreshMessage>(this, msg => LoadControl(msg.Obj as ListViewEXT));
        }
    }
}