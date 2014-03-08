using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    /// <summary>
    /// Represents an application manifest
    /// </summary>
    [DataContract]
    public struct P3bbleApplicationManifest
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
        /// Gets the required firmware version.
        /// </summary>
        /// <value>
        /// The required firmware version.
        /// </value>
        [DataMember(Name = "reqFwVer", IsRequired = true)]
        public int RequiredFirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp
        {
            get
            {
                return this.TimestampInternal.AsDateTime();
            }
        }

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

        [DataMember(Name = "timestamp", IsRequired = true)]
        internal int TimestampInternal { get; set; }
    }
}
