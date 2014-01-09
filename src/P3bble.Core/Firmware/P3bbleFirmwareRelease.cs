using P3bble.Core.Helper;
using System;
using System.Runtime.Serialization;

namespace P3bble.Core.Firmware
{
    [DataContract]
    public class P3bbleFirmwareRelease : P3bbbleVersion
    {
        [DataMember(Name = "url", IsRequired = true)]
        public string Url { get; private set; }

        [DataMember(Name = "notes", IsRequired = true)]
        public string Notes { get; private set; }

        [DataMember(Name = "sha-256", IsRequired = true)]
        public string Checksum { get; private set; }
    }

    [DataContract]
    public class P3bbleFirmwareLatest
    {
        [DataMember(Name = "recovery")]
        public P3bbleFirmwareRelease RecoveryFirmwareVersion { get; private set; }

        [DataMember(Name = "normal")]
        public P3bbleFirmwareRelease FirmwareVersion { get; private set; }
    }
}
