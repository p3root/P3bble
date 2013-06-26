using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Helper
{
    public class ByteHelper
    {
        public static byte[] ConvertToByteArray(Int32 I32)
        {
            return BitConverter.GetBytes(I32);
        }

        public static byte[] ConvertToByteArray(uint uin)
        {
            return BitConverter.GetBytes(uin);
        }

        public static byte[] ConvertToByteArray(ushort uin)
        {
            return BitConverter.GetBytes(uin);
        }
    }
}
