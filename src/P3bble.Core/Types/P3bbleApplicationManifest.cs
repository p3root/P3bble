using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    [DataContract]
    public struct P3bbleApplicationManifest
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Filename { get; private set; }

        [DataMember(Name = "reqFwVer", IsRequired = true)]
        public int RequiredFirmwareVersion { get; private set; }

        public DateTime Timestamp
        {
            get
            {
                return this.TimestampInternal.AsDateTime();
            }
        }

        [DataMember(Name = "crc", IsRequired = true)]
        public uint CRC { get; private set; }

        [DataMember(Name = "size", IsRequired = true)]
        public int Size { get; private set; }

        [DataMember(Name = "timestamp", IsRequired = true)]
        internal int TimestampInternal { get; set; }
    }
}
