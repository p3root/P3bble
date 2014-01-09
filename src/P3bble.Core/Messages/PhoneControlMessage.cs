using P3bble.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal enum PhoneControlType : byte
    {
        ANSWER = 1,
        HANGUP = 2,
        GET_STATE = 3,
        INCOMING_CALL = 4,
        OUTGOING_CALL = 5,
        MISSED_CALL = 6,
        RING = 7,
        START = 8,
        END = 9
    }

    internal class PhoneControlMessage : P3bbleMessage
    {
        private PhoneControlType _type;
        private List<string> _parts;
        private ushort _length;
        private byte[] _cookie;

        public PhoneControlMessage(PhoneControlType type, byte[] cookie, params string[] parts)
            : base(P3bbleEndpoint.PhoneControl)
        {
            _type = type;
            _parts = parts.ToList();
            _length = 0;
            _cookie = cookie;
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            //payload.Add(_type);
            //payload.AddRange(_cookie);

            string[] parts = _parts.ToArray();
            byte[] data = { (byte)_type, };

            data = data.Concat(_cookie).ToArray();

            foreach (string part in parts)
            {
                byte[] _part = Encoding.UTF8.GetBytes(part);
                if (_part.Length > 255)
                {
                    _part = _part.Take(255).ToArray();
                }
                byte[] len = { Convert.ToByte(_part.Length) };
                data = data.Concat(len).Concat(_part).ToArray();
            }
            _length = (ushort)data.Length;

            base.AddContentToMessage(payload);
            payload.AddRange(data);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }

        protected override ushort PayloadLength
        {
            get
            {
                return _length;
            }
        }
    }
}
