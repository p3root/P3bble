using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    public class VersionMessage : P3bbleMessage
    {
        public VersionMessage()
            : base(P3bbleEndpoint.Version)
        {

        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
            payload.Add(0x00);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            byte[] data = payload.ToArray();

            this.Firmware = ParseVersion(data.Skip(1).Take(47).ToArray());
            this.RecoveryFirmware = ParseVersion(data.Skip(48).Take(47).ToArray());
        }

        private static FirmwareVersion ParseVersion(byte[] data)
        {
            byte[] _ts = data.Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(_ts);
            }
            DateTime timestamp = Util.TimestampToDateTime(BitConverter.ToInt32(_ts, 0));
            String version = Encoding.UTF8.GetString(data.Skip(4).Take(32).ToArray(), 0, 32);
            String commit = Encoding.UTF8.GetString(data.Skip(36).Take(8).ToArray(), 0, 8);
            version = version.Substring(0, version.IndexOf('\0'));
            commit = commit.Substring(0, commit.IndexOf('\0'));
            Boolean is_recovery = BitConverter.ToBoolean(data, 44);
            byte hardware_platform = data[45];
            byte metadata_ver = data[46];
            return new FirmwareVersion(timestamp, version, commit, is_recovery, hardware_platform, metadata_ver);
        }

        public FirmwareVersion Firmware { get; private set; }
        public FirmwareVersion RecoveryFirmware { get; private set; }

        protected override ushort PayloadLength
        {
            get
            {
                return 1;
            }
        }
    }
}
