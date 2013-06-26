using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    public class PingMessage : P3bbleMessage
    {
        public PingMessage()
            : base(P3bbleEndpoint.Ping)
        {

        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
            payload.Add(0xf1);
        }

        protected override ushort PayloadLength
        {
            get { return 1; }
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }
    }
}
