using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    [Serializable]
    [Table("Histroys")]
    public class HistroyInfo : BaseInfo
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateOn { get; set; }

        public HistroyInfo() { }
        public HistroyInfo(bool result)
        {
            this.Result = result;
            this.CreateOn = DateTime.Now;
        }
    }
}
