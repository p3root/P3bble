using P3bble.Core.Communication;
using P3bble.Core.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;

#if WINDOWS_PHONE
using P3bble.Core.Constants;
using P3bble.Core.Firmware;
using System.Runtime.Serialization.Json;
using System.IO;
using P3bble.Core.Helper;
using P3bble.Core.EventArguments;
using System.Threading;
#endif

namespace P3bble.Core
{
    /// <summary>
    /// Defines a connection to a Pebble watch
    /// </summary>
    public class P3bble
    {
        // The underlying protocol handler...
        private Protocol _protocol;

        // Used to synchronise calls to the Pebble to make more natural for the API consumer...
        private ManualResetEventSlim _pendingMessageSignal;
        private P3bbleMessage _pendingMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="P3bble"/> class.
        /// </summary>
        /// <param name="peerInformation">The peer device to connect to.</param>
        internal P3bble(PeerInformation peerInformation)
        {
            PeerInformation = peerInformation;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the display name for the device
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName
        {
            get
            {
                return PeerInformation.DisplayName.Replace("Pebble", string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets the underlying Bluetooth peer information.
        /// </summary>
        /// <value>
        /// The peer information.
        /// </value>
        public PeerInformation PeerInformation { get; private set; }
        
        public P3bbleFirmwareVersion FirmwareVersion { get; private set; }
        
        public P3bbleFirmwareVersion RecoveryFirmwareVersion { get; private set; }

        /// <summary>
        /// Detects any paired pebbles.
        /// </summary>
        /// <returns>A list of pebbles if some are found</returns>
        public static Task<List<P3bble>> DetectPebbles()
        {
            return Task<List<P3bble>>.Factory.StartNew(() => FindPebbles());
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns>A boolean indicating if the connection was successful</returns>
        public async Task<bool> ConnectAsync()
        {
            // Check we're not already connected...
            if (this.IsConnected)
            {
                return true;
            }

            try
            {
                _protocol = await Protocol.CreateProtocolAsync(PeerInformation);
                _protocol.MessageReceived += ProtocolMessageReceived;
                this.IsConnected = true;

                // Now we're connected, request the Pebble version info...
                await this.SendMessageAndAwaitResponseAsync<VersionMessage>(new VersionMessage());
            }
            catch
            {
                this.IsConnected = false;
            }

            return this.IsConnected;
        }

        public Task PingAsync()
        {
            return _protocol.WriteMessage(new PingMessage());
        }

        // Possibly useful for log message reading??
        //public void BadPing()
        //{
        //    _protocol.WriteMessage(new PingMessage(new byte[7] { 1, 2, 3, 4, 5, 6, 7 }));
        //}

        public Task ResetAsync()
        {
            return _protocol.WriteMessage(new ResetMessage());
        }

        public Task SetNowPlayingAsync(string artist, string album, string track)
        {
            return _protocol.WriteMessage(new SetMusicMessage(artist, album, track));
        }

        public async Task<DateTime> GetTimeAsync()
        {
            TimeMessage result = await this.SendMessageAndAwaitResponseAsync<TimeMessage>(new TimeMessage());
            if (result != null)
            {
                return result.Time;
            }
            else
            {
                throw new TimedOutException();
            }
        }

        public event EventHandler<CheckForNewFirmwareVersionEventArgs> CheckForNewFirmwareCompleted;

        public void CheckForNewFirmwareAsync(bool useNightlyBuild = false)
        {
            string url = this.FirmwareVersion.GetFirmwareServerUrl(useNightlyBuild);


            WebClient wc = new WebClient();
            wc.DownloadStringAsync(new Uri(url));
            wc.DownloadStringCompleted += (sender, e) =>
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(e.Result);
                    MemoryStream stream = new MemoryStream(byteArray);
                    var serializer = new DataContractJsonSerializer(typeof(P3bbleFirmwareLatest));


                    P3bbleFirmwareLatest info = serializer.ReadObject(stream) as P3bbleFirmwareLatest;
                    stream.Close();
                    CheckForNewFirmwareVersionEventArgs eventArgs = new CheckForNewFirmwareVersionEventArgs(false, null);

                    if (FirmwareVersion.Version > info.Normal.FriendlyVersion.AsVersion())
                    {
                        eventArgs = new CheckForNewFirmwareVersionEventArgs(true, info);
                    }

                    if (CheckForNewFirmwareCompleted != null)
                        CheckForNewFirmwareCompleted(this, eventArgs);
                };
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Demo methods that aren't much use without lower level OS support...
        //////////////////////////////////////////////////////////////////////////////////

        public Task SmsNotificationAsync(string sender, string message)
        {
            return _protocol.WriteMessage(new NotificationMessage(NotificationType.SMS, sender, message));
        }

        public Task FacebookNotificationAsync(string sender, string message)
        {
            return _protocol.WriteMessage(new NotificationMessage(NotificationType.Facebook, sender, message));
        }

        public Task EmailNotificationAsync(string sender, string subject, string body)
        {
            return _protocol.WriteMessage(new NotificationMessage(NotificationType.EMAIL, sender, body, subject));
        }

        public Task PhoneCallAsync(string name, string number, byte[] cookie)
        {
            return _protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.INCOMING_CALL, cookie, number, name));
        }

        public Task RingAsync(byte[] cookie)
        {
            return _protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.RING, cookie));
        }

        public Task StartCallAsync(byte[] cookie)
        {
            return _protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.START, cookie));
        }

