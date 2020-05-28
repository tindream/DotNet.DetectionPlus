using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Win.ViewModel
{
    public class TeachViewModel : ViewModelPlus
    {
        public TeachViewModel() { }

        private ICommand selectionCommand;
        public ICommand SelectionCommand
        {
            get
            {
                return selectionCommand ?? (selectionCommand = new RelayCommand<ListViewEXT>(listView1 =>
                {
                    if (listView1.SelectedItem is IListViewInfo info)
                    {
                        switch (info.Content)
                        {
                            case "检测功能":
                                if (Method.Child<Frame>(listView1, out Frame frame, "frame"))
                                {
                                    frame.Source = new Uri("pack://application:,,,/DetectionPlus.Win;component/View/Teach/FunctionPage.xaml");
                                }
                                break;
                            case "物件形状":
                                if (Method.Child<Frame>(listView1, out frame, "frame"))
                                {
                                    frame.Source = new Uri("pack://application:,,,/DetectionPlus.Win;component/View/Teach/ShapePage.xaml");
                                }
                                break;
                            default:
                                if (Method.Child<Frame>(listView1, out frame, "frame"))
                                {
                                    frame.Source = null;
                                }
                                break;
                        }
                    }
                }));
            }
        }
    }
}