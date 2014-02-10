using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    public enum PutBytesType : byte
    {
        Firmware = 1,
        Recovery = 2,
        SystemResources = 3,
        Resources = 4,
        Binary = 5
    }

    public enum PutBytesState
    {
        NotStarted,
        WaitForToken,
        InProgress,
        Commit,
        Complete,
        Failed
    }

    internal class PutBytesMessage : P3bbleMessage
    {
        public PutBytesMessage(PutBytesType type)
            : base(Constants.P3bbleEndpoint.PutBytes)
        {

        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }

        protected override ushort PayloadLength
        {
            get
            {
                return base.PayloadLength;
            }
        }
    }
}
