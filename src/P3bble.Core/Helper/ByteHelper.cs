using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Helper
{
    internal class ByteHelper
    {
        public static byte[] ConvertToByteArray(int value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] ConvertToByteArray(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] ConvertToByteArray(ushort value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
