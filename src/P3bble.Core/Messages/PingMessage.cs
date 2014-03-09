using System;
using System.Collections.Generic;
using P3bble.Constants;

namespace P3bble.Messages
{
    internal class PingMessage : P3bbleMessage
    {
        private byte[] _cookie;

        public PingMessage(byte[] cookie = null)
            : base(Endpoint.Ping)
        {
            if (cookie == null)
            {
                this._cookie = new byte[5] { 0x00, 0xEB, 0x00, 0x00, 0x00 };
            }
            else
            {
                this._cookie = cookie;
            }
        }

        public uint Cookie { get; private set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
            payload.AddRange(this._cookie);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            this.Cookie = BitConverter.ToUInt32(payload.ToArray(), 1);
        }
    }
}
