using DetectionPlus.Message;
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

namespace DetectionPlus.Win.ViewModel
{
    public class ShootSetViewModel : ViewModelPlus
    {
        private readonly List<ListViewModel> carameList;
        public List<ListViewModel> CarameList { get { return carameList; } }
        public ShootSetViewModel()
        {
            carameList = new List<ListViewModel>();
            carameList.Add(new ListViewModel("C1") { IsSelected = true });
            carameList.Add(new ListViewModel("C2"));
            carameList.Add(new ListViewModel("C3"));
            carameList.Add(new ListViewModel("C4"));
            carameList.Add(new ListViewModel("C5"));
            carameList.Add(new ListViewModel("C6"));
            carameList.Add(new ListViewModel("C7"));
            carameList.Add(new ListViewModel("C8"));
            carameList.Add(new ListViewModel("C9"));
            carameList.Add(new ListViewModel("C10"));
            carameList.Add(new ListViewModel("C11"));
            carameList.Add(new ListViewModel("C12"));
            carameList.Add(new ListViewModel()
            {
                Image = new ImageEXT(new BitmapImage(new Uri("pack://application:,,,/Images/add.png")), null,
                new BitmapImage(new Uri("pack://application:,,,/Images/add_w.png")))
            });
        }

        #region 加载控件
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
        public void LoadControl(ListViewEXT listView1)
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
    }
}