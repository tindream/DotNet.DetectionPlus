using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionPlus.Camera
{
    public class HKCamera : ICamera
    {
        private readonly HKCameraBase cameraOperator;
        private Bitmap currentBitmap;
        public event Action<BitmapInfo> ScreenEvent;
        public object Tag { get; set; }

        public HKCamera()
        {
            cameraOperator = new HKCameraBase();
            cameraOperator.ImageReady += CameraOperator_ImageReady;
        }

        #region 获取图片接口
        private void CameraOperator_ImageReady(BitmapInfo bitmapInfo)
        {
            ClearImage();
            currentBitmap = (Bitmap)bitmapInfo.Bitmap.Clone();
            ScreenEvent?.Invoke(bitmapInfo);
        }
        private void ClearImage()
        {
            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
                currentBitmap = null;
            }
        }
        public Bitmap MaxImage()
        {
            Bitmap bmp = null;
            if (!cameraOperator.IsCameraOpen)
                throw new Exception("请先连接相机");
            CameraStop();
            int width = cameraOperator.SizeMax.Width;
            int height = cameraOperator.SizeMax.Height;
            var b = SetROI(new Rectangle(0, 0, width, height), width, height, 4);

            if (b)
            {
                DateTime startTime = new DateTime();
                ClearImage();
                OneShot();
                while (true)
                {
                    DateTime endTime = new DateTime();
                    double usedMilliseconds = (endTime - startTime).TotalMilliseconds;
                    if (usedMilliseconds > 1000)
                    {
                        break;
                    }
                    if (currentBitmap != null)
                    {
                        bmp = (Bitmap)currentBitmap.Clone();
                        ClearImage();
                        break;
                    }
                }
            }
            return bmp;
        }
        public Bitmap CurrentImage()
        {
            Bitmap bmp = null;
            DateTime startTime = DateTime.Now;
            ClearImage();
            OneShot();
            while (true)
            {
                DateTime endTime = DateTime.Now;
                double usedMilliseconds = (endTime - startTime).TotalMilliseconds;
                if (usedMilliseconds > 2000)
                {
                    break;
                }
                if (currentBitmap != null)
                {
                    bmp = (Bitmap)currentBitmap.Clone();
                    ClearImage();
                    break;
                }
                System.Threading.Thread.Sleep(10);
            }
            return bmp;
        }

        #endregion

        #region 相机操作接口
        public string CameraName { get; set; }
        public bool IsOpen { get { return cameraOperator.IsCameraOpen; } }
        public int GetCameraCount
        {
            get
            {
                return cameraOperator.CameraCount();
            }
        }
        public Size GetCameraROIMax
        {
            get
            {
                return cameraOperator.GetCameraROIMax();
            }
        }
        public Rectangle GetCameraROI
        {
            get { return cameraOperator.GetCameraROI(); }
        }
        public float ExposureTime
        {
            get { return cameraOperator.ExposureTime; }
            set { cameraOperator.ExposureTime = value; }
        }
        public float InitExposureTime
        {
            get { return cameraOperator.InitExposureTime; }
            set { cameraOperator.InitExposureTime = value; }
        }
        public float Gain
        {
            get { return cameraOperator.Gain; }
            set { cameraOperator.Gain = value; }
        }
        public float InitGain
        {
            get { return cameraOperator.InitGain; }
            set { cameraOperator.InitGain = value; }
        }

        public bool Connect()
        {
            if (!cameraOperator.IsCameraOpen)
            {
                var device = cameraOperator.CameraDevice(CameraName);
                int nRet = cameraOperator.OpenMVS(device);
                if (nRet == 0)
                {
                    if (!CameraStartGrabing(cameraOperator))
                    {
                        throw new Exception("相机取流错误");
                    }
                    else
                    {
                        Rectangle rectangle = GetCameraROI;
                        Size size = GetCameraROIMax;
                    }
                }
                else
                {
                    throw new Exception(string.Format("连接相机错误:Code({0})", nRet));
                }
            }
            return true;
        }
        public void OneShot()
        {
            if (CameraStartGrabing(cameraOperator))
                cameraOperator.OneShot();
            else
                throw new Exception("无法启动单张采集模式");
        }
        public void ContinueShot()
        {
            if (CameraStartGrabing(cameraOperator))
                cameraOperator.ContinueShot();
            else
                throw new Exception("无法启动连续采集模式");
        }
        public bool CameraStop()
        {
            if (cameraOperator.IsGrabbing)
            {
                int nRet = cameraOperator.StopGrabbing();
                if (nRet != 0)
                    return false;
            }
            cameraOperator.Stop();
            return true;
        }
        public bool CameraClose()
        {
            if (cameraOperator.IsGrabbing)
            {
                int nRet = cameraOperator.StopGrabbing();
                if (nRet != 0)
                    return false;
            }
            cameraOperator.Stop();
            if (cameraOperator.IsCameraOpen)
            {
                int nRet = cameraOperator.Close();
                if (nRet != 0)
                    return false;
            }
            return true;
        }
        public bool SetTriggerMode(bool isTrigger)
        {
            return cameraOperator.AcquisitionMode(isTrigger);
        }
        public bool SetROI(Rectangle rect, int widthMax, int heightMax, int increase)
        {
            int width = rect.Width - rect.Width % increase;
            int height = rect.Height - rect.Height % increase;
            int offsetX = rect.X - rect.X % increase;
            int offsetY = rect.Y - rect.Y % increase;

            if (width + offsetX > widthMax || height + offsetY > heightMax)
            {
                return false;
            }
            cameraOperator.RectROI = new Rectangle(offsetX, offsetY, width, height);
            return true;
        }
        public bool UserSetSave()
        {
            return cameraOperator.UserSetSave();
        }
        private bool CameraStartGrabing(HKCameraBase cameraOperator)
        {
            if (!cameraOperator.IsGrabbing)
            {
                int nRet = cameraOperator.StartGrabImage();
                System.Threading.Thread.Sleep(200);
                if (nRet != 0)
                    return false;
            }
            return true;
        }

        #endregion
    }
}
