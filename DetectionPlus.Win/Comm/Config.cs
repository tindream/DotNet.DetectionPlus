﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DetectionPlus.Win
{
    public class Config : DetectionPlus.Config
    {
        #region 常量
        public new const string Text = "相机模式";

        #endregion

        #region 全局数据
        public static AdminInfo Admin { get; set; }

        #endregion
    }
}
