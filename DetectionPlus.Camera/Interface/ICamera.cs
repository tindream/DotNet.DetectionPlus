using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DetectionPlus.Camera
{
    public interface ICamera
    {
        event Action<BitmapInfo> ScreenEvent;

        object Tag { get; set; }
        string CameraName { get; set; }
        bool IsOpen { get; }
        int GetCameraCount { get; }
        Size GetCameraROIMax { get; }
        Rectangle GetCameraROI { get; }
        float ExposureTime { get; set; }
        float InitExposureTime { get; set; }
        float Gain { get; set; }
        float InitGain { get; set; }

        Bitmap CurrentImage();
        Bitmap MaxImage();
        bool Connect();
        void OneShot();
        void ContinueShot();
        bool CameraStop();
        bool CameraClose();
        bool SetTriggerMode(bool isTrigger);
        bool SetROI(Rectangle rect, int widthMax, int heightMax, int increase);
        bool UserSetSave();
    }
}
