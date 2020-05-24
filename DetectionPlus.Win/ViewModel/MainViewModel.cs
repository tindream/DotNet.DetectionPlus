﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.WPF;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Win.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelPlus
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //Colors.Transparent
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
                            case "教导":
                                Method.Show(listView1, new TeachWindow());
                                listView1.SelectedIndex = -1;
                                break;
                        }
                    }
                }));
            }
        }

        private string title = "Hello MVVMLight,Hello C#";
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged(); }
        }
    }
}