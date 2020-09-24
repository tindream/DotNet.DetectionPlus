using log4net;
using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DetectionPlus
{
    public class Method : Paway.WPF.Method
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 图片二值化转换
        /// </summary>
        public static BitmapSource Binary(string file, double percent)
        {
            //生成图
            var bitmap = BitmapHelper.GetBitmapFormFile(file);
            var width = bitmap.Width;
            var height = bitmap.Height;
            var rect = new Rectangle(0, 0, width, height);
            //用可读写的方式锁定全部位图像素
            var bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var value = 255 * percent / 100;
            unsafe //启用不安全模式
            {
                var p = (byte*)bmpData.Scan0; //获取首地址
                var offset = bmpData.Stride - width * 4;
                //二维图像循环
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        p[0] = (byte)value;//R
                        //p[1] = (byte)value;//G
                        //p[2] = (byte)value;//B
                        p += 4;
                    }
                    p += offset;
                }
            }
            bitmap.UnlockBits(bmpData);
            var result = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return result;
        }

        /// <summary>
        /// 将十六进制字符串转为无符号整数数组
        /// </summary>
        public static byte[] HexStrToByteArr(string hexString)
        {
            int len = hexString.Length / 2;
            if (hexString.Length % 2 == 1)
            {
                len++;
                hexString = hexString.PadLeft(hexString.Length + 1, '0');
            }
            byte[] buffer = new byte[len];
            for (int i = 0; i < len; i++)
            {
                buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return buffer;
        }
    }
}
