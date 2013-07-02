using P3bble.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal class PhoneVersionMessage : P3bbleMessage
    {
        uint sessionCaps = (uint)P3bbleSessionCaps.GAMMA_RAY;
        uint remoteCaps = (uint)(P3bbleRemoteCaps.TELEPHONY | P3bbleRemoteCaps.SMS | P3bbleRemoteCaps.WINDOWS | P3bbleRemoteCaps.GPS);
        ushort length;

        public PhoneVersionMessage()
            : base(P3bbleEndpoint.PhoneVersion)
        {

        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            byte[] prefix = { 0x01, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] session = BitConverter.GetBytes(sessionCaps);
            byte[] remote = BitConverter.GetBytes(remoteCaps);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(session);
                Array.Reverse(remote);
            }

            byte[] msg = new byte[0];
            msg = msg.Concat(prefix).Concat(session).Concat(remote).ToArray();
            length = (ushort)msg.Length;

            base.AddContentToMessage(payload);

            payload.AddRange(msg);
        }

        protected override ushort PayloadLength
        {
            get
            {
                return length;
            }
        }
    }
}
