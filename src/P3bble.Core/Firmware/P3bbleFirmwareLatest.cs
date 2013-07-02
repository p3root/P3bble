using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Firmware
{
    [DataContract]
    public class P3bbleFirmwareLatestInfo
    {
        [DataMember(Name = "url", IsRequired = true)]
        public String Url { get; private set; }

        [DataMember(Name="timestamp", IsRequired=true)]
        public int Timestamp { get; private set; }

        [DataMember(Name = "notes", IsRequired = true)]
        public string Notes { get; private set; }

        [DataMember(Name = "friendlyVersion", IsRequired = true)]
        public string FriendlyVersion { get; private set; }

        [DataMember(Name = "sha-256", IsRequired = true)]
        public string Checksum { get; private set; }
    }

    [DataContract]
    public class P3bbleFirmwareLatest
    {
        [DataMember(Name = "recovery")]
        public P3bbleFirmwareLatestInfo Recovery { get; private set; }

        [DataMember(Name = "normal")]
        public P3bbleFirmwareLatestInfo Normal { get; private set; }
    }
}
