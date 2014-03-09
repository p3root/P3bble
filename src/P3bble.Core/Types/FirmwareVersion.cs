using P3bble.Constants;

namespace P3bble.Types
{
    /// <summary>
    /// Represents firmware version information from the Pebble
    /// </summary>
    public class FirmwareVersion : VersionInfo
    {
        private const string URL = "http://pebblefw.s3.amazonaws.com/pebble/{0}/{1}/latest.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="FirmwareVersion"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="version">The version.</param>
        /// <param name="commit">The commit.</param>
        /// <param name="isrecovery">if set to <c>true</c> [isrecovery].</param>
        /// <param name="hardwareplatform">The hardwareplatform.</param>
        /// <param name="metadataversion">The metadataversion.</param>
        public FirmwareVersion(int timestamp, string version, string commit, bool isrecovery, byte hardwareplatform, byte metadataversion)
        {
            this.TimestampInternal = timestamp;
            this.VersionInternal = version;
            this.Commit = commit;
            this.IsRecovery = isrecovery;
            this.HardwarePlatform = hardwareplatform;
            this.HardwareRevision = (HardwareRevision)hardwareplatform;
            this.MetadataVersion = metadataversion;
        }

        /// <summary>
        /// Gets the commit information.
        /// </summary>
        /// <value>
        /// The commit.
        /// </value>
        public string Commit { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is recovery firmware.
        /// </summary>
        /// <value>
        ///   <c>true</c> if recovery; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecovery { get; private set; }

        /// <summary>
        /// Gets the hardware platform.
        /// </summary>
        /// <value>
        /// The hardware platform.
        /// </value>
        public byte HardwarePlatform { get; private set; }

        /// <summary>
        /// Gets the metadata version.
        /// </summary>
        /// <value>
        /// The metadata version.
        /// </value>
        public byte MetadataVersion { get; private set; }

        /// <summary>
        /// Gets or sets the hardware revision.
        /// </summary>
        /// <value>
        /// The hardware revision.
        /// </value>
        public HardwareRevision HardwareRevision { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string format = "Version {0}, commit {1} ({2})\n"
                + "Recovery:         {3}\n"
                + "HW Platform:      {4}\n"
                + "Metadata version: {5}";
            return string.Format(format, this.Version, this.Commit, this.Timestamp, this.IsRecovery, this.HardwarePlatform, this.MetadataVersion);
        }

        /// <summary>
        /// Gets the firmware server URL.
        /// </summary>
        /// <param name="useNightlyBuild">if set to <c>true</c> [use nightly build].</param>
        /// <returns>The url</returns>
        public string GetFirmwareServerUrl(bool useNightlyBuild = false)
        {
            return string.Format(URL, this.HardwareRevision.ToString(), useNightlyBuild ? "nightly" : "release");
        }
    }
}
