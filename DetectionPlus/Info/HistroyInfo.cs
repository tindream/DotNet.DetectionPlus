using Paway.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DetectionPlus
{
    [Serializable]
    [Table("Histroys")]
    public class HistroyInfo : BaseInfo
    {
        /// <summary>
        /// 时间
        /// </summary>
        [Text("执行时间")]
        public DateTime CreateOn { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        [NoShow]
        public bool Result { get; set; }
        [Text("结果")]
        [NoSelect]
        public string Results { get { return Result ? "成功" : "失败"; } }

        [Text("描述")]
        public string Description { get; set; }

        [Text("图片")]
        [NoSelect]
        public string Images
        {
            get
            {
                return File.Exists(Path.Combine(Config.Images, $"{Id}.bmp")) ? "已存" : null;
            }
        }

        public override string Desc()
        {
            return $"{CreateOn:G}-{Results}";
        }
        public HistroyInfo()
        {
            this.CreateOn = DateTime.Now;
        }
        public HistroyInfo(bool result) : this()
        {
            this.Result = result;
        }
    }
}
