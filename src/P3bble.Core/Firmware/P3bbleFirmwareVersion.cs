using P3bble.Core.Constants;

namespace P3bble.Core.Firmware
{
    public class P3bbleFirmwareVersion : P3bbbleVersion
    {
        private const string URL = "http://pebblefw.s3.amazonaws.com/pebble/{0}/{1}/latest.json";

        public string Commit { get; private set; }
        public bool IsRecovery { get; private set; }
        public byte HardwarePlatform { get; private set; }
        public byte MetadataVersion { get; private set; }
        public P3bbleHardwareRevision HardwareRevision { get; set; }

        public P3bbleFirmwareVersion(int timestamp, string version, string commit,
            bool isrecovery, byte hardwareplatform, byte metadataversion)
        {
            base.TimestampInternal = timestamp;
            base.VersionInternal = version;
            Commit = commit;
            IsRecovery = isrecovery;
            HardwarePlatform = hardwareplatform;
            HardwareRevision = (P3bbleHardwareRevision)hardwareplatform;
            MetadataVersion = metadataversion;
        }

        public override string ToString()
        {
            string format = "Version {0}, commit {1} ({2})\n"
                + "Recovery:         {3}\n"
                + "HW Platform:      {4}\n"
                + "Metadata version: {5}";
            return string.Format(format, Version, Commit, Timestamp, IsRecovery, HardwarePlatform, MetadataVersion);
        }

        /// <summary>
        /// Gets the firmware server URL.
        /// </summary>
        /// <param name="useNightlyBuild">if set to <c>true</c> [use nightly build].</param>
        /// <returns></returns>
        public string GetFirmwareServerUrl(bool useNightlyBuild = false)
        {
            return string.Format(URL, HardwareRevision.ToString(), useNightlyBuild ? "nightly" : "release");
        }
    }
}
