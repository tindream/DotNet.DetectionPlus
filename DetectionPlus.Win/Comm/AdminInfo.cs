using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus.Win
{
    /// <summary>
    /// 管理数据
    /// </summary>
    [Serializable]
    public class AdminInfo
    {
        #region Base
        /// <summary>
        /// 数据库版本
        /// </summary>
        public int Version { get; set; }

        #endregion
    }
}
