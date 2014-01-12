using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    [DataContract]
    public class P3bbleFirmwareResponse
    {
        [DataMember(Name = "recovery")]
        public P3bbleFirmwareRelease RecoveryFirmwareVersion { get; private set; }

        [DataMember(Name = "normal")]
        public P3bbleFirmwareRelease FirmwareVersion { get; private set; }
    }
}
