using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DetectionPlus.Sign
{
    /// <summary>
    /// 管理数据
    /// </summary>
    [Serializable]
    public class AdminInfo : BaseInfo
    {
        #region Camera
        /// <summary>
        /// 相机名称
        /// </summary>
        public string CameraName { get; set; } = "Com1";
        /// <summary>
        /// 硬触发标记
        /// </summary>
        public bool IsTrigger { get; set; }
        private float exposureTime;
        /// <summary>
        /// 曝光
        /// </summary>
        public float ExposureTime
        {
            get { return exposureTime; }
            set { exposureTime = value; OnPropertyChanged(); }
        }

        #endregion

        private string expand = "Expand";
        /// <summary>
        /// 扩展接口目录
        /// </summary>
        public string Expand
        {
            get { return expand; }
            set { expand = value; OnPropertyChanged(); }
        }

        #region Base
        /// <summary>
        /// 数据库版本
        /// </summary>
        public int Version { get; set; }

        #endregion
    }
}
