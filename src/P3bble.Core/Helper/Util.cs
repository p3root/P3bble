using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Helper
{
    internal class Util
    {
        public static DateTime TimestampToDateTime(Int32 ts)
        {
            return new DateTime(1970, 1, 1).AddSeconds(ts);
        }

        public static T ReadStruct<T>(Stream fs) where T : struct
        {
            // Borrowed from http://stackoverflow.com/a/1936208 because BitConverter-ing all of this would be a pain
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            fs.Read(buffer, 0, buffer.Length);
            return ReadStruct<T>(buffer);
        }

        public static T ReadStruct<T>(byte[] bytes) where T : struct
        {
            if (bytes.Count() != Marshal.SizeOf(typeof(T)))
            {
                throw new ArgumentException("Byte array does not match size of target type.");
            }
            T ret;
            GCHandle hdl = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                ret = (T)Marshal.PtrToStructure(hdl.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                hdl.Free();
            }
            return ret;
        }

        private class Version
        {
            private int _first;
            private int _second;
            private int _last;

            public static Version ParseFromString(string version)
            {
                version = version.Remove(0, 1);

                string[] split = version.Split('.');

                if (split.Length == 3)
                {
                    Version vesr = new Version();
                    vesr._first = Convert.ToInt32(split[0]);
                    vesr._second = Convert.ToInt32(split[1]);
                    vesr._last = Convert.ToInt32(split[2]);
                    return vesr;
                }

                return null;
            }

            public static bool operator >(Version v1, Version v2)
            {
                if (v1._first > v2._first)
                    return true;
                if (v1._second > v2._second)
                    return true;
                if (v1._last > v2._last)
                    return true;

                return false;
            }

            public static bool operator <(Version v1, Version v2)
            {
                if (v1._first < v2._first)
                    return true;
                if (v1._second < v2._second)
                    return true;
                if (v1._last < v2._last)
                    return true;

                return false;
            }
        }

        public static bool IsNewerVersionAvailable(string currentVersion, string serverVersion)
        {
            Version current = Version.ParseFromString(currentVersion);
            Version server = Version.ParseFromString(serverVersion);

            if (server > current)
                return true;


            return false;
        }
    }
}
