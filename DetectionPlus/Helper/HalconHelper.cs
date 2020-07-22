using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HalconDotNet;

namespace DetectionPlus
{
    public class HalconHelper
    {
        #region 创建模板(形状)
        /// <summary>
        /// 创建模板(形状)
        /// ho_Image 输入图像  ho_region 输入区域   savePath 模板保存路径  index 相机编号
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_region">输入区域</param>
        /// <param name="regionPath">区域文件</param>
        /// <param name="modelPath">模板文件</param>
        public static void CreateShapModel(HObject ho_Image, HObject ho_region, string regionPath, string modelPath)
        {
            HObject ho_ImageReduced, ho_GrayImage;
            HTuple hv_ModelID = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            try
            {
                if (ho_Image != null)
                {
                    HTuple hv_Channels;
                    HOperatorSet.CountChannels(ho_Image, out hv_Channels);
                    ho_GrayImage.Dispose();
                    if (hv_Channels > 1)
                    {
                        HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                    }
                    else
                    {
                        HOperatorSet.CopyImage(ho_Image, out ho_GrayImage);
                    }
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_region, out ho_ImageReduced);
                    HOperatorSet.CreateShapeModel(ho_ImageReduced, "auto", (new HTuple(0)).TupleRad()
                        , (new HTuple(360)).TupleRad(), "auto", "auto", "use_polarity", "auto", "auto", out hv_ModelID);
                    //HOperatorSet.WriteImage(ho_Image, "bmp", 0, imagePath); //保存示教模型
                    HOperatorSet.WriteRegion(ho_region, regionPath);
                    HOperatorSet.WriteShapeModel(hv_ModelID, modelPath);  //保存模板

                    ho_ImageReduced.Dispose();
                }
            }
            finally
            {
                //ho_region.Dispose();
                ho_ImageReduced.Dispose();
            }
        }

        #endregion

        #region 区域
        /// <summary>
        /// 保存区域
        /// </summary>
        public static void WriteRegion(HObject ho_region, string regionPath)
        {
            HOperatorSet.WriteRegion(ho_region, regionPath);
        }
        /// <summary>
        /// 读取区域并显示
        /// </summary>
        public static HObject ReadRegion(string regionPath, HWindow hWindow, bool iDispObj = true)
        {
            if (File.Exists(regionPath))
            {
                HObject ho_Region;
                HOperatorSet.GenEmptyObj(out ho_Region);
                ho_Region.Dispose();
                HOperatorSet.ReadRegion(out ho_Region, regionPath);
                if (iDispObj) HOperatorSet.DispObj(ho_Region, hWindow);
                return ho_Region;
            }
            return null;
        }

        #endregion
    }
}
