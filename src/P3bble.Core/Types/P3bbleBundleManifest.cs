using P3bble.Core.Types;
using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Types
{
    [DataContract]
    internal class P3bbleBundleManifest
    {
        [DataContract]
        public struct P3bbleApplicationManifest
        {
            [DataMember(Name = "name", IsRequired = true)]
            public string Filename { get; private set; }

            [DataMember(Name = "reqFwVer", IsRequired = true)]
            public int RequiredFirmwareVersion { get; private set; }

            [DataMember(Name = "timestamp", IsRequired = true)]
            internal int TimestampInternal { get; set; }
            public DateTime Timestamp { get { return TimestampInternal.AsDateTime(); } }

            [DataMember(Name = "crc", IsRequired = true)]
            public uint CRC { get; private set; }

            [DataMember(Name = "size", IsRequired = true)]
            public int Size { get; private set; }
        }

        [DataContract]
        public struct P3bbleFirmwareManifest
        {
            [DataMember(Name = "name", IsRequired = true)]
            public string Filename { get; private set; }

            [DataMember(Name = "timestamp", IsRequired = true)]
            internal int TimestampInternal { get; set; }
            public DateTime Timestamp { get { return TimestampInternal.AsDateTime(); } }

            [DataMember(Name = "crc", IsRequired = true)]
            public uint CRC { get; private set; }

            [DataMember(Name = "hwrev", IsRequired = true)]
            public string HardwareRevision { get; private set; }

            [DataMember(Name = "type", IsRequired = true)]
            public string Type { get; private set; }
            public bool IsRecovery { get { return (Type == "recovery"); } }

            [DataMember(Name = "size", IsRequired = true)]
            public int Size { get; private set; }
        }

        [DataContract]
        public class P3bbleResourcesManifest : P3bbleVersion
        {
            [DataMember(Name = "name", IsRequired = true)]
            public string Filename { get; private set; }

            [DataMember(Name = "crc", IsRequired = true)]
            public uint CRC { get; private set; }

            [DataMember(Name = "size", IsRequired = true)]
            public int Size { get; private set; }
        }

        [DataMember(Name = "manifestVersion", IsRequired = true)]
        public int ManifestVersion { get; private set; }

        [DataMember(Name = "generatedAt", IsRequired = true)]
        internal int GeneratedAtInternal { get; set; }
        public DateTime GeneratedAt { get { return GeneratedAtInternal.AsDateTime(); } }

        [DataMember(Name = "generatedBy", IsRequired = true)]
        public string GeneratedBy { get; private set; }

        [DataMember(Name = "application", IsRequired = false)]
        public P3bbleApplicationManifest Application { get; private set; }

        [DataMember(Name = "firmware", IsRequired = false)]
        public P3bbleFirmwareManifest Firmware { get; private set; }

        [DataMember(Name = "resources", IsRequired = false)]
        public P3bbleResourcesManifest Resources { get; private set; }

        [DataMember(Name = "type", IsRequired = true)]
        public string Type { get; private set; }
    }
}
