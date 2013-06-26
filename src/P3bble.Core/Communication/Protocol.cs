using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace P3bble.Core.Communication
{
    public class Protocol
    {
        private StreamSocket _socket;
        public Protocol(StreamSocket sock)
        {
            _socket = sock;
        }

        public P3bbleMessage ReadMessage()
        {
            byte[] payloadSize = new byte[2];
            byte[] endpoint = new byte[2];
            uint curPayloadSize = 0, endpo = 0;
            List<byte> lstBytes = new List<byte>();

            var buffer = new Windows.Storage.Streams.Buffer(4);
             _socket.InputStream.ReadAsync(buffer, 4, InputStreamOptions.None);
             if (buffer.Length == 4)
             {
                 using (var dr = DataReader.FromBuffer(buffer))
                 {
                     dr.ReadBytes(payloadSize);
                     dr.ReadBytes(endpoint);
                 }
                 if (BitConverter.IsLittleEndian)
                 {
                     Array.Reverse(payloadSize);
                     Array.Reverse(endpoint);
                 }
                 curPayloadSize = BitConverter.ToUInt16(payloadSize, 0);
                 endpo = BitConverter.ToUInt16(endpoint, 0);
                 var payloadContent = new Windows.Storage.Streams.Buffer(curPayloadSize);

                 do
                 {
                    _socket.InputStream.ReadAsync(payloadContent, curPayloadSize, InputStreamOptions.None);

                 } while (payloadContent.Length >= curPayloadSize);

                 byte[] payloadContentByte = new byte[curPayloadSize];

                 using (var dr = DataReader.FromBuffer(buffer))
                 {
                     dr.ReadBytes(payloadContentByte);
                 }

                 lstBytes = payloadContentByte.ToList();

                 endpo.ToString();
             }


             return P3bbleMessage.CreateMessage((P3bbleEndpoint)endpo, lstBytes);
        }

        public void WriteMessage(P3bbleMessage message)
        {
            IBuffer buf = GetBufferFromByteArray(message.ToBuffer());
            _socket.OutputStream.WriteAsync(buf);
        }

        private IBuffer GetBufferFromByteArray(byte[] package)
        {
            using (DataWriter dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }
    }
}
