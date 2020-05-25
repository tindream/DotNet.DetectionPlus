using DetectionPlus.Model;
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
    public class ShootViewModel : ViewModelPlus
    {
        private List<ListViewModel> carameList;
        public List<ListViewModel> CarameList { get { return carameList; } }
        public ShootViewModel()
        {
            carameList = new List<ListViewModel>();
            carameList.Add(new ListViewModel("C1") { IsSelected = true });
            carameList.Add(new ListViewModel("C2"));
            carameList.Add(new ListViewModel("C3"));
            carameList.Add(new ListViewModel("全部"));
        }

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
                            case "C1":
                            case "C2":
                                break;
                        }
                    }
                }));
            }
        }
    }
}