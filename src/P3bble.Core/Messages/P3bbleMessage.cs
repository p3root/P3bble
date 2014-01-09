﻿using P3bble.Core.Constants;
using P3bble.Core.Helper;
using P3bble.Core.Messages;
using System;
using System.Collections.Generic;

namespace P3bble.Core
{
    internal class P3bbleMessage
    {
        public P3bbleEndpoint Endpoint { get; private set; }

        public P3bbleMessage(P3bbleEndpoint endpoint)
        {
            this.Endpoint = endpoint;
        }

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

                default:
                    frame =  new P3bbleMessage(endpoint);
                    break;
            }

            frame.GetContentFromMessage(payload);
            return frame;
        }

        public byte[] ToBuffer()
        {
            List<byte> buf = new List<byte>();

            AddContentToMessage(buf);
            return buf.ToArray();
        }

        protected virtual ushort PayloadLength { get { return 0; } }

        protected virtual void AddContentToMessage(List<byte> payload) 
        {
            byte[] lengthBytes = ByteHelper.ConvertToByteArray(PayloadLength);
            byte[] endpointBytes = ByteHelper.ConvertToByteArray(Convert.ToUInt16(this.Endpoint));

            if (BitConverter.IsLittleEndian)
            {
                // Data is transmitted big-endian, so flip.
                Array.Reverse(lengthBytes);
                Array.Reverse(endpointBytes);
            }

            payload.Insert(0, lengthBytes[0]);
            payload.Insert(1, lengthBytes[1]);
            payload.Insert(2, endpointBytes[0]);
            payload.Insert(3, endpointBytes[1]);
        }

        protected virtual void GetContentFromMessage(List<byte> payload)
        {
        }
    }
}
