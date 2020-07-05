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
using log4net;
using System.Reflection;

namespace DetectionPlus.Camera
{
    internal class HKCameraBase : CameraBase
    {
        #region 属性与变量
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object obj = new object();
        private readonly BitmapInfo bitmapInfo;
        private Thread m_grabThread;
        private bool m_grabThreadRun = false;
        private bool m_grabOnce = false;
        private MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        private Bitmap m_bitmap;

        public Size SizeMax = new Size(5500, 5500);
        public Rectangle RectROI { get; set; }

        public event Action<BitmapInfo> ImageReady;

        #endregion

        public HKCameraBase()
        {
            bitmapInfo = new BitmapInfo();
            m_grabThread = new Thread(Grab);
        }

        #region 取图
        /// <summary>
        /// 取图
        /// </summary>
        public void OnMVImageReadyEventCallback()
        {
            lock (obj)
            {
                try
                {
                    byte[] buffer = new byte[SizeMax.Width * SizeMax.Height * 3];
                    IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    uint nDataLen = 0;
                    var stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO();
                    //  stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                    //超时获取一帧，超时时间为1秒

                    var nRet = GetOneFrame(pData, ref nDataLen, (uint)SizeMax.Width * (uint)SizeMax.Height * 3, ref stFrameInfo);

                    // nRet = GetImageBGR(pData, (uint)SizeMax.Width * (uint)SizeMax.Height * 3, ref stFrameInfo);

                    if (MyCamera.MV_OK == nRet)
                    {
                        if (m_bitmap != null)
                        {
                            /* Update the bitmap with the image data. */
                            UpdateBitmap(m_bitmap, buffer, stFrameInfo.nWidth, stFrameInfo.nHeight, false);
                            /* To show the new image, request the display control to update itself. */
                        }
                        else /* A new bitmap is required. */
                        {
                            CreateBitmap(ref m_bitmap, stFrameInfo.nWidth, stFrameInfo.nHeight, false);
                            UpdateBitmap(m_bitmap, buffer, stFrameInfo.nWidth, stFrameInfo.nHeight, false);
                            //* Provide the display control with the new bitmap. This action automatically updates the display. */
                        }
                        bitmapInfo.Bitmap = (Bitmap)m_bitmap.Clone();
                        ImageReady?.Invoke(bitmapInfo);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        #endregion

        #region 相机设置
        public MyCamera.MV_CC_DEVICE_INFO CameraDevice(string name)
        {
            /*创建设备列表*/
            System.GC.Collect();
            EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            //在窗体列表中显示设备名
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (name == gigeInfo.chUserDefinedName || name == gigeInfo.chSerialNumber)
                    {
                        bitmapInfo.CameraName = name;
                        return device;
                    }
                }
            }
            throw new ArgumentException("未找到相机: " + name);
        }
        public int CameraCount()
        {
            /*创建设备列表*/
            System.GC.Collect();
            EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            int count = (int)m_pDeviceList.nDeviceNum;
            return count;
        }
        public int OpenMVS(MyCamera.MV_CC_DEVICE_INFO device)
        {
            if (m_bitmap != null)
            {
                m_bitmap.Dispose();
                m_bitmap = null;

            }
            if (m_pDeviceList.nDeviceNum == 0)
            {
                return -1;  //没有相机
            }
            //打开设备
            var nRet = Open(ref device);
            if (MyCamera.MV_OK != nRet)
            {
                return -2; //打开相机失败
            }
            //设置采集连续模式
            SetEnumValue("AcquisitionMode", 2);// 工作在连续模式
            SetEnumValue("TriggerMode", 0);
            // 连续模式     
            //触发源选择:0 - Line0;
            //           1 - Line1;
            //           2 - Line2;
            //           3 - Line3;
            //           4 - Counter;
            //           7 - Software;
            // SetEnumValue("TriggerSource", triggerSource);    //触发源   
            //return cameraOperator.SetROI(rect, widthMax, heightMax, increase);
            //Size maxSize = GetCameraROIMax();
            //SetROI(RectROI, maxSize.Width, maxSize.Height, 4);
            this.ExposureTime = InitExposureTime;
            //SetGain(Gain);
            IsCameraOpen = true;
            return 0;
        }
        public bool AcquisitionMode(bool isTrigger)
        {
            int nRet = SetEnumValue("AcquisitionMode", 2);// 工作在连续模式
            if (nRet == MyCamera.MV_OK)
            {
                if (isTrigger)
                {
                    nRet = SetEnumValue("TriggerMode", 1);
                    if (nRet != MyCamera.MV_OK)
                    {
                        return false;
                    }
                    //触发源选择:0 - Line0;
                    //           1 - Line1;
                    //           2 - Line2;
                    //           3 - Line3;
                    //           4 - Counter;
                    //           7 - Software;
                    //nRet= SetEnumValue("TriggerSource", 0);
                    if (nRet != MyCamera.MV_OK)
                    {
                        return false;
                    }
                }
                else
                {
                    nRet = SetEnumValue("TriggerMode", 0);    // 连续模式
                    if (nRet != MyCamera.MV_OK)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public bool UserSetSave()
        {
            int nRet = CommandExecute("UserSetSave");
            if (nRet == MyCamera.MV_OK)
                return true;
            return false;
        }
        public bool UserSet(int index)
        {
            int nRet = SetEnumValue("UserSetSelector", (uint)index);
            if (nRet == MyCamera.MV_OK)
                return true;
            return false;
        }
        public float InitExposureTime { get; set; }
        public float ExposureTime
        {
            get
            {
                float exposureTime = 0;
                GetFloatValue("ExposureTime", ref exposureTime);
                return exposureTime;
            }
            set
            {
                SetEnumValue("ExposureAuto", 0);
                SetFloatValue("ExposureTime", value);
            }
        }
        public float InitGain { get; set; }
        public float Gain
        {
            get
            {
                float gain = 0;
                GetFloatValue("Gain", ref gain);
                return gain;
            }
            set
            {
                SetEnumValue("GainAuto", 0);
                SetFloatValue("Gain", value);
            }
        }
        public Rectangle GetCameraROI()
        {
            uint offsetX = 0, offsetY = 0, width = 0, height = 0;
            GetIntValue("OffsetX", ref offsetX);
            GetIntValue("OffsetY", ref offsetY);
            GetIntValue("Width", ref width);
            GetIntValue("Height", ref height);
            var rect = new Rectangle((int)offsetX, (int)offsetY, (int)width, (int)height);
            return rect;
        }
        public Size GetCameraROIMax()
        {
            uint widthMax = 0, heightMax = 0;
            GetIntValue("WidthMax", ref widthMax);
            GetIntValue("HeightMax", ref heightMax);
            var p = new Size((int)widthMax, (int)heightMax);
            return p;
        }
        public bool SetROI(Rectangle rect, int widthMax, int heightMax, int increase)
        {
            int nRet;
            int width = rect.Width - rect.Width % increase;
            int height = rect.Height - rect.Height % increase;
            int offsetX = rect.X - rect.X % increase;
            int offsetY = rect.Y - rect.Y % increase;

            if (width + offsetX > widthMax || height + offsetY > heightMax)
            {
                return false;
            }
            nRet = SetIntValue("Width", (uint)width);
            if (nRet != MyCamera.MV_OK)
                return false;
            nRet = SetIntValue("Height", (uint)height);
            if (nRet != MyCamera.MV_OK)
                return false;
            nRet = SetIntValue("OffsetX", (uint)offsetX);
            if (nRet != MyCamera.MV_OK)
                return false;
            nRet = SetIntValue("OffsetY", (uint)offsetY);
            if (nRet != MyCamera.MV_OK)
                return false;

            return true;
        }
        public int StartGrabImage()
        {
            int nRet = 0;
            if (!IsGrabbing)
            {
                //开始采集
                nRet = StartGrabbing();
                if (MyCamera.MV_OK != nRet)
                {
                    return -1;
                }
                //标志位置位true
                IsGrabbing = true;
            }
            return nRet;
        }

        #endregion

        #region 相机线程
        private void Grab()
        {
            try
            {
                while (m_grabThreadRun)
                {
                    OnMVImageReadyEventCallback();
                    if (m_grabOnce)
                    {
                        m_grabThreadRun = false;
                        break;
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        public void OneShot()
        {
            if (!m_grabThread.IsAlive) /* Only start when open and not grabbing already. */
            {
                /* Set up the grabbing and start. */
                m_grabOnce = true;
                m_grabThreadRun = true;
                m_grabThread = new Thread(Grab);
                m_grabThread.Start();
            }
        }
        public void ContinueShot()
        {
            if (!m_grabThread.IsAlive)  /* Only start when open and not grabbing already. */
            {
                /* Set up the grabbing and start. */
                m_grabOnce = false;
                m_grabThreadRun = true;
                m_grabThread = new Thread(Grab);
                m_grabThread.Start();
            }
        }
        public void Stop()
        {
            if (m_grabThread.IsAlive) /* Only start when open and grabbing. */
            {
                m_grabThreadRun = false; /* Causes the grab thread to stop. */
                m_grabThread.Join(); /* Wait for it to stop. */
            }
        }

        #endregion
    }
}
