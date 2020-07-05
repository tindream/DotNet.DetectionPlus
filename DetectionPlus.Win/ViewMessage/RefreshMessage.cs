using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Win
{
    public class RefreshMessage
    {
        public DependencyObject Obj { get; set; }
    }
    public class TeachRefreshMessage : RefreshMessage { }
    public class ShootRefreshMessage : RefreshMessage { }
}
