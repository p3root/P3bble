using System.Collections.Generic;
using P3bble.Core.Constants;

namespace P3bble.Core.Messages
{
    internal class ResetMessage : P3bbleMessage
    {
        public ResetMessage()
            : base(P3bbleEndpoint.Reset)
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
