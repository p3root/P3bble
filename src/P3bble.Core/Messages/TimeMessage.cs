using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    public enum TimeMessageAction
    {
        GetTime,
        SetTime
    }
    public class TimeMessage : P3bbleMessage
    {
        private TimeMessageAction _action;
        private DateTime _dateTime;

        public TimeMessage()
            : base(P3bbleEndpoint.Time)
        {
            _action = TimeMessageAction.GetTime;

        }
        public TimeMessage(TimeMessageAction action, DateTime dt)
            : base(P3bbleEndpoint.Time)
        {
            _dateTime = dt;
            _action = action;
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);

            if (_action == TimeMessageAction.GetTime)
                payload.Add(0x00);
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            base.GetContentFromMessage(payload);
        }

        protected override ushort PayloadLength
        {
            get
            {
                if (_action == TimeMessageAction.GetTime)
                    return 1;
                return base.PayloadLength;
            }
        }
    }
}
