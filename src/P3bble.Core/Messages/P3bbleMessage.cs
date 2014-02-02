using System;
using System.Collections.Generic;
using P3bble.Core.Constants;
using P3bble.Core.Helper;
using P3bble.Core.Messages;

namespace P3bble.Core
{
    internal abstract class P3bbleMessage
    {
        public P3bbleMessage(P3bbleEndpoint endpoint)
        {
            this.Endpoint = endpoint;
        }

        public P3bbleEndpoint Endpoint { get; private set; }

        public static P3bbleMessage CreateMessage(P3bbleEndpoint endpoint, List<byte> payload)
        {
            P3bbleMessage frame = null;

            switch (endpoint)
            {
                case P3bbleEndpoint.Ping:
                    frame = new PingMessage();
                    break;

                case P3bbleEndpoint.Version:
                    frame = new VersionMessage();
                    break;
                
                case P3bbleEndpoint.Time:
                    frame = new TimeMessage();
                    break;
                
                case P3bbleEndpoint.Logs:
                    frame = new LogsMessage();
                    break;

                case P3bbleEndpoint.AppManager:
                    frame = new AppManagerMessage();
                    break;

                case P3bbleEndpoint.MusicControl:
                    frame = new MusicMessage();
                    break;

                case P3bbleEndpoint.ApplicationMessage:
                case P3bbleEndpoint.Launcher:
                    frame = new AppMessage(endpoint);
                    break;

                case P3bbleEndpoint.PutBytes:
                    frame = new PutBytesMessage();
                    break;
            }

            if (frame != null)
            {
                frame.GetContentFromMessage(payload);
            }

            return frame;
        }

        public byte[] ToBuffer()
        {
            List<byte> buf = new List<byte>();

            this.AddContentToMessage(buf);

            byte[] lengthBytes = ByteHelper.ConvertToByteArray(Convert.ToUInt16(buf.Count));
            byte[] endpointBytes = ByteHelper.ConvertToByteArray(Convert.ToUInt16(this.Endpoint));

            if (BitConverter.IsLittleEndian)
            {
                // Data is transmitted big-endian, so flip.
                Array.Reverse(lengthBytes);
                Array.Reverse(endpointBytes);
            }

            buf.Insert(0, lengthBytes[0]);
            buf.Insert(1, lengthBytes[1]);
            buf.Insert(2, endpointBytes[0]);
            buf.Insert(3, endpointBytes[1]);

            return buf.ToArray();
        }

        protected abstract void AddContentToMessage(List<byte> payload);

        protected abstract void GetContentFromMessage(List<byte> payload);
    }
}
