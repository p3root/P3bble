using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core
{
    public class FirmwareVersion
    {
        public DateTime Timestamp { get; private set; }
        public String Version { get; private set; }
        public String Commit { get; private set; }
        public Boolean IsRecovery { get; private set; }
        public byte HardwarePlatform { get; private set; }
        public byte MetadataVersion { get; private set; }

        public FirmwareVersion(DateTime timestamp, String version, String commit,
            bool isrecovery, byte hardwareplatform, byte metadataversion)
        {
            Timestamp = timestamp;
            Version = version;
            Commit = commit;
            IsRecovery = isrecovery;
            HardwarePlatform = hardwareplatform;
            MetadataVersion = metadataversion;
        }

        public override string ToString()
        {
            String format = "Version {0}, commit {1} ({2})\n"
                + "Recovery:         {3}\n"
                + "HW Platform:      {4}\n"
                + "Metadata version: {5}";
            return String.Format(format, Version, Commit, Timestamp, IsRecovery, HardwarePlatform, MetadataVersion);
        }
    }
}
