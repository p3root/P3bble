using System;
using System.Runtime.Serialization;
using P3bble.Helper;

namespace P3bble.Types
{
    /// <summary>
    /// Gets the details of bundle resources
    /// </summary>
    [DataContract]
    public class ResourcesManifest : VersionInfo
    {
        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [DataMember(Name = "name", IsRequired = true)]
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the CRC.
        /// </summary>
        /// <value>
        /// The CRC.
        /// </value>
        [DataMember(Name = "crc", IsRequired = true)]
        public uint CRC { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        [DataMember(Name = "size", IsRequired = true)]
        public int Size { get; private set; }
    }
}
