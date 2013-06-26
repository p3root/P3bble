using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    public class VersionMessage : P3bbleMessage
    {
        public VersionMessage()
            : base(P3bbleEndpoint.Version)
        {

        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
            payload.Add(0x00);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);

            //TODO: PARSE VERSION
        }

        protected override ushort PayloadLength
        {
            get
            {
                return 1;
            }
        }
    }
}
