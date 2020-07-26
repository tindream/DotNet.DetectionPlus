using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus
{
    public class DataGridViewModel<T> : ViewModelPlus where T : class, IModel, IId, new()
    {
        #region 属性
        protected IDataService server;

        public ObservableCollection<T> List { get; protected set; } = new ObservableCollection<T>();
        private object selectedItem;
        public object SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; RaisePropertyChanged(); }
        }

        protected virtual AddViewModelPlus<T> ViewModel()
        {
            return new AddViewModelPlus<T>();
        }
        private AddViewModelPlus<T> addViewModel;
        protected AddViewModelPlus<T> AddViewModel
        {
            get
            {
                if (addViewModel == null) addViewModel = ViewModel();
                return addViewModel;
            }
        }

        #endregion

        #region 命令
        protected virtual Window AddWindow()
        {
            return null;
        }
        protected virtual List<T> Find()
        {
            return server.Find<T>();
        }
        protected virtual void Added(T info)
        {
            server.Insert(info);
            List.Add(info);
        }
        protected virtual void Updated(DependencyObject obj, T info)
        {
            server.Update(info);
        }
        protected virtual void Delete(DependencyObject obj, T info)
        {
            if (Method.Ask(obj, "确认删除：" + info.Desc()))
            {
                var index = -1;
                if (Method.Child(obj, out DataGridEXT datagrid))
                {
                    index = datagrid.SelectedIndex;
                }
                Deleted(obj, info);
                if (index >= List.Count) index = List.Count - 1;
                datagrid.SelectedIndex = index;
            }
        }
        protected virtual void Deleted(DependencyObject obj, T info)
        {
            server.Delete(info);
            List.Remove(info);
        }
        protected virtual void Selectioned(ListViewEXT listView1, IListView info)
        {
            switch (info.Text)
            {
                case "刷新":
                    Refresh(listView1);
                    break;
                case "新加":
                    AddViewModel.Info = new T();
                    var add = AddWindow();
                    if (add != null && Method.Show(listView1, add) == true)
                    {
                        Added(AddViewModel.Info);
                        if (Method.Child(listView1, out DataGridEXT datagrid))
                        {
                            datagrid.SelectedIndex = List.Count - 1;
                        }
                    }
                    break;
                case "编辑":
                    if (selectedItem is T infoEdit) Edit(listView1, infoEdit);
                    break;
                case "删除":
                    if (selectedItem is T infoDel)
                    {
                        Delete(listView1, infoDel);
                    }
                    break;
            }
            listView1.SelectedIndex = -1;
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
                        Selectioned(listView1, info);
                    }
                }));
            }
        }
        protected void Refresh(DependencyObject obj)
        {
            Method.Progress(obj, () =>
            {
                var list = Find();
                Method.BeginInvoke(obj, () =>
                {
                    List.Clear();
                    foreach (var item in list) List.Add(item);
                });
            }, Refreshed);
        }
        protected virtual void Refreshed() { }
        private void Edit(DependencyObject obj, T info)
        {
            AddViewModel.Info = info;
            var edit = AddWindow();
            if (Method.Show(obj, edit) == true)
            {
                Updated(obj, info);
            }
        }
        protected virtual void OnDoubleClick(DataGridEXT datagrid1, T info)
        {
            Edit(datagrid1, info);
        }
        private ICommand doubleClick;
        public ICommand DoubleClick
        {
            get
            {
                return doubleClick ?? (doubleClick = new RelayCommand<MouseButtonEventArgs>(e =>
                {
                    if (e.Source is DataGridEXT datagrid1)
                    {
                        var point = e.GetPosition(datagrid1);
                        var obj = datagrid1.InputHitTest(point);
                        if (Method.Parent(obj, out DataGridRow row))
                        {
                            if (row.Item is T info)
                            {
                                OnDoubleClick(datagrid1, info);
                            }
                        }
                    }
                }));
            }
        }


        #endregion

        public DataGridViewModel() { }
    }
}