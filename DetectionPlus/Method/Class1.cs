using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectionPlus
{
    public class Class1 : ISign
    {
        public int Result(int i)
        {
            return 3 * i + 13;
        }
    }
    public interface ISign
    {
        int Result(int i);
    }
}
