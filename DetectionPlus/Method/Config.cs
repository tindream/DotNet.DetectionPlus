using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionPlus
{
    public class Config
    {
        public static string Images
        {
            get
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }
        public static string Expand
        {
            get
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Expand");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }
    }
}
