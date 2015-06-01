using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using P3bble.Constants;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using P3bble.PCL;

namespace P3bble.Communication
{
    /// <summary>
    /// Encapsulates comms with the Pebble
    /// </summary>
    internal class Protocol : IDisposable
    {
        private readonly Mutex _mutex = new Mutex();
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;
        private object _lock;
        private bool _isRunning;

        private Protocol(StreamSocket socket)
        {
            this._socket = socket;
            this._writer = new DataWriter(this._socket.OutputStream);
            this._reader = new DataReader(this._socket.InputStream);
            this._reader.InputStreamOptions = InputStreamOptions.Partial;

            this._lock = new object();
#if WINDOWS_PHONE
            this._isRunning = true;
            System.Threading.ThreadPool.QueueUserWorkItem(this.Run);
#else
            this._isRunning = true;
            this.Run(null);
#endif
        }

        public delegate void MessageReceivedHandler(P3bbleMessage message);

        public MessageReceivedHandler MessageReceived { get; set; }

        /// <summary>
        /// Creates the protocol - encapsulates the socket creation
        /// </summary>
        /// <param name="peer">The peer.</param>
        /// <returns>A protocol object</returns>
        public static async Task<Protocol> CreateProtocolAsync(PeerInformation peer)
        {
#if WINDOWS_PHONE || WINDOWS_PHONE_APP
            // {00001101-0000-1000-8000-00805f9b34fb} specifies we want a Serial Port - see http://developer.nokia.com/Community/Wiki/Bluetooth_Services_for_Windows_Phone
            // {00000000-deca-fade-deca-deafdecacaff} Fix ServiceID for WP8.1 Update 2
            StreamSocket socket = new StreamSocket();
            await socket.ConnectAsync(peer.HostName, Guid.Parse("00000000-deca-fade-deca-deafdecacaff").ToString("B"));
            return new Protocol(socket);
#endif

            throw new NotImplementedException();
        }

#if NETFX_CORE  && !WINDOWS_PHONE_APP
        public static async Task<Protocol> CreateProtocolAsync(Windows.Devices.Bluetooth.Rfcomm.RfcommDeviceService peer)
        {
            StreamSocket socket = new StreamSocket();
            await socket.ConnectAsync(peer.ConnectionHostName, new Guid(0x00001101, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB).ToString("B"), SocketProtectionLevel.PlainSocket);
            return new Protocol(socket);
        }
#endif

        /// <summary>
        /// Sends a message to the Pebble.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>An async task to wait</returns>
        public Task WriteMessage(P3bbleMessage message)
        {
            return Task.Factory.StartNew(() =>
            {
                this._mutex.WaitOne();

                byte[] package = message.ToBuffer();
                ServiceLocator.Logger.WriteLine("<< SEND MESSAGE FOR ENDPOINT " + ((Endpoint)message.Endpoint).ToString() + " (" + ((int)message.Endpoint).ToString() + ")");
                ServiceLocator.Logger.WriteLine("<< PAYLOAD: " + BitConverter.ToString(package));

                this._writer.WriteBytes(package);
                this._writer.StoreAsync().AsTask().Wait();

                this._mutex.ReleaseMutex();
            });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this._writer != null)
            {
                this._writer.Dispose();
                this._writer = null;
            }

            if (this._reader != null)
            {
                this._reader.Dispose();
                this._reader = null;
            }

            if (this._socket != null)
            {
                this._socket.Dispose();
                this._socket = null;
            }

            if (this._mutex != null)
            {
                this._mutex.Dispose();
            }
        }

#if NETFX_CORE  && !WINDOWS_PHONE_APP
        private void Run(object host)
        {
            Task.Factory.StartNew(
                async () =>
            {
#else
        private async void Run(object host)
        {
#endif
            var readMutex = new AsyncLock();

            while (this._isRunning)
            {
                try
                {
                    await this._reader.LoadAsync(4);

                    ServiceLocator.Logger.WriteLine("[message available]");
                    using (await readMutex.LockAsync())
                    {
                        ServiceLocator.Logger.WriteLine("[message unlocked]");
                        uint payloadLength;
                        uint endpoint;

                        if (this._reader.UnconsumedBufferLength > 0)
                        {
                            IBuffer buffer = this._reader.ReadBuffer(4);

                            this.GetLengthAndEndpoint(buffer, out payloadLength, out endpoint);
                            ServiceLocator.Logger.WriteLine(">> RECEIVED MESSAGE FOR ENDPOINT: " + ((Endpoint)endpoint).ToString() + " (" + endpoint.ToString() + ") - " + payloadLength.ToString() + " bytes");
                            if (endpoint > 0 && payloadLength > 0)
                            {
                                byte[] payload = new byte[payloadLength];
                                await this._reader.LoadAsync(payloadLength);
                                this._reader.ReadBytes(payload);

                                P3bbleMessage msg = this.ReadMessage(payload, endpoint);

                                if (msg != null && this.MessageReceived != null)
                                {
                                    this.MessageReceived(msg);
                                }
                            }
                            else
                            {
                                ServiceLocator.Logger.WriteLine(">> RECEIVED MESSAGE WITH BAD ENDPOINT OR LENGTH: " + endpoint.ToString() + ", " + payloadLength.ToString());
                            }
                        }
                    }
                }
                catch
                {
                }
#if NETFX_CORE  && !WINDOWS_PHONE_APP
                    await Task.Delay(100);
#endif
            }
#if NETFX_CORE  && !WINDOWS_PHONE_APP
            },
            TaskCreationOptions.LongRunning);
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

        private P3bbleMessage ReadMessage(byte[] payloadContent, uint endpoint)
        {
            List<byte> lstBytes = payloadContent.ToList();
            byte[] array = lstBytes.ToArray();
            ServiceLocator.Logger.WriteLine(">> PAYLOAD: " + BitConverter.ToString(array));
            return P3bbleMessage.CreateMessage((Endpoint)endpoint, lstBytes);
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
