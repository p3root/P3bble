using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P3bble.Core.Constants;

namespace P3bble.Core.Messages
{
    /// <summary>
    /// Application command
    /// </summary>
    internal enum AppCommand : byte
    {
        /// <summary>
        /// The push response
        /// </summary>
        Push = 1,

        /// <summary>
        /// The request response
        /// </summary>
        Request = 2,

        /// <summary>
        /// The finalise install command
        /// </summary>
        FinaliseInstall = 3,

        /// <summary>
        /// The ack response
        /// </summary>
        Ack = 0xff,

        /// <summary>
        /// The nack response
        /// </summary>
        Nack = 0x7f
    }

    /// <summary>
    /// Application Message Data Types
    /// </summary>
    internal enum AppMessageTupleDataType : byte
    {
        /// <summary>
        /// A byte array
        /// </summary>
        ByteArray = 0,

        /// <summary>
        /// A string
        /// </summary>
        String = 1,

        /// <summary>
        /// An unsigned integer
        /// </summary>
        UInt = 2,

        /// <summary>
        /// An integer
        /// </summary>
        Int = 3
    }

    /// <summary>
    /// Launcher keys
    /// </summary>
    internal enum LauncherKeys : byte
    {
        /// <summary>
        /// The run state key
        /// </summary>
        RunState = 1
    }

    /// <summary>
    /// Application launch param
    /// </summary>
    internal enum LauncherParams : byte
    {
        /// <summary>
        /// The not running state
        /// </summary>
        NotRunning = 0,

        /// <summary>
        /// The running state
        /// </summary>
        Running = 1
    }

    internal class AppMessage : P3bbleMessage
    {
        internal const byte RunState = 1;

        private List<byte[]> _tuples = new List<byte[]>();
        
        public AppMessage()
            : this(P3bbleEndpoint.ApplicationMessage)
        {
        }

        public AppMessage(P3bbleEndpoint messageType)
            : base(messageType)
        {
        }

        public Guid AppUuid { get; set; }

        public AppCommand AppCommand { get; set; }

        public uint AppIndex { get; set; }

        /// <summary>
        /// Gets or sets the remaining response.
        /// </summary>
        /// <value>
        /// The remaining response.
        /// </value>
        /// <remarks>Not expecting this to get used</remarks>
        private byte[] RemainingResponse { get; set; }

        public void AddTuple(uint key, AppMessageTupleDataType dataType, byte data)
        {
            this.AddTuple(key, dataType, new byte[] { data });
        }
        
        public void AddTuple(uint key, AppMessageTupleDataType dataType, byte[] data)
        {
            List<byte> result = new List<byte>();

            byte[] keyBytes = BitConverter.GetBytes(key);
            byte[] lengthBytes = BitConverter.GetBytes((short)data.Length);

            //if (BitConverter.IsLittleEndian)
            //{
            //    // Data is transmitted big-endian, so flip.
            //    Array.Reverse(keyBytes);
            //    Array.Reverse(lengthBytes);
            //}
            
            result.AddRange(keyBytes);
            result.Add((byte)dataType);
            result.AddRange(lengthBytes);
            result.AddRange(data);

            this._tuples.Add(result.ToArray());
        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            // Add the command
            payload.Add((byte)this.AppCommand);

            if (this.AppCommand == AppCommand.FinaliseInstall)
            {
                byte[] indexBytes = BitConverter.GetBytes(this.AppIndex);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(indexBytes);
                }

                payload.AddRange(indexBytes);
            }
            else
            {
                List<byte> data = new List<byte>();

                // Add a transaction id:
                data.Add(0);

                // Add the app id:
                data.AddRange(this.AppUuid.ToByteArray());

                // Add the actual data to send - first the count...
                data.Add((byte)(this._tuples.Count * 4));

                // Now the tuples...
                foreach (var tuple in this._tuples)
                {
                    data.AddRange(tuple);
                }

                payload.AddRange(data);
            }
        }

        protected override void GetContentFromMessage(System.Collections.Generic.List<byte> payload)
        {
            if (payload.Count > 0)
            {
                switch (payload[0])
                {
                    case (byte)AppCommand.Push:
                    case (byte)AppCommand.Request:
                    case (byte)AppCommand.Ack:
                    case (byte)AppCommand.Nack:

                        this.AppCommand = (AppCommand)payload[0];
                        break;
                    
                    default:
                        break;
                }

                if (payload.Count > 1)
                {
                    this.RemainingResponse = new byte[payload.Count - 1];
                    payload.CopyTo(1, this.RemainingResponse, 0, payload.Count - 1);
                }
            }
        }
    }
}
