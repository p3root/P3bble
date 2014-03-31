using System;
using System.Runtime.Serialization;
using P3bble.Helper;

namespace P3bble.Types
{
    [DataContract]
    internal class BundleManifest
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
        public ApplicationManifest ApplicationManifest { get; private set; }

        [DataMember(Name = "firmware", IsRequired = false)]
        public FirmwareManifest Firmware { get; private set; }

        [DataMember(Name = "resources", IsRequired = false)]
        public ResourcesManifest Resources { get; private set; }

        [DataMember(Name = "type", IsRequired = true)]
        public string Type { get; private set; }

        [DataMember(Name = "generatedAt", IsRequired = true)]
        internal int GeneratedAtInternal { get; set; }
    }
}
