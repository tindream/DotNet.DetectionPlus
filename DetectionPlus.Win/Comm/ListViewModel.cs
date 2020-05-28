﻿using Paway.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionPlus.Win
{
    public class ListViewModel : IListViewInfo
    {
        public string Content { get; set; }
        public bool IsSelected { get; set; }
        public ImageEXT Image { get; set; }
        public string Desc { get; set; }

        public ListViewModel(string name)
        {
            this.Content = name;
        }
    }
}