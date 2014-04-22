using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace P3bble.Helper
{
    internal static class Util
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1);

        public static DateTime AsDateTime(this int ts)
        {
            return _epoch.AddSeconds(ts);
        }

        public static int AsEpoch(this DateTime time)
        {
            return Convert.ToInt32((time - _epoch).TotalSeconds);
        }

        public static double DateTimeToTimeStamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static T AsStruct<T>(this Stream fs) where T : struct
        {
            // Borrowed from http://stackoverflow.com/a/1936208 because BitConverter-ing all of this would be a pain
#if NETFX_CORE  && !WINDOWS_PHONE_APP
            byte[] buffer = new byte[Marshal.SizeOf<T>()];
#else
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
#endif
            fs.Read(buffer, 0, buffer.Length);
            return AsStruct<T>(buffer);
        }

        public static T AsStruct<T>(this byte[] bytes) where T : struct
        {
#if NETFX_CORE  && !WINDOWS_PHONE_APP
            if (bytes.Count() != Marshal.SizeOf<T>())
#else
            if (bytes.Count() != Marshal.SizeOf(typeof(T)))
#endif
            {
                throw new ArgumentException("Byte array does not match size of target type.");
            }

            T ret;
            GCHandle hdl = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
#if NETFX_CORE  && !WINDOWS_PHONE_APP
                ret = (T)Marshal.PtrToStructure<T>(hdl.AddrOfPinnedObject());
#else
                ret = (T)Marshal.PtrToStructure(hdl.AddrOfPinnedObject(), typeof(T));
#endif
            }
            finally
            {
                hdl.Free();
            }

            return ret;
        }

        public static Version AsVersion(this string version)
        {
            return new Version(version.Remove(0, 1));
        }

        public static uint Crc32(this List<byte> data)
        {
            return CrcProcessBuffer(data);
        }

        private static uint CrcProcessBuffer(List<byte> data, uint crc = 0xffffffff)
        {
            int wordCount = data.Count / 4;
            if (data.Count % 4 != 0)
            {
                wordCount += 1;
            }

            for (int i = 0; i < wordCount; i++)
            {
                byte[] word = new byte[4];
                int len = Math.Min(data.Count - (i * 4), 4);
                data.CopyTo(i * 4, word, 4 - len, len);
                if (len < 4)
                {
                    Array.Reverse(word);
                }

                crc = CrcProcessWord(word, crc);
            }

            return crc;
        }

        private static uint CrcProcessWord(byte[] data, uint crc = 0xffffffff)
        {
            const uint CRC_POLY = 0x04C11DB7;
            List<byte> dataArray = new List<byte>(data);

            crc = crc ^ BitConverter.ToUInt32(dataArray.ToArray(), 0);

            for (int i = 0; i < 32; i++)
            {
                if ((crc & 0x80000000) != 0)
                {
                    crc = (crc << 1) ^ CRC_POLY;
                }
                else
                {
                    crc = crc << 1;
                }
            }

            return crc & 0xffffffff;
        }
    }
}
