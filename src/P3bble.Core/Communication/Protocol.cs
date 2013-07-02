using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

using System.Runtime.InteropServices.WindowsRuntime;
using P3bble.Core.Constants;

namespace P3bble.Core.Communication
{
    internal class P3bbleMessageReceivedEventArgs : EventArgs
    {
        public P3bbleMessageReceivedEventArgs(P3bbleMessage message)
            :base()
        {
            Message = message;
        }

        public P3bbleMessage Message { get; set; }
    }

    internal class Protocol
    {
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;
        private object _lock;
        private bool _isRunning;
        private readonly Mutex _mutex = new Mutex();

        public EventHandler<P3bbleMessageReceivedEventArgs> MessageReceived;

        public Protocol(StreamSocket sock)
        {
            _socket = sock;
            _writer = new DataWriter(sock.OutputStream);
            _reader = new DataReader(sock.InputStream);
            _reader.InputStreamOptions = InputStreamOptions.Partial;

            _lock = new object();
#if WINDOWS_PHONE
           // Thread t = new Thread(new ThreadStart(Run));
            _isRunning = true;
          //  t.Start();

            System.Threading.ThreadPool.QueueUserWorkItem(Run);
#endif
        }

        private async void Run(object host)
        {
#if NETFX_CORE
            Task.Factory.StartNew(() =>
            {
#endif
                while (_isRunning)
                {
                        try
                        {
                            await _reader.LoadAsync(4);
                            uint payloadLength;
                            uint endpoint;
                
                            if (_reader.UnconsumedBufferLength > 0)
                            {
                                IBuffer buffer = _reader.ReadBuffer(4);

                                GetLengthAndEndpoint(buffer, out payloadLength, out endpoint);
#if DEBUG
                                Debug.WriteLine("ENDPOINT: " + endpoint);
                                Debug.WriteLine("PAYLOADS: " + payloadLength);
#endif
                                await _reader.LoadAsync(payloadLength);
                                IBuffer buf = _reader.ReadBuffer(payloadLength);

                                P3bbleMessage msg = await ReadMessage(buf, endpoint);

                                if (msg != null)
                                {
                                    if (MessageReceived != null)
                                        MessageReceived(this, new P3bbleMessageReceivedEventArgs(msg));
                                }
                            }
                        }
                        catch
                        {

                        }
#if NETFX_CORE
                    Task.Delay(100);
#endif
                }
#if NETFX_CORE
            }, TaskCreationOptions.LongRunning);
#endif
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
        }

        private Task<P3bbleMessage> ReadMessage(IBuffer buffer, uint endpoint)
        {
            List<byte> lstBytes = new List<byte>();

            byte[] payloadContentByte = new byte[buffer.Length];

            using (var dr = DataReader.FromBuffer(buffer))
            {
                dr.ReadBytes(payloadContentByte);
            }

            lstBytes = payloadContentByte.ToList();
#if DEBUG
            byte[] array = lstBytes.ToArray();
            Debug.WriteLine("PAYLOAD: " + BitConverter.ToString(array));
#endif
            return Task.FromResult<P3bbleMessage>(P3bbleMessage.CreateMessage((P3bbleEndpoint)endpoint, lstBytes));
        }

        public void WriteMessage(P3bbleMessage message)
        {
            _mutex.WaitOne();

            byte[] package = message.ToBuffer();
            Debug.WriteLine("Write Package: " + BitConverter.ToString(package));

            _writer.WriteBytes(package);
            _writer.StoreAsync().AsTask().Wait();

            _mutex.ReleaseMutex();
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
