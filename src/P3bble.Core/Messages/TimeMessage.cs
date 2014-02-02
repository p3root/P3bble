using System;
using System.Collections.Generic;
using System.Diagnostics;
using P3bble.Core.Constants;
using P3bble.Core.Helper;

namespace P3bble.Core.Messages
{
    /// <summary>
    /// Represents the time message
    /// </summary>
    internal enum TimeMessageAction : byte
    {
        /// <summary>
        /// Get the time
        /// </summary>
        GetTime = 0,

        /// <summary>
        /// Set the time
        /// </summary>
        SetTime = 2
    }

    internal class TimeMessage : P3bbleMessage
    {
        private TimeMessageAction _action;
        
        public TimeMessage()
            : base(P3bbleEndpoint.Time)
        {
            this._action = TimeMessageAction.GetTime;
        }

        public TimeMessage(DateTime time)
            : base(P3bbleEndpoint.Time)
        {
            this._action = TimeMessageAction.SetTime;
            this.Time = time;
        }

        public DateTime Time { get; private set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
            if (this._action == TimeMessageAction.GetTime)
            {
                payload.Add((byte)this._action);
            }
            else
            {
                Debug.WriteLine("Set Time to " + this.Time.ToString());

                byte[] rawTime = BitConverter.GetBytes(this.Time.AsEpoch());
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(rawTime);
                }

                // Magic number required for the time to be set!
                payload.Add((byte)this._action);

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
            this.Time = timestamp.AsDateTime();
        }
    }
}
