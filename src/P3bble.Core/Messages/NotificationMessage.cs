using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3bble.Constants;

namespace P3bble.Messages
{
    /// <summary>
    /// Represents a notification type
    /// </summary>
    internal enum NotificationType
    {
        /// <summary>
        /// The email type
        /// </summary>
        Email = 0,

        /// <summary>
        /// The SMS type
        /// </summary>
        SMS = 1,

        /// <summary>
        /// The facebook type
        /// </summary>
        Facebook = 2
    }

    internal class NotificationMessage : P3bbleMessage
    {
        private NotificationType _type;
        private List<string> _parts;

        public NotificationMessage(NotificationType type, params string[] parts)
            : base(Endpoint.Notification)
        {
            this._type = type;
            this._parts = parts.ToList();
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            string[] ts = { (new DateTime(1970, 1, 1) - DateTime.Now).TotalSeconds.ToString() };
            string[] parts = this._parts.Take(2).Concat(ts).Concat(this._parts.Skip(2)).ToArray();
            byte[] data = { (byte)this._type };

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

            payload.AddRange(data);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
        }
    }
}
