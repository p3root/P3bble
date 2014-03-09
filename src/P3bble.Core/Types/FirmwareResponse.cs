using System;
using System.Runtime.Serialization;
using P3bble.Helper;

namespace P3bble.Types
{
    /// <summary>
    /// Represents firmware information from the cloud
    /// </summary>
    [DataContract]
    public class FirmwareResponse
    {
        /// <summary>
        /// Gets the recovery firmware version.
        /// </summary>
        /// <value>
        /// The recovery firmware version.
        /// </value>
        [DataMember(Name = "recovery")]
        public FirmwareRelease RecoveryFirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        /// <value>
        /// The firmware version.
        /// </value>
        [DataMember(Name = "normal")]
        public FirmwareRelease FirmwareVersion { get; private set; }
    }
}
