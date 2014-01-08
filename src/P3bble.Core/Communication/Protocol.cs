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
    internal class Protocol
    {
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;
        private object _lock;
        private bool _isRunning;
        private readonly Mutex _mutex = new Mutex();

        public delegate void MessageReceivedHandler(P3bbleMessage message);
        public MessageReceivedHandler MessageReceived;

        /// <summary>
        /// Creates the protocol - encapsulates the socket creation
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <returns>A protocol object</returns>
        public static async Task<Protocol> CreateProtocolAsync(PeerInformation peer)
        {
            StreamSocket socket = new StreamSocket();
#if WINDOWS_PHONE
            // {00001101-0000-1000-8000-00805f9b34fb} specifies we want a Serial Port - see http://developer.nokia.com/Community/Wiki/Bluetooth_Services_for_Windows_Phone
            await socket.ConnectAsync(peer.HostName, new Guid(0x00001101, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB).ToString("B"));
#elif NETFX_CORE
            await socket = Windows.Networking.Proximity.PeerFinder.ConnectAsync(peer);
#endif
            return new Protocol(socket);
        }

        private Protocol(StreamSocket socket)
        {
            _socket = socket;
            _writer = new DataWriter(_socket.OutputStream);
            _reader = new DataReader(_socket.InputStream);
            _reader.InputStreamOptions = InputStreamOptions.Partial;

            _lock = new object();
#if WINDOWS_PHONE
           // Thread t = new Thread(new ThreadStart(Run));
            _isRunning = true;
          //  t.Start();
            System.Threading.ThreadPool.QueueUserWorkItem(Run);
#endif
        }

        /// <summary>
        /// Sends a message to the Pebble.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void WriteMessage(P3bbleMessage message)
        {
            _mutex.WaitOne();

            byte[] package = message.ToBuffer();
            Debug.WriteLine("<< SEND MESSAGE FOR ENDPOINT " + ((int)message.Endpoint).ToString());
            Debug.WriteLine("<< PAYLOAD: " + BitConverter.ToString(package));

            _writer.WriteBytes(package);
            _writer.StoreAsync().AsTask().Wait();

            _mutex.ReleaseMutex();
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
                                Debug.WriteLine(">> RECEIVED MESSAGE FOR ENDPOINT: " + endpoint + " - " + payloadLength.ToString() + " bytes");
#endif
                                await _reader.LoadAsync(payloadLength);
                                IBuffer buf = _reader.ReadBuffer(payloadLength);

                                P3bbleMessage msg = await ReadMessage(buf, endpoint);

                                if (msg != null && this.MessageReceived != null)
                                {
                                    this.MessageReceived(msg);
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
            Debug.WriteLine(">> PAYLOAD: " + BitConverter.ToString(array));
#endif
            return Task.FromResult<P3bbleMessage>(P3bbleMessage.CreateMessage((P3bbleEndpoint)endpoint, lstBytes));
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
