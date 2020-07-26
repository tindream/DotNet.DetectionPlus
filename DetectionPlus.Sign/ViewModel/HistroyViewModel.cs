using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Paway.Helper;
using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace DetectionPlus.Sign
{
    public class HistroyViewModel : DataGridViewModel<HistroyInfo>
    {
        #region 命令
        protected override void Selectioned(ListViewEXT listView1, IListView item)
        {
            switch (item.Text)
            {
                case "查询":
                    var query = new HistroyQueryWindow();
                    if (Method.Show(listView1, query) == true)
                    {
                        DateTime? start = null;
                        DateTime? end = null;
                        if (ViewModelLocator.Default.HistroyQuery.IStart) start = ViewModelLocator.Default.HistroyQuery.Start.Date;
                        if (ViewModelLocator.Default.HistroyQuery.IEnd) end = ViewModelLocator.Default.HistroyQuery.End.AddDays(1).Date;

                        var find = $"1=1";
                        if (start != null) find += $" and {nameof(HistroyInfo.CreateOn)}>=@start";
                        if (end != null) find += $" and {nameof(HistroyInfo.CreateOn)}<@end";
                        {
                            find += $" order by {nameof(HistroyInfo.CreateOn)} desc";
                            Method.Progress(listView1, () =>
                            {
                                var list = DataService.Default.Find<HistroyInfo>(find, new { start, end });
                                Method.BeginInvoke(listView1, () =>
                                {
                                    base.List.Clear();
                                    foreach (var temp in list) base.List.Add(temp);
                                });
                            });
                        }
                    }
                    break;
                case "清空":
                    if (Method.Ask(listView1, $"确认清空所有记录：共 {List.Count} 项"))
                    {
                        var list = List.ToList();
                        DataService.Default.Delete(list);
                        foreach (var temp in list)
                        {
                            File.Delete(Path.Combine(Config.Images, $"{temp.Id}.bmp"));
                        }
                        List.Clear();
                    }
                    break;
                case "测试":
                    if (SelectedItem is HistroyInfo infoTest)
                    {
                        var file = Path.Combine(Config.Images, $"{infoTest.Id}.bmp");
                        if (File.Exists(file))
                        {
                            this.MessengerInstance.Send(new TestMessage(file) { Obj = listView1 });
                        }
                        else
                        {
                            Method.Toast(listView1, "图片不存在");
                        }
                    }
                    break;
            }
            base.Selectioned(listView1, item);
        }
        protected override List<HistroyInfo> Find()
        {
            var list = DataService.Default.Find<HistroyInfo>($"{nameof(HistroyInfo.CreateOn)}>=@start order by {nameof(HistroyInfo.CreateOn)} desc", new { start = DateTime.Now.Date });
            return list;
        }
        protected override void Delete(DependencyObject obj, HistroyInfo info)
        {
            if (Method.Child(obj, out DataGridEXT dataGridEXT))
            {
                var list = new List<HistroyInfo>();
                foreach (HistroyInfo item in dataGridEXT.SelectedItems)
                {
                    list.Add(item);
                }
                if (list.Count > 0)
                {
                    if (Method.Ask(obj, $"确认删除：{list[0].Desc()} 等 {list.Count} 项"))
                    {
                        var index = dataGridEXT.SelectedIndex;

                        DataService.Default.Delete(list);
                        foreach (var temp in list)
                        {
                            List.Remove(temp);
                            File.Delete(Path.Combine(Config.Images, $"{temp.Id}.bmp"));
                        }

                        if (index >= List.Count) index = List.Count - 1;
                        dataGridEXT.SelectedIndex = index;
                    }
                }
            }
        }
        protected override void OnDoubleClick(DataGridEXT datagrid1, HistroyInfo info)
        {
            var file = Path.Combine(Config.Images, $"{info.Id}.bmp");
            if (File.Exists(file))
            {
                Method.Show(datagrid1, new ImageWindow(file));
            }
            else
            {
                Method.Toast(datagrid1, "图片不存在");
            }
        }

        #endregion

        #region 消息
        private void Histroy(HistroyInfo info)
        {
            Method.Invoke(Config.Window, () => { this.List.Insert(0, info); });
        }
        private void HistroyInit(DependencyObject obj)
        {
            var pagedView = new PagedCollectionView(List) { PageSize = 20, };
            if (obj is DataGridEXT dataGrid)
            {
                dataGrid.ItemsSource = pagedView;
                if (Method.Child(dataGrid, out DataPager dataPager))
                {
                    dataPager.Source = pagedView;
                    dataPager.IsTotalItemCountFixed = true;
                }
            }
        }

        #endregion

        public HistroyViewModel()
        {
            base.server = DataService.Default;
            this.MessengerInstance.Register<HistroyMessage>(this, msg => Histroy(msg.Info));
            this.MessengerInstance.Register<HistroyInitMessage>(this, msg => HistroyInit(msg.Obj));
        }
    }
}