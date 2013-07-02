using P3bble.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal class PingMessage : P3bbleMessage
    {
        private byte[] _cookie;

        public PingMessage(byte[] cookie = null )
            : base(P3bbleEndpoint.Ping)
        {
            if(cookie == null)
                _cookie = new byte[5] { 0x00, 0xEB, 0x00, 0x00, 0x00 };
            else 
                _cookie = cookie;
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
            
            foreach (byte cur in _cookie)
            {
                payload.Add(cur);
            }

        }

        protected override ushort PayloadLength
        {
            get 
            {
                if (_cookie != null)
                    return (ushort)_cookie.Length;
                return 0;
            }
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            Cookie = BitConverter.ToUInt32(payload.ToArray(), 1);
        }

        public UInt32 Cookie { get; private set; }
    }
}
