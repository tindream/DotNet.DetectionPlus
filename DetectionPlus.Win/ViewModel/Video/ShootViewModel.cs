using DetectionPlus.Message;
using DetectionPlus.Model;
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

namespace DetectionPlus.Win.ViewModel
{
    public class ShootViewModel : ViewModelPlus
    {
        private readonly List<ListViewModel> carameList;
        public List<ListViewModel> CarameList { get { return carameList; } }
        public ShootViewModel()
        {
            carameList = new List<ListViewModel>();
            carameList.Add(new ListViewModel("C1") { IsSelected = true });
            carameList.Add(new ListViewModel("C2"));
            carameList.Add(new ListViewModel("C3"));
            carameList.Add(new ListViewModel("C4"));
            carameList.Add(new ListViewModel("C5"));
            carameList.Add(new ListViewModel("C6"));
            carameList.Add(new ListViewModel("全部"));
            carameList.Add(new ListViewModel("设置"));
            try
            {
                var result = DataService.Default.ExecuteScalar("select 0");
            }
            catch (Exception ex)
            {
                Method.Toast(ex.Message());
            }
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
                    grid.RowDefinitions.Clear();
                    grid.ColumnDefinitions.Clear();
                    switch (info.Content)
                    {
                        case "C1":
                        case "C2":
                        default:
                            AddControl(grid, 0, 0, info.Content);
                            break;
                        case "全部":
                            var count = carameList.Count - 1;
                            if (count <= 4)
                            {
                                AddColumn(grid, 0, count);
                            }
                            else
                            {
                                var row = (count + 3) / 4;
                                AddRow(grid, row, count);
                            }
                            break;
                        case "设置":
                            listView1.SelectedIndex = -1;
                            Method.Show(listView1, new ShootSetWindow());
                            break;
                    }
                }
            }
        }
        private void AddControl(Grid grid, int row, int column, object name)
        {
            var frame = new Frame();
            grid.Children.Add(frame);
            frame.Content = ViewlLocator.GetViewInstance<ShootOnePage>($"{grid.GetHashCode()}-{name}");
            frame.SetValue(Grid.RowProperty, row); //设置按钮所在Grid控件的行
            frame.SetValue(Grid.ColumnProperty, column); //设置按钮所在Grid控件的列
        }
        private void AddRow(Grid grid, int count, int total)
        {
            for (int i = 0; i < count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                AddColumn(grid, i, i < count - 1 ? 4 : total % 4);
            }
        }
        private void AddColumn(Grid grid, int row, int count)
        {
            for (int i = 0; i < count; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                AddControl(grid, row, i, row * 4 + i);
            }
        }

        #endregion
    }
}