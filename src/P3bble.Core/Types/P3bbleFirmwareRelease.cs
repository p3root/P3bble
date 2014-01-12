using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    [DataContract]
    public class P3bbleFirmwareRelease : P3bbleVersion
    {
        [DataMember(Name = "url", IsRequired = true)]
        public string Url { get; private set; }

        [DataMember(Name = "notes", IsRequired = true)]
        public string Notes { get; private set; }

        [DataMember(Name = "sha-256", IsRequired = true)]
        public string Checksum { get; private set; }
    }
}
