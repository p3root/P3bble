using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
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
}
