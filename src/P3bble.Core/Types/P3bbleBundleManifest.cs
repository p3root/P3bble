using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    [DataContract]
    internal class P3bbleBundleManifest
    {
        [DataMember(Name = "manifestVersion", IsRequired = true)]
        public int ManifestVersion { get; private set; }

        public DateTime GeneratedAt
        {
            get
            {
                return this.GeneratedAtInternal.AsDateTime();
            }
        }

        [DataMember(Name = "generatedBy", IsRequired = true)]
        public string GeneratedBy { get; private set; }

        [DataMember(Name = "application", IsRequired = false)]
        public P3bbleApplicationManifest ApplicationManifest { get; private set; }

        [DataMember(Name = "firmware", IsRequired = false)]
        public P3bbleFirmwareManifest Firmware { get; private set; }

        [DataMember(Name = "resources", IsRequired = false)]
        public P3bbleResourcesManifest Resources { get; private set; }

        [DataMember(Name = "type", IsRequired = true)]
        public string Type { get; private set; }

        [DataMember(Name = "generatedAt", IsRequired = true)]
        internal int GeneratedAtInternal { get; set; }
    }
}
