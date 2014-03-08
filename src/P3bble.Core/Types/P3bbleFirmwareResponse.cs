using System;
using System.Runtime.Serialization;
using P3bble.Core.Helper;

namespace P3bble.Core.Types
{
    /// <summary>
    /// Represents firmware information from the cloud
    /// </summary>
    [DataContract]
    public class P3bbleFirmwareResponse
    {
        /// <summary>
        /// Gets the recovery firmware version.
        /// </summary>
        /// <value>
        /// The recovery firmware version.
        /// </value>
        [DataMember(Name = "recovery")]
        public P3bbleFirmwareRelease RecoveryFirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        /// <value>
        /// The firmware version.
        /// </value>
        [DataMember(Name = "normal")]
        public P3bbleFirmwareRelease FirmwareVersion { get; private set; }
    }
}
