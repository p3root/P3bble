using System;
using System.Collections.Generic;
using P3bble.Constants;
using P3bble.Helper;
using P3bble.Messages;

namespace P3bble
{
    /// <summary>
    /// The base message
    /// </summary>
    internal abstract class P3bbleMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="P3bbleMessage"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public P3bbleMessage(Endpoint endpoint)
        {
            this.Endpoint = endpoint;
        }

        /// <summary>
        /// Gets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public Endpoint Endpoint { get; private set; }

        /// <summary>
        /// Creates an incoming message.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="payload">The payload.</param>
        /// <returns>A specific message type</returns>
        public static P3bbleMessage CreateMessage(Endpoint endpoint, List<byte> payload)
        {
            P3bbleMessage frame = null;

            switch (endpoint)
            {
                case Endpoint.Ping:
                    frame = new PingMessage();
                    break;

                case Endpoint.Version:
                    frame = new VersionMessage();
                    break;
                
                case Endpoint.Time:
                    frame = new TimeMessage();
                    break;
                
                case Endpoint.Logs:
                    frame = new LogsMessage();
                    break;

                case Endpoint.AppManager:
                    frame = new AppManagerMessage();
                    break;

                case Endpoint.MusicControl:
                    frame = new MusicMessage();
                    break;

                case Endpoint.ApplicationMessage:
                case Endpoint.Launcher:
                    frame = new AppMessage(endpoint);
                    break;

                case Endpoint.PutBytes:
                    frame = new PutBytesMessage();
                    break;
            }

            if (frame != null)
            {
                frame.GetContentFromMessage(payload);
            }

            return frame;
        }

        /// <summary>
        /// Serialises a message into a byte representation ready for sending
        /// </summary>
        /// <returns>A byte array</returns>
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

        /// <summary>
        /// Allows descendant classes to add content to the outgoing message.
        /// </summary>
        /// <param name="payload">The payload storage.</param>
        protected abstract void AddContentToMessage(List<byte> payload);

        /// <summary>
        /// Extracts content for the incoming message.
        /// </summary>
        /// <param name="payload">The payload.</param>
        protected abstract void GetContentFromMessage(List<byte> payload);
    }
}
