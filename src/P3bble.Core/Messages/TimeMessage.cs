using P3bble.Core.Constants;
using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal enum TimeMessageAction
    {
        GetTime = 0,
        GetTimeResponse = 1,
        SetTime =2
    }
    internal class TimeMessage : P3bbleMessage
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
            if (_action == TimeMessageAction.GetTime)
            {
                base.AddContentToMessage(payload);
                payload.Add((int)TimeMessageAction.GetTime);

            }
            else if (_action == TimeMessageAction.SetTime)
            {
                double timestamp = Util.DateTimeToTimeStamp(_dateTime);

                byte[] array = BitConverter.GetBytes((int)timestamp);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(array, 0, 4);
                }
                base.AddContentToMessage(payload);
                payload.Add((int)TimeMessageAction.SetTime);
                payload.AddRange(array.ToList());
            }
          
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            byte[] payloadArray = payload.ToArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(payloadArray, 1, 4);
            }

            int timestamp = BitConverter.ToInt32(payloadArray, 1);
            Time = Util.TimestampToDateTime(timestamp);
        }

        protected override ushort PayloadLength
        {
            get
            {
                if (_action == TimeMessageAction.GetTime)
                    return 1;
                else if (_action == TimeMessageAction.SetTime)
                    return 5;
                return base.PayloadLength;
            }
        }

        public DateTime Time { get; private set; }
    }
}
