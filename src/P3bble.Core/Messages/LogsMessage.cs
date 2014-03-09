using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P3bble.Constants;
using P3bble.Helper;

namespace P3bble.Messages
{
    /// <summary>
    /// The log level
    /// </summary>
    internal enum LogLevel
    {
        /// <summary>
        /// All levels
        /// </summary>
        All = 0,

        /// <summary>
        /// The error level
        /// </summary>
        Error = 1,

        /// <summary>
        /// The warning level
        /// </summary>
        Warning = 50,

        /// <summary>
        /// The information level
        /// </summary>
        Information = 100,

        /// <summary>
        /// The debug level
        /// </summary>
        Debug = 200,

        /// <summary>
        /// The verbose level
        /// </summary>
        Verbose = 250
    }

    internal class LogsMessage : P3bbleMessage
    {
        public LogsMessage()
            : base(Endpoint.Logs)
        {
        }

        public DateTime Timestamp { get; private set; }
        
        public short LineNo { get; private set; }
        
        public string Filename { get; private set; }

        public string Message { get; private set; }

        public LogLevel LogLevel
        {
            get
            {
                return (LogLevel)this.LevelInternal;
            }
        }

        internal byte LevelInternal { get; set; }

        protected override void AddContentToMessage(List<byte> payload)
        {
        }

        protected override void GetContentFromMessage(List<byte> payload)
        {
            byte[] payloadArray = payload.ToArray();
            byte[] metadata = new byte[8];
            byte msgsize;
            Array.Copy(payloadArray, metadata, 8);
            /* 
             * Unpack the metadata.  Eight bytes:
             * 0..3 -> integer timestamp
             * 4    -> Message level (severity)
             * 5    -> Size of the message
             * 6..7 -> Line number (?)
             */
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(metadata);
                this.Timestamp = BitConverter.ToInt32(metadata, 4).AsDateTime();
                this.LevelInternal = metadata[3];
                msgsize = metadata[2];
                this.LineNo = BitConverter.ToInt16(metadata, 0);
            }
            else
            {
                this.Timestamp = BitConverter.ToInt32(metadata, 0).AsDateTime();
                this.LevelInternal = metadata[4];
                msgsize = metadata[5];
                this.LineNo = BitConverter.ToInt16(metadata, 6);
            }

            // Now to extract the actual data
            byte[] _filename = new byte[16];
            byte[] _data = new byte[msgsize];
            Array.Copy(payloadArray, 8, _filename, 0, 16);
            Array.Copy(payloadArray, 24, _data, 0, msgsize);

            this.Filename = Encoding.UTF8.GetString(_filename, 0, 16);
            this.Message = Encoding.UTF8.GetString(_data, 0, msgsize);
        }
    }
}
