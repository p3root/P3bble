using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using P3bble.Core.Communication;
using P3bble.Core.Constants;
using P3bble.Core.Helper;
using P3bble.Core.Messages;
using P3bble.Core.Types;
using Windows.Networking.Proximity;

namespace P3bble.Core
{
    /// <summary>
    /// Delegate to handle music control events
    /// </summary>
    /// <param name="action">The control action.</param>
    public delegate void MusicControlReceivedHandler(MusicControlAction action);

    /// <summary>
    /// Defines a connection to a Pebble watch
    /// </summary>
    public class P3bble : IP3bble
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
        /// Gets or sets the music control received handler.
        /// </summary>
        /// <value>
        /// The music control received handler.
        /// </value>
        public MusicControlReceivedHandler MusicControlReceived { get; set; }

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
        /// Gets the firmware version.
        /// </summary>
        /// <value>
        /// The firmware version.
        /// </value>
        public P3bbleFirmwareVersion FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the recovery firmware version.
        /// </summary>
        /// <value>
        /// The recovery firmware version.
        /// </value>
        public P3bbleFirmwareVersion RecoveryFirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the underlying Bluetooth peer information.
        /// </summary>
        /// <value>
        /// The peer information.
        /// </value>
        internal PeerInformation PeerInformation { get; private set; }

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
        /// <returns>A bool indicating if the connection was successful</returns>
        public async Task<bool> ConnectAsync()
        {
            // Check we're not already connected...
            if (this.IsConnected)
            {
                return true;
            }

            try
            {
                this._protocol = await Protocol.CreateProtocolAsync(PeerInformation);
                this._protocol.MessageReceived += this.ProtocolMessageReceived;
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

        /// <summary>
        /// Pings the device.
        /// </summary>
        /// <returns>An async task to wait if required</returns>
        public Task PingAsync()
        {
            return this._protocol.WriteMessage(new PingMessage());
        }

        /// <summary>
        /// Resets the watch.
        /// </summary>
        /// <returns>An async task to wait if required</returns>
        public Task ResetAsync()
        {
            return this._protocol.WriteMessage(new ResetMessage());
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <returns>An async task to wait that will return the current time</returns>
        public async Task<DateTime> GetTimeAsync()
        {
            var result = await this.SendMessageAndAwaitResponseAsync<TimeMessage>(new TimeMessage());
            if (result != null)
            {
                return result.Time;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// Sets the time on the Pebble.
        /// </summary>
        /// <param name="newTime">The new time.</param>
        /// <returns>An async task to wait</returns>
        public Task SetTimeAsync(DateTime newTime)
        {
            return this._protocol.WriteMessage(new TimeMessage(newTime));
        }

        /// <summary>
        /// Gets the latest firmware version.
        /// </summary>
        /// <returns>An async task to wait that will result in firmware info</returns>
        public async Task<P3bbleFirmwareResponse> GetLatestFirmwareVersionAsync()
        {
            string url = this.FirmwareVersion.GetFirmwareServerUrl(false);

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(P3bbleFirmwareResponse));
                P3bbleFirmwareResponse info = serializer.ReadObject(stream) as P3bbleFirmwareResponse;
                stream.Close();
                return info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a list of the installed apps.
        /// </summary>
        /// <returns>
        /// An async task to wait that will result in a list of apps
        /// </returns>
        public async Task<P3bbleInstalledApplications> GetInstalledAppsAsync()
        {
            var result = await this.SendMessageAndAwaitResponseAsync<AppMessage>(new AppMessage(AppMessageAction.ListApps));
            if (result != null)
            {
                return result.InstalledApplications;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// Remove an installed application from the specified app-bank.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        public Task RemoveAppAsync(P3bbleInstalledApplication app)
        {
            return this._protocol.WriteMessage(new AppMessage(AppMessageAction.RemoveApp, app.Id, app.Index));
        }

        public Task SetNowPlayingAsync(string artist, string album, string track)
        {
            return this._protocol.WriteMessage(new MusicMessage(artist, album, track));
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Demo methods that aren't much use without lower level OS support...
        //////////////////////////////////////////////////////////////////////////////////

        // Possibly useful for log message reading??
        ////public void BadPing()
        ////{
        ////    _protocol.WriteMessage(new PingMessage(new byte[7] { 1, 2, 3, 4, 5, 6, 7 }));
        ////}

        public Task SmsNotificationAsync(string sender, string message)
        {
            return this._protocol.WriteMessage(new NotificationMessage(NotificationType.SMS, sender, message));
        }

        public Task FacebookNotificationAsync(string sender, string message)
        {
            return this._protocol.WriteMessage(new NotificationMessage(NotificationType.Facebook, sender, message));
        }

        public Task EmailNotificationAsync(string sender, string subject, string body)
        {
            return this._protocol.WriteMessage(new NotificationMessage(NotificationType.Email, sender, body, subject));
        }

        public Task PhoneCallAsync(string name, string number, byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.IncomingCall, cookie, number, name));
        }

        public Task RingAsync(byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.Ring, cookie));
        }

        public Task StartCallAsync(byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.Start, cookie));
        }

        public Task EndCallAsync(byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.End, cookie));
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
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = string.Empty;

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
                    this._protocol.WriteMessage(new PhoneVersionMessage());
                    break;

                case P3bbleEndpoint.Version:
                    // Store version info we got from the Pebble...
                    VersionMessage version = message as VersionMessage;
                    this.FirmwareVersion = version.Firmware;
                    this.RecoveryFirmwareVersion = version.RecoveryFirmware;
                    break;

                case P3bbleEndpoint.Logs:
                    if (message as LogsMessage != null)
                    {
                        Debug.WriteLine(">> LOG: " + (message as LogsMessage).Message);
                    }

                    break;

                case P3bbleEndpoint.MusicControl:
                    var musicMessage = message as MusicMessage;
                    if (this.MusicControlReceived != null && musicMessage != null && musicMessage.ControlAction != MusicControlAction.Unknown)
                    {
                        this.MusicControlReceived(musicMessage.ControlAction);
                    }

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
        private Task<T> SendMessageAndAwaitResponseAsync<T>(P3bbleMessage message, int millisecondsTimeout = 10000)
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

                    T pendingMessage = null;

                    if (this._pendingMessageSignal.IsSet)
                    {
                        // Store any response will benull if timed out...
                        pendingMessage = this._pendingMessage as T;
                    }

                    // Clear the pending variables...
                    this._pendingMessageSignal = null;
                    this._pendingMessage = null;

                    int timeTaken = Environment.TickCount - startTicks;

                    if (pendingMessage != null)
                    {
                        Debug.WriteLine(pendingMessage.GetType().Name + " message received back in " + timeTaken.ToString() + "ms");
                    }
                    else
                    {
                        Debug.WriteLine(message.GetType().Name + " message timed out in " + timeTaken.ToString() + "ms");
                    }

                    return pendingMessage;
                });
        }
    }
}
