using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    /// <summary>
    /// 管理数据结构
    /// </summary>
    [Serializable]
    [Table("Admins")]
    public class AdminBaseInfo : BaseInfo, IInfo
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime DateTime { get; set; }

        public AdminBaseInfo()
        {
            this.DateTime = DateTime.Now;
        }
    }
}
