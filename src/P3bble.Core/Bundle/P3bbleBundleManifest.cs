using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Bundle
{
    [DataContract]
    internal class P3bbleBundleManifest
    {
        [DataContract]
        public struct P3bbleApplicationManifest
        {
            [DataMember(Name = "name", IsRequired = true)]
            public String Filename { get; private set; }

           [DataMember(Name = "reqFwVer", IsRequired = true)]
            public int RequiredFirmwareVersion { get; private set; }

            [DataMember(Name = "timestamp", IsRequired = true)]
            public int Timestamp { get; private set; }
            public DateTime TimestampDT { get { return Timestamp.AsDateTime(); } }

           [DataMember(Name = "crc", IsRequired = true)]
            public uint CRC { get; private set; }

            [DataMember(Name = "size", IsRequired = true)]
            public int Size { get; private set; }
        }

         [DataContract]
        public struct P3bbleFirmwareManifest
        {
           [DataMember(Name = "name", IsRequired = true)]
            public String Filename { get; private set; }

            [DataMember(Name = "timestamp", IsRequired = true)]
            public int Timestamp { get; private set; }
            public DateTime TimestampDT { get { return Timestamp.AsDateTime(); } }

            [DataMember(Name = "crc", IsRequired = true)]
            public uint CRC { get; private set; }

            [DataMember(Name = "hwrev", IsRequired = true)]
            public String HardwareRevision { get; private set; }

            [DataMember(Name = "type", IsRequired = true)]
            public String Type { get; private set; }
            public bool IsRecovery { get { return (Type == "recovery"); } }

            [DataMember(Name = "size", IsRequired = true)]
            public int Size { get; private set; }
        }

        [DataContract]
        public struct P3bbleResourcesManifest
        {
             [DataMember(Name = "name", IsRequired = true)]
            public String Filename { get; private set; }

            [DataMember(Name = "timestamp", IsRequired = true)]
            public int Timestamp { get; private set; }
            public DateTime TimestampDT { get { return Timestamp.AsDateTime(); } }

            [DataMember(Name = "crc", IsRequired = true)]
            public uint CRC { get; private set; }

            [DataMember(Name = "friendlyVersion", IsRequired = true)]
            public String FriendlyVersion { get; private set; }

             [DataMember(Name = "size", IsRequired = true)]
            public int Size { get; private set; }
        }

        [DataMember(Name = "manifestVersion", IsRequired = true)]
        public int ManifestVersion { get; private set; }

        [DataMember(Name = "generatedAt", IsRequired = true)]
        public int GeneratedAt { get; private set; }

        public DateTime GeneratedAtDT { get { return GeneratedAt.AsDateTime(); } }

        [DataMember(Name = "generatedBy", IsRequired = true)]
        public String GeneratedBy { get; private set; }

        [DataMember(Name = "application", IsRequired = false)]
        public P3bbleApplicationManifest Application { get; private set; }

        [DataMember(Name = "firmware", IsRequired = false)]
        public P3bbleFirmwareManifest Firmware { get; private set; }

        [DataMember(Name = "resources", IsRequired = false)]
        public P3bbleResourcesManifest Resources { get; private set; }

        [DataMember(Name = "type", IsRequired = true)]
        public String Type { get; private set; }
    }
}
