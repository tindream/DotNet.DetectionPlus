using log4net;
using Paway.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DetectionPlus
{
    //public class Sign : ISign
    //{
    //    public int Result(string image)
    //    {
    //        return image.GetHashCode();
    //    }
    //}
    public interface ISign
    {
        int Result(string image);
    }
    /// <summary>
    /// 动态调用
    /// </summary>
    public class Expand
    {
        public static bool Run<T>(out T result, string path, params object[] args)
        {
            result = default;
            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.dll");
            foreach (var file in files)
            {
                var types = Assembly.LoadFile(file.FullName).GetTypes();
                var inter = types.Where(c => c.IsInterface && c.Name == nameof(ISign)).FirstOrDefault();
                if (inter != null)
                {
                    var type = types.Where(c => inter.IsAssignableFrom(c)).FirstOrDefault();
                    if (type != null)
                    {
                        var sign = Activator.CreateInstance(type);
                        return Method.ExecuteMethod(sign, nameof(ISign.Result), out result, args);
                    }
                }
            }
            return false;
        }
    }
}
