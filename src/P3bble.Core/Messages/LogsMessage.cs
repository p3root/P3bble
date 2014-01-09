using P3bble.Core.Constants;
using P3bble.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3bble.Core.Messages
{
    internal class LogsMessage : P3bbleMessage
    {
        public LogsMessage()
            : base(P3bbleEndpoint.Logs)
        {

        }

        protected override void AddContentToMessage(List<byte> payload)
        {
            base.AddContentToMessage(payload);
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
                Timestamp = BitConverter.ToInt32(metadata, 4).AsDateTime();
                Level = metadata[3];
                msgsize = metadata[2];
                LineNo = BitConverter.ToInt16(metadata, 0);
            }
            else
            {
                Timestamp = BitConverter.ToInt32(metadata, 0).AsDateTime();
                Level = metadata[4];
                msgsize = metadata[5];
                LineNo = BitConverter.ToInt16(metadata, 6);
            }
            // Now to extract the actual data
            byte[] _filename = new byte[16];
            byte[] _data = new byte[msgsize];
            Array.Copy(payloadArray, 8, _filename, 0, 16);
            Array.Copy(payloadArray, 24, _data, 0, msgsize);

            Filename = Encoding.UTF8.GetString(_filename, 0, 16);
            Message = Encoding.UTF8.GetString(_data, 0, msgsize);
        
        }

        protected override ushort PayloadLength
        {
            get
            {
                return base.PayloadLength;
            }
        }

        public DateTime Timestamp { get; private set; }
        public byte Level { get; private set; }
        public Int16 LineNo { get; private set; }
        public String Filename { get; private set; }
        public String Message { get; private set; }
    }
}
