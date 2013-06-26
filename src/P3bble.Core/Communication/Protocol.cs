using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace P3bble.Core.Communication
{
    public class P3bbleMessageReceivedEventArgs : EventArgs
    {
        public P3bbleMessageReceivedEventArgs(P3bbleMessage message)
            :base()
        {
            Message = message;
        }

        public P3bbleMessage Message { get; set; }
    }

    public class Protocol
    {
        private StreamSocket _socket;
        private object _lock;
        private bool _isRunning;

        public EventHandler<P3bbleMessageReceivedEventArgs> MessageReceived;

        public Protocol(StreamSocket sock)
        {
            _socket = sock;
            Thread t = new Thread(new ThreadStart(Run));
            _isRunning = true;
            _lock = new object();
            t.Start();
        }

        private void Run()
        {
            while (_isRunning)
            {
                lock (_lock)
                {
                    var buffer = new Windows.Storage.Streams.Buffer(4);
                    _socket.InputStream.ReadAsync(buffer, 4, InputStreamOptions.None).AsTask().Wait(100);

                    if (buffer.Length > 0)
                    {
                        uint payloadLength, endpoint;

                        GetLengthAndEndpoint(buffer, out payloadLength, out endpoint);

                        if (endpoint == 0)
                            continue;

                        P3bbleMessage msg = ReadMessage(payloadLength, endpoint);

                        if (msg != null)
                        {
                            if(MessageReceived != null)
                                MessageReceived(this, new P3bbleMessageReceivedEventArgs(msg));
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }

        private void GetLengthAndEndpoint(IBuffer buffer, out uint payloadLength, out uint endpoint)
        {
            if (buffer.Length != 4)
            {
                payloadLength = 0;
                endpoint = 0;
                return;
            }
            byte[] payloadSize = new byte[2];
            byte[] endpo = new byte[2];

            using (var dr = DataReader.FromBuffer(buffer))
            {
                dr.ReadBytes(payloadSize);
                dr.ReadBytes(endpo);
            }
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(payloadSize);
                Array.Reverse(endpo);
            }
            payloadLength = BitConverter.ToUInt16(payloadSize, 0);
            endpoint = BitConverter.ToUInt16(endpo, 0);

            if (!Enum.IsDefined(typeof(P3bbleEndpoint), Convert.ToInt32(endpoint)))
            {
                endpoint = 0;
                payloadLength = 0;
            }
        }

        private P3bbleMessage ReadMessage(uint payloadSie, uint endpoint)
        {
            var payloadContent = new Windows.Storage.Streams.Buffer(payloadSie);
            List<byte> lstBytes = new List<byte>();

            do
            {
                _socket.InputStream.ReadAsync(payloadContent, payloadSie, InputStreamOptions.None).AsTask().Wait();

            } while (payloadContent.Length > payloadSie);

            byte[] payloadContentByte = new byte[payloadSie];

            using (var dr = DataReader.FromBuffer(payloadContent))
            {
                dr.ReadBytes(payloadContentByte);
            }

            lstBytes = payloadContentByte.ToList();

            return P3bbleMessage.CreateMessage((P3bbleEndpoint)endpoint, lstBytes);
        }

        public P3bbleMessage ReadMessage()
        {
            lock (_lock)
            {
                var buffer = new Windows.Storage.Streams.Buffer(4);
                _socket.InputStream.ReadAsync(buffer, 4, InputStreamOptions.None).AsTask().Wait(100);
                if (buffer.Length == 4)
                {
                    uint payloadLength, endpoint;

                    GetLengthAndEndpoint(buffer, out payloadLength, out endpoint);

                    if (endpoint == 0)
                        return null;

                    return ReadMessage(payloadLength, endpoint);
                }
                return null;
            }
        }


        public void WriteMessage(P3bbleMessage message)
        {
            lock (_lock)
            {
                IBuffer buf = GetBufferFromByteArray(message.ToBuffer());
                _socket.OutputStream.WriteAsync(buf).AsTask().Wait();
            }
        }

        private IBuffer GetBufferFromByteArray(byte[] package)
        {
            Debug.WriteLine("Write Package: " + BitConverter.ToString(package));
            using (DataWriter dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }
    }
}
