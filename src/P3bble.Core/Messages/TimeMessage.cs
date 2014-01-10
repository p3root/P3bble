using P3bble.Core.Constants;
using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace P3bble.Core.Messages
{
    internal enum TimeMessageAction
    {
        GetTime,
        SetTime
    }

    internal class TimeMessage : P3bbleMessage
    {
        private TimeMessageAction _action;
        private ushort _length;

        public TimeMessage()
            : base(P3bbleEndpoint.Time)
        {
            _action = TimeMessageAction.GetTime;

        }

        public TimeMessage(DateTime time)
            : base(P3bbleEndpoint.Time)
        {
            _action = TimeMessageAction.SetTime;
            Time = time;
        }

        public DateTime Time { get; private set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
            if (_action == TimeMessageAction.GetTime)
            {
                base.AddContentToMessage(payload);
                payload.Add(0x00);
            }
            else
            {
                Debug.WriteLine("Set Time to " + Time.ToString());

                byte[] rawTime = BitConverter.GetBytes(Time.AsEpoch());
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(rawTime);
                }

                _length = (ushort)(rawTime.Length + 1);

                base.AddContentToMessage(payload);

                // Magic number required for the time to be set!
                payload.Add(2);

                payload.AddRange(rawTime);
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
            Time = timestamp.AsDateTime();
        }

        protected override ushort PayloadLength
        {
            get
            {
                if (_action == TimeMessageAction.GetTime)
                {
                    return 1;
                }
                else
                {
                    return _length;
                }
            }
        }
    }
}
