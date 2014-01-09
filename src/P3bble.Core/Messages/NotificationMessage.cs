using P3bble.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal enum NotificationType
    {
        EMAIL = 0,
        SMS = 1,
        Facebook = 2
    }

    internal class NotificationMessage : P3bbleMessage
    {
        private NotificationType _type;
        private List<string> _parts;
        private ushort _length;

        public NotificationMessage(NotificationType type, params string[] parts)
            : base(P3bbleEndpoint.Notification)
        {
            _type = type;
            _parts = parts.ToList();
            _length = 0;
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            string[] ts = { (new DateTime(1970, 1, 1) - DateTime.Now).TotalSeconds.ToString() };
            string[] parts = _parts.Take(2).Concat(ts).Concat(_parts.Skip(2)).ToArray();
            byte[] data = { (byte)_type };

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
