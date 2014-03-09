using System;
using System.Collections.Generic;
using System.Linq;
using P3bble.Constants;

namespace P3bble.Messages
{
    internal class PhoneVersionMessage : P3bbleMessage
    {
        private uint _sessionCaps = (uint)SessionCaps.GammaRay;
        private uint _remoteCaps = (uint)(RemoteCaps.Telephony | RemoteCaps.Sms | RemoteCaps.Android | RemoteCaps.Gps);

        public PhoneVersionMessage()
            : base(Endpoint.PhoneVersion)
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
