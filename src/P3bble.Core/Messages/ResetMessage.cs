using P3bble.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal class ResetMessage : P3bbleMessage
    {
        public ResetMessage()
            : base(P3bbleEndpoint.Reset)
        {

        }

        protected override ushort PayloadLength
        {
            get
            {
                return 1;
            }
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
            payload.Add(0x00);
        }
    }
}
