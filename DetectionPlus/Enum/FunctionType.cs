using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    public enum FunctionType
    {
        None,
        [Description("上(下)视检测功能")]
        UpDown,
        [Description("其它检测功能")]
        Other,
    }
}
