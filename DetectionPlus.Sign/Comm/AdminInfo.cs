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
    public class AdminInfo
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
        /// <summary>
        /// 曝光
        /// </summary>
        public float ExposureTime { get; set; }

        #endregion

        #region Base
        /// <summary>
        /// 数据库版本
        /// </summary>
        public int Version { get; set; }

        #endregion
    }
}
