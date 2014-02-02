using System;
using System.Collections.Generic;
using System.Linq;
using P3bble.Core.Constants;

namespace P3bble.Core.Messages
{
    internal class PhoneVersionMessage : P3bbleMessage
    {
        private uint _sessionCaps = (uint)P3bbleSessionCaps.GammaRay;
        private uint _remoteCaps = (uint)(P3bbleRemoteCaps.Telephony | P3bbleRemoteCaps.Sms | P3bbleRemoteCaps.Android | P3bbleRemoteCaps.Gps);

        public PhoneVersionMessage()
            : base(P3bbleEndpoint.PhoneVersion)
        {
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            byte[] prefix = { 0x01, 0xFF, 0xFF, 0xFF, 0xFF };
            byte[] session = BitConverter.GetBytes(this._sessionCaps);
            byte[] remote = BitConverter.GetBytes(this._remoteCaps);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(session);
                Array.Reverse(remote);
            }

            byte[] msg = new byte[0];
            msg = msg.Concat(prefix).Concat(session).Concat(remote).ToArray();

            payload.AddRange(msg);
        }
    }
}