        public Task EndCallAsync(byte[] cookie)
        {
            return _protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.END, cookie));
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Private methods below - e.g. handling discovery or incoming messages
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Finds paired pebbles.
        /// </summary>
        /// <returns>A list of pebbles</returns>
        private static List<P3bble> FindPebbles()
        {
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";

#if NETFX_CORE
            PeerFinder.Start();
#endif

            IReadOnlyList<PeerInformation> pairedDevices = PeerFinder.FindAllPeersAsync().AsTask().Result;
            List<P3bble> lst = new List<P3bble>();

            // Filter to only devices that are named Pebble - right now, that's the only way to
            // stop us getting headphones, etc. showing up...
            foreach (PeerInformation pi in pairedDevices)
            {
                if (pi.DisplayName.StartsWith("Pebble", StringComparison.InvariantCultureIgnoreCase))
                {
                    lst.Add(new P3bble(pi));
                }
            }

            if (pairedDevices.Count == 0)
            {
                Debug.WriteLine("No paired devices were found.");
            }

            return lst;
        }

        /// <summary>
        /// Handles protocol messages
        /// </summary>
        /// <param name="message">The message.</param>
        private void ProtocolMessageReceived(P3bbleMessage message)
        {
            Debug.WriteLine("ProtocolMessageReceived: " + message.Endpoint.ToString());

            switch (message.Endpoint)
            {
                case P3bbleEndpoint.PhoneVersion:
                    // We need to tell the Pebble what we are...
                    _protocol.WriteMessage(new PhoneVersionMessage());
                    break;

                case P3bbleEndpoint.Version:
                    // Store version info we got from the Pebble...
                    VersionMessage version = message as VersionMessage;
                    FirmwareVersion = version.Firmware;
                    RecoveryFirmwareVersion = version.RecoveryFirmware;
                    break;

                default:
                    break;
            }

            // Check if we're waiting for a message...
            if (this._pendingMessageSignal != null && this._pendingMessage != null)
            {
                if (this._pendingMessage.Endpoint == message.Endpoint)
                {
                    Debug.WriteLine("ProtocolMessageReceived: we were waiting for this type of message");
                    this._pendingMessage = message;
                    this._pendingMessageSignal.Set();
                }
            }
        }

        /// <summary>
        /// Sends a message to the Pebble and awaits the response.
        /// </summary>
        /// <typeparam name="T">The type of message</typeparam>
        /// <param name="message">The message content.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <returns>A message response</returns>
        /// <exception cref="System.InvalidOperationException">A message is being waited for already</exception>
        /// <remarks>Beware when debugging that setting a breakpoint in Protocol.Run or ProtocolMessageReceived will cause the ResetEvent to time out</remarks>
        private Task<T> SendMessageAndAwaitResponseAsync<T>(P3bbleMessage message, int millisecondsTimeout = 5000)
            where T : P3bbleMessage
        {
            if (this._pendingMessageSignal != null)
            {
                throw new InvalidOperationException("A message is being waited for already");
            }

            return Task.Run<T>(async () =>
                {
                    int startTicks = Environment.TickCount;
                    this._pendingMessageSignal = new ManualResetEventSlim(false);
                    this._pendingMessage = message;
                    
                    // Send the message...
                    await _protocol.WriteMessage(message);
                    
                    // Wait for the response...
                    this._pendingMessageSignal.Wait(millisecondsTimeout);

                    // Store any response or (null if timed out)...
                    T pendingMessage = this._pendingMessage as T;

                    // Clear the pending variables...
                    this._pendingMessageSignal = null;
                    this._pendingMessage = null;

                    Debug.WriteLine(pendingMessage.GetType().Name + " message received back in " + (Environment.TickCount - startTicks).ToString() + "ms");
                    
                    return pendingMessage;
                });
        }
    }
}
