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
        #region 通讯
        /// <summary>
        /// 输出结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 输出信号
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 输出地址
        /// </summary>
        public byte Address { get; set; }
        /// <summary>
        /// 通讯主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 通讯端口
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region 注册
        /// <summary>
        /// 授权码
        /// </summary>
        public string Listener { get; set; }

        #endregion

        #region Camera
        /// <summary>
        /// 相机名称
        /// </summary>
        public string CameraName { get; set; } = "Com1";
        /// <summary>
        /// 模板中心点
        /// </summary>
        public double CenterX { get; set; }
        public double CenterY { get; set; }
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

        #region 接口
        private string expand = "Expand";
        /// <summary>
        /// 扩展接口目录
        /// </summary>
        public string Expand
        {
            get { return expand; }
            set { expand = value; OnPropertyChanged(); }
        }

        #endregion

        #region Base
        /// <summary>
        /// 数据库版本
        /// </summary>
        public int Version { get; set; }

        #endregion
    }
}
