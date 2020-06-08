using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionPlus.Model
{
    public class ListViewModel : IListViewInfo
    {
        public string Content { get; set; }
        public bool IsSelected { get; set; }
        public ImageEXT Image { get; set; }
        public string Desc { get; set; }

        public ListViewModel() { }
        public ListViewModel(string name)
        {
            this.Content = name;
        }
    }
}
