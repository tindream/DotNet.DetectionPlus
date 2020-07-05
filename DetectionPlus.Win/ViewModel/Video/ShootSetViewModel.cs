using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DetectionPlus.Win
{
    public class ShootSetViewModel : ViewModelPlus
    {
        #region 属性
        public List<ListViewModel> CarameList { get; } = new List<ListViewModel>();

        #endregion

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
                if (Method.Child<Grid>(listView1, out Grid grid, "grid"))
                {
                    switch (info.Text)
                    {
                        case "C1":
                        case "C2":
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        public ShootSetViewModel()
        {
            CarameList.Add(new ListViewModel("C1") { IsSelected = true });
            CarameList.Add(new ListViewModel("C2"));
            CarameList.Add(new ListViewModel("C3"));
            CarameList.Add(new ListViewModel("C4"));
            CarameList.Add(new ListViewModel("C5"));
            CarameList.Add(new ListViewModel("C6"));
            CarameList.Add(new ListViewModel("C7"));
            CarameList.Add(new ListViewModel("C8"));
            CarameList.Add(new ListViewModel("C9"));
            CarameList.Add(new ListViewModel("C10"));
            CarameList.Add(new ListViewModel("C11"));
            CarameList.Add(new ListViewModel("C12"));
            CarameList.Add(new ListViewModel()
            {
                Image = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/Images/add.png")), null,
                new BitmapImage(new Uri("pack://application:,,,/Images/add_w.png")))
            });
        }
    }
}