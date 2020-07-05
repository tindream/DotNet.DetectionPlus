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
    using ImageCallBack = MyCamera.cbOutputdelegate;
    using ExceptionCallBack = MyCamera.cbExceptiondelegate;

    internal class CameraBase
    {
        public const int CO_FAIL = -1;
        public const int CO_OK = 0;
        private MyCamera m_pCSI;
        public bool IsCameraOpen { get; set; }
        public bool IsGrabbing { get; set; }

        public CameraBase()
        {
            m_pCSI = new MyCamera();
        }

        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           EnumDevices</para> 
        /// <para> * @brief        枚举可连接设备</para> 
        /// <para> * @param        nLayerType       IN         传输层协议：1-GigE; 4-USB;可叠加</para> 
        /// <para> * @param        stDeviceList     OUT        设备列表</para> 
        /// <para> * @return       成功：0；错误：错误码</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int EnumDevices(uint nLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDeviceList)
        {
            return MyCamera.MV_CC_EnumDevices_NET(nLayerType, ref stDeviceList);
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           Open</para> 
        /// <para> * @brief        连接设备</para> 
        /// <para> * @param        stDeviceInfo       IN       设备信息结构体</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int Open(ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
        {
            if (null == m_pCSI)
            {
                m_pCSI = new MyCamera();
                if (null == m_pCSI)
                {
                    return CO_FAIL;
                }
            }
            int nRet;
            nRet = m_pCSI.MV_CC_CreateDevice_NET(ref stDeviceInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            nRet = m_pCSI.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           Close</para> 
        /// <para> * @brief        关闭设备</para> 
        /// <para> * @param        none</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int Close()
        {
            int nRet;

            nRet = m_pCSI.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            nRet = m_pCSI.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            IsCameraOpen = false;
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           StartGrabbing</para> 
        /// <para> * @brief        开始采集</para> 
        /// <para> * @param        none</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int StartGrabbing()
        {
            int nRet;
            //开始采集
            nRet = m_pCSI.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           StopGrabbing</para> 
        /// <para> * @brief        停止采集</para> 
        /// <para> * @param        none</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int StopGrabbing()
        {
            int nRet;
            nRet = m_pCSI.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            IsGrabbing = false;
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           RegisterImageCallBack</para> 
        /// <para> * @brief        注册取流回调函数</para> 
        /// <para> * @param        CallBackFunc          IN        回调函数</para> 
        /// <para> * @param        pUser                 IN        用户参数</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int RegisterImageCallBack(ImageCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterImageCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           RegisterExceptionCallBack</para> 
        /// <para> * @brief        注册异常回调函数</para> 
        /// <para> * @param        CallBackFunc          IN        回调函数</para> 
        /// <para> * @param        pUser                 IN        用户参数</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int RegisterExceptionCallBack(ExceptionCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterExceptionCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           GetOneFrame</para> 
        /// <para> * @brief        获取一帧图像数据</para> 
        /// <para> * @param        pData                 IN-OUT            数据数组指针</para> 
        /// <para> * @param        pnDataLen             IN                数据大小</para> 
        /// <para> * @param        nDataSize             IN                数组缓存大小</para> 
        /// <para> * @param        pFrameInfo            OUT               数据信息</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int GetOneFrame(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo)
        {
            int nRet = m_pCSI.MV_CC_GetOneFrame_NET(pData, nDataSize, ref pFrameInfo);
            pnDataLen = pFrameInfo.nFrameLen;
            if (MyCamera.MV_OK != nRet)
            {
                return nRet;
            }
            return nRet;
        }
        public int GetImageBGR(IntPtr pData, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo)
        {
            int nRet = m_pCSI.MV_CC_GetImageForBGR_NET(pData, nDataSize, ref pFrameInfo, 10);
            return nRet;
        }
        public int GetOneFrameTimeout(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, Int32 nMsec)
        {
            int nRet = m_pCSI.MV_CC_GetOneFrameTimeout_NET(pData, nDataSize, ref pFrameInfo, nMsec);
            pnDataLen = pFrameInfo.nFrameLen;
            if (MyCamera.MV_OK != nRet)
            {
                return nRet;
            }
            return nRet;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           Display</para> 
        /// <para> * @brief        显示图像</para> 
        /// <para> * @param        hWnd                  IN        窗口句柄</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int Display(IntPtr hWnd)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_Display_NET(hWnd);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           GetIntValue</para> 
        /// <para> * @brief        获取Int型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        pnValue               OUT       返回值</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int GetIntValue(string strKey, ref UInt32 pnValue)
        {

            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_pCSI.MV_CC_GetIntValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           SetIntValue</para> 
        /// <para> * @brief        设置Int型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int SetIntValue(string strKey, UInt32 nValue)
        {

            int nRet = m_pCSI.MV_CC_SetIntValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           GetFloatValue</para> 
        /// <para> * @brief        获取Float型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        pValue                OUT       返回值</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int GetFloatValue(string strKey, ref float pfValue)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetFloatValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pfValue = stParam.fCurValue;

            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           SetFloatValue</para> 
        /// <para> * @brief        设置Float型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        fValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int SetFloatValue(string strKey, float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetFloatValue_NET(strKey, fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           GetEnumValue</para> 
        /// <para> * @brief        获取Enum型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        pnValue               OUT       返回值</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int GetEnumValue(string strKey, ref UInt32 pnValue)
        {
            MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
            int nRet = m_pCSI.MV_CC_GetEnumValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           SetEnumValue</para> 
        /// <para> * @brief        设置Float型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int SetEnumValue(string strKey, UInt32 nValue)
        {
            int nRet = m_pCSI.MV_CC_SetEnumValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           GetBoolValue</para> 
        /// <para> * @brief        获取Bool型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        pbValue               OUT       返回值</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int GetBoolValue(string strKey, ref bool pbValue)
        {
            int nRet = m_pCSI.MV_CC_GetBoolValue_NET(strKey, ref pbValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           SetBoolValue</para> 
        /// <para> * @brief        设置Bool型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        bValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int SetBoolValue(string strKey, bool bValue)
        {
            int nRet = m_pCSI.MV_CC_SetBoolValue_NET(strKey, bValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           GetStringValue</para> 
        /// <para> * @brief        获取String型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        strValue              OUT       返回值</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int GetStringValue(string strKey, ref string strValue)
        {
            MyCamera.MVCC_STRINGVALUE stParam = new MyCamera.MVCC_STRINGVALUE();
            int nRet = m_pCSI.MV_CC_GetStringValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            strValue = stParam.chCurValue;

            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           SetStringValue</para> 
        /// <para> * @brief        设置String型参数值</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @param        strValue              IN        设置参数值，具体取值范围参考HikCameraNode.xls文档</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int SetStringValue(string strKey, string strValue)
        {
            int nRet = m_pCSI.MV_CC_SetStringValue_NET(strKey, strValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para> 
        /// <para> * @fn           CommandExecute</para> 
        /// <para> * @brief        Command命令</para> 
        /// <para> * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int CommandExecute(string strKey)
        {
            int nRet = m_pCSI.MV_CC_SetCommandValue_NET(strKey);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /// <summary>
        /// <para>/****************************************************************************</para>
        /// <para> * @fn           SaveImage</para> 
        /// <para> * @brief        保存图片</para> 
        /// <para> * @param        pSaveParam            IN        保存图片配置参数结构体</para> 
        /// <para> * @return       成功：0；错误：-1</para> 
        /// <para> ****************************************************************************/</para> 
        /// </summary>
        public int SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM_EX pSaveParam)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_SaveImageEx_NET(ref pSaveParam);
            return nRet;
        }

        /// <summary>
        /// Creates a new bitmap object with the supplied properties.
        /// </summary>
        public void CreateBitmap(ref Bitmap bitmap, int width, int height, bool color)
        {
            bitmap = new Bitmap(width, height, GetFormat(color));

            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                ColorPalette colorPalette = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    colorPalette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = colorPalette;
            }
        }
        private PixelFormat GetFormat(bool color)
        {
            return color ? PixelFormat.Format24bppRgb : PixelFormat.Format8bppIndexed;
        }
        /// <summary>
        /// Copies the raw image data to the bitmap buffer.
        /// </summary>
        public void UpdateBitmap(Bitmap bitmap, byte[] buffer, int width, int height, bool color)
        {
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            /* Get the pointer to the bitmap's buffer. */
            IntPtr ptrBmp = bmpData.Scan0;
            /* Compute the width of a line of the image data. */
            int imageStride = GetStride(width, color);
            /* If the widths in bytes are equal, copy in one go. */
            if (imageStride == bmpData.Stride)
            {
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, ptrBmp, bmpData.Stride * bitmap.Height);
            }
            else /* The widths in bytes are not equal, copy line by line. This can happen if the image width is not divisible by four. */
            {
                for (int i = 0; i < bitmap.Height; ++i)
                {
                    Marshal.Copy(buffer, i * imageStride, new IntPtr(ptrBmp.ToInt64() + i * bmpData.Stride), width);
                }
            }
            /* Unlock the bits. */
            bitmap.UnlockBits(bmpData);
        }
        private int GetStride(int width, bool color)
        {
            return color ? width * 3 : width;
        }
    }
}
