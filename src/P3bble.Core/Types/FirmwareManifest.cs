using System;
using System.Runtime.Serialization;
using P3bble.Helper;

namespace P3bble.Types
{
    /// <summary>
    /// Represents details of a firmware bundle
    /// </summary>
    [DataContract]
    public struct FirmwareManifest
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
        /// Gets the hardware revision.
        /// </summary>
        /// <value>
        /// The hardware revision.
        /// </value>
        [DataMember(Name = "hwrev", IsRequired = true)]
        public string HardwareRevision { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DataMember(Name = "type", IsRequired = true)]
        public string Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the firmware is recovery.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recovery.
        /// </value>
        public bool IsRecovery
        {
            get
            {
                return this.Type == "recovery";
            }
        }

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
