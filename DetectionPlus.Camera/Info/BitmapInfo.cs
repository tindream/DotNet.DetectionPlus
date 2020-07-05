/***************************************************************************************************
* 文件名称：CameraOperator.cs
* 摘    要：相机操作类
* 当前版本：1.0.0.0
* 日    期：2016-07-07
***************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace DetectionPlus.Camera
{
    public class BitmapInfo
    {
        public Bitmap Bitmap { get; set; }
        public string CameraName { get; set; }
        public BitmapInfo() { }
        public BitmapInfo(Bitmap bmp, string cameraName)
        {
            Bitmap = bmp;
            CameraName = cameraName;
        }
    }
}
