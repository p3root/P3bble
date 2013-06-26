using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Helper
{
    public class Util
    {
        public static DateTime TimestampToDateTime(Int32 ts)
        {
            return new DateTime(1970, 1, 1).AddSeconds(ts);
        }
    }
}
