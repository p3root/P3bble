using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using P3bble.Core.Constants;
using P3bble.Core.Types;

namespace P3bble.Core.Messages
{
    internal class VersionMessage : P3bbleMessage
    {
        public VersionMessage()
            : base(P3bbleEndpoint.Version)
        {
        }

        public P3bbleFirmwareVersion Firmware { get; private set; }

        public P3bbleFirmwareVersion RecoveryFirmware { get; private set; }

        protected override ushort PayloadLength
        {
            get
            {
                return 1;
            }
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

        private static P3bbleFirmwareVersion ParseVersion(byte[] data)
        {
            byte[] _ts = data.Take(4).ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(_ts);
            }

            int timestamp = BitConverter.ToInt32(_ts, 0);
            string version = Encoding.UTF8.GetString(data.Skip(4).Take(32).ToArray(), 0, 32);
            string commit = Encoding.UTF8.GetString(data.Skip(36).Take(8).ToArray(), 0, 8);
            version = version.Substring(0, version.IndexOf('\0'));
            commit = commit.Substring(0, commit.IndexOf('\0'));
            bool is_recovery = BitConverter.ToBoolean(data, 44);
            byte hardware_platform = data[45];
            byte metadata_ver = data[46];
            return new P3bbleFirmwareVersion(timestamp, version, commit, is_recovery, hardware_platform, metadata_ver);
        }
    }
}
