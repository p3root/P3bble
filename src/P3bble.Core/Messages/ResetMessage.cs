using System.Collections.Generic;
using P3bble.Constants;

namespace P3bble.Messages
{
    internal class ResetMessage : P3bbleMessage
    {
        public ResetMessage()
            : base(Endpoint.Reset)
        {
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            payload.Add(0x00);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
        }
    }
}
