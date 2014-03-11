using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using P3bble.Communication;
using P3bble.Constants;
using P3bble.Messages;
using P3bble.Types;
using Windows.Networking.Proximity;
using Windows.Storage;

namespace P3bble
{
    /// <summary>
    /// Delegate to handle music control events
    /// </summary>
    /// <param name="action">The control action.</param>
    public delegate void MusicControlReceivedHandler(MusicControlAction action);

    /// <summary>
    /// Delegate to handle installation progress
    /// </summary>
    /// <param name="percentComplete">The percent complete.</param>
    public delegate void InstallProgressHandler(int percentComplete);

    /// <summary>
    /// Defines a connection to a Pebble watch
    /// </summary>
    public class Pebble : IPebble
    {
        // The underlying protocol handler...
        private Protocol _protocol;

        // Used to synchronise calls to the Pebble to make more natural for the API consumer...
        private ManualResetEventSlim _pendingMessageSignal;
        private P3bbleMessage _pendingMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pebble"/> class.
        /// </summary>
        /// <param name="peerInformation">The peer device to connect to.</param>
        internal Pebble(PeerInformation peerInformation)
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
        /// Gets or sets the install progress handler.
        /// </summary>
        /// <value>
        /// The install progress handler.
        /// </value>
        public InstallProgressHandler InstallProgress { get; set; }

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
                return PeerInformation.DisplayName.Trim();
            }
        }

        /// <summary>
        /// Gets the firmware version.
        /// </summary>
        /// <value>
        /// The firmware version.
        /// </value>
        public FirmwareVersion FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets the recovery firmware version.
        /// </summary>
        /// <value>
        /// The recovery firmware version.
        /// </value>
        public FirmwareVersion RecoveryFirmwareVersion { get; private set; }

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
        public static Task<List<Pebble>> DetectPebbles()
        {
            return Task<List<Pebble>>.Factory.StartNew(() => FindPebbles());
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
        public async Task<FirmwareResponse> GetLatestFirmwareVersionAsync()
        {
            string url = this.FirmwareVersion.GetFirmwareServerUrl(false);

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var serializer = new DataContractJsonSerializer(typeof(FirmwareResponse));
                    FirmwareResponse info = serializer.ReadObject(stream) as FirmwareResponse;
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of the installed apps.
        /// </summary>
        /// <returns>
        /// An async task to wait that will result in a list of apps
        /// </returns>
        public async Task<InstalledApplications> GetInstalledAppsAsync()
        {
            Debug.WriteLine("GetInstalledAppsAsync");
            var result = await this.SendMessageAndAwaitResponseAsync<AppManagerMessage>(new AppManagerMessage(AppManagerAction.ListApps));
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
        public async Task<bool> RemoveAppAsync(InstalledApplication app)
        {
            Debug.WriteLine("RemoveAppAsync");
            var result = await this.SendMessageAndAwaitResponseAsync<AppManagerMessage>(new AppManagerMessage(AppManagerAction.RemoveApp, app.Id, app.Index));
            if (result != null)
            {
                return result.Result == AppManagerResult.AppRemoved;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// Sets the now playing track.
        /// </summary>
        /// <param name="artist">The artist.</param>
        /// <param name="album">The album.</param>
        /// <param name="track">The track.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        public Task SetNowPlayingAsync(string artist, string album, string track)
        {
            return this._protocol.WriteMessage(new MusicMessage(artist, album, track));
        }

        /// <summary>
        /// Downloads an application or firmware bundle
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        public async Task<Bundle> DownloadBundleAsync(string uri)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var downloadStream = await response.Content.ReadAsStreamAsync();

                Guid fileGuid = Guid.NewGuid();

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileGuid.ToString());

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    byte[] buffer = new byte[1024];
                    while (downloadStream.Read(buffer, 0, buffer.Length) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                }

                var bundle = new Bundle(fileGuid.ToString());
                await bundle.Initialise();
                return bundle;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Installs an application.
        /// </summary>
        /// <param name="bundle">The application.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        public async Task InstallApp(Bundle bundle)
        {
            if (bundle.BundleType != BundleType.Application)
            {
                throw new ArgumentException("Only app bundles can be installed");
            }

            // Get list of installed apps...
            var installedApps = await this.GetInstalledAppsAsync();

            // Find the first free slot...
            uint firstFreeBank = 1;
            foreach (var installedApp in installedApps.ApplicationsInstalled)
            {
                if (installedApp.Index == firstFreeBank)
                {
                    firstFreeBank++;
                }

                if (firstFreeBank == installedApps.ApplicationBanks)
                {
                    throw new CannotInstallException("There are no memory slots free");
                }
            }

            double progress = 0;
            double totalBytes = bundle.Manifest.ApplicationManifest.Size + bundle.Manifest.Resources.Size;

            InstallProgressHandler handler = null;

            if (this.InstallProgress != null)
            {
                // Derive overall progress from the bytes sent for the part...
                handler = new InstallProgressHandler((partProgress) =>
                {
                    progress += partProgress;
                    int percentComplete = (int)(progress / totalBytes * 100);
                    Debug.WriteLine("Installation " + percentComplete.ToString() + "% complete - " + progress.ToString() + " / " + totalBytes.ToString());
                    this.InstallProgress(percentComplete);
                });
            }

            Debug.WriteLine(string.Format("Attempting to add app to bank {0} of {1}", firstFreeBank, installedApps.ApplicationBanks));

            PutBytesMessage binMsg = new PutBytesMessage(PutBytesTransferType.Binary, bundle.BinaryContent, handler, firstFreeBank);

            try
            {
                var binResult = await this.SendMessageAndAwaitResponseAsync<PutBytesMessage>(binMsg, 60000);

                if (binResult == null || binResult.Errored)
                {
                    throw new CannotInstallException(string.Format("Failed to send binary {0}", bundle.Manifest.ApplicationManifest.Filename));
                }
            }
            catch (ProtocolException pex)
            {
                throw new CannotInstallException("Sorry, an internal error occurred: " + pex.Message);
            }

            if (bundle.HasResources)
            {
                PutBytesMessage resourcesMsg = new PutBytesMessage(PutBytesTransferType.Resources, bundle.Resources, handler, firstFreeBank);

                try
                {
                    var resourceResult = await this.SendMessageAndAwaitResponseAsync<PutBytesMessage>(resourcesMsg, 240000);

                    if (resourceResult == null || resourceResult.Errored)
                    {
                        throw new CannotInstallException(string.Format("Failed to send resources {0}", bundle.Manifest.Resources.Filename));
                    }
                }
                catch (ProtocolException pex)
                {
                    throw new CannotInstallException("Sorry, an internal error occurred: " + pex.Message);
                }
            }

            var appMsg = new AppMessage(Endpoint.AppManager) { Command = AppCommand.FinaliseInstall, AppIndex = firstFreeBank };
            await this.SendMessageAndAwaitResponseAsync<AppManagerMessage>(appMsg);

            await Bundle.DeleteFromStorage(bundle);

            if (this.InstallProgress != null)
            {
                this.InstallProgress(100);
            }

            // Now launch the new app
            await this.LaunchApp(bundle.Application.Uuid);
        }

        /// <summary>
        /// Launches an application.
        /// </summary>
        /// <param name="appUuid">The application UUID.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        public async Task<bool> LaunchApp(Guid appUuid)
        {
            var msg = new AppMessage(Endpoint.Launcher)
            {
                AppUuid = appUuid,
                Command = AppCommand.Push
            };

            msg.AddTuple((uint)LauncherKeys.RunState, AppMessageTupleDataType.UInt, (byte)LauncherParams.Running);

            var result = await this.SendMessageAndAwaitResponseAsync<AppMessage>(msg);

            if (result != null)
            {
                return result.Command == AppCommand.Ack;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// Installs a firmware bundle.
        /// </summary>
        /// <param name="bundle">The firmware.</param>
        /// <param name="recovery">Whether to install recovery firmware.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        public async Task InstallFirmware(Bundle bundle, bool recovery)
        {
            if (bundle.BundleType != BundleType.Firmware)
            {
                throw new ArgumentException("Only firmware bundles can be installed");
            }

            double progress = 0;
            double totalBytes = bundle.Manifest.Firmware.Size + bundle.Manifest.Resources.Size;

            InstallProgressHandler handler = null;

            if (this.InstallProgress != null)
            {
                // Derive overall progress from the bytes sent for the part...
                handler = new InstallProgressHandler((partProgress) =>
                {
                    progress += partProgress;
                    int percentComplete = (int)(progress / totalBytes * 100);
                    Debug.WriteLine("Installation " + percentComplete.ToString() + "% complete - " + progress.ToString() + " / " + totalBytes.ToString());
                    this.InstallProgress(percentComplete);
                });
            }

            await this._protocol.WriteMessage(new SystemMessage(SystemCommand.FirmwareStart));

            if (bundle.HasResources)
            {
                PutBytesMessage resourcesMsg = new PutBytesMessage(PutBytesTransferType.SystemResources, bundle.Resources, handler);

                try
                {
                    var resourceResult = await this.SendMessageAndAwaitResponseAsync<PutBytesMessage>(resourcesMsg, 720000);

                    if (resourceResult == null || resourceResult.Errored)
                    {
                        throw new CannotInstallException(string.Format("Failed to send resources {0}", bundle.Manifest.Resources.Filename));
                    }
                }
                catch (ProtocolException pex)
                {
                    throw new CannotInstallException("Sorry, an internal error occurred: " + pex.Message);
                }
            }

            PutBytesMessage binMsg = new PutBytesMessage(recovery ? PutBytesTransferType.Recovery : PutBytesTransferType.Firmware, bundle.BinaryContent, handler);

            try
            {
                var binResult = await this.SendMessageAndAwaitResponseAsync<PutBytesMessage>(binMsg, 720000);

                if (binResult == null || binResult.Errored)
                {
                    throw new CannotInstallException(string.Format("Failed to send binary {0}", bundle.Manifest.Firmware.Filename));
                }
            }
            catch (ProtocolException pex)
            {
                throw new CannotInstallException("Sorry, an internal error occurred: " + pex.Message);
            }

            await this._protocol.WriteMessage(new SystemMessage(SystemCommand.FirmwareComplete));

            await Bundle.DeleteFromStorage(bundle);

            if (this.InstallProgress != null)
            {
                this.InstallProgress(100);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Demo methods that aren't much use without lower level OS support...
        //////////////////////////////////////////////////////////////////////////////////

        // Possibly useful for log message reading??
        ////public void BadPing()
        ////{
        ////    _protocol.WriteMessage(new PingMessage(new byte[7] { 1, 2, 3, 4, 5, 6, 7 }));
        ////}

        /// <summary>
        /// Sends an SMS notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        public Task SmsNotificationAsync(string sender, string message)
        {
            return this._protocol.WriteMessage(new NotificationMessage(NotificationType.SMS, sender, message));
        }

        /// <summary>
        /// Sends a Facebook notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        public Task FacebookNotificationAsync(string sender, string message)
        {
            return this._protocol.WriteMessage(new NotificationMessage(NotificationType.Facebook, sender, message));
        }

        /// <summary>
        /// Sends an Email notification.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        public Task EmailNotificationAsync(string sender, string subject, string body)
        {
            return this._protocol.WriteMessage(new NotificationMessage(NotificationType.Email, sender, body, subject));
        }

        /// <summary>
        /// Starts a Phone call notification.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="number">The number.</param>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        public Task PhoneCallAsync(string name, string number, byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.IncomingCall, cookie, number, name));
        }

        /// <summary>
        /// Starts a Phone call Ring.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        public Task RingAsync(byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.Ring, cookie));
        }

        /// <summary>
        /// Indicate that a Phone call has started.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
        public Task StartCallAsync(byte[] cookie)
        {
            return this._protocol.WriteMessage(new PhoneControlMessage(PhoneControlType.Start, cookie));
        }

        /// <summary>
        /// Indicate that a Phone call has ended.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        /// <returns>
        /// An async task to wait
        /// </returns>
        /// <remarks>
        /// Mainly for demoing capability
        /// </remarks>
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
        private static List<Pebble> FindPebbles()
        {
            List<Pebble> result = new List<Pebble>();

            try
            {
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = string.Empty;

#if NETFX_CORE
            PeerFinder.Start();
#endif
                IReadOnlyList<PeerInformation> pairedDevices = PeerFinder.FindAllPeersAsync().AsTask().Result;

                // Filter to only devices that are named Pebble - right now, that's the only way to
                // stop us getting headphones, etc. showing up...
                foreach (PeerInformation pi in pairedDevices)
                {
                    if (pi.DisplayName.StartsWith("Pebble", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(new Pebble(pi));
                    }
                }

                if (pairedDevices.Count == 0)
                {
                    Debug.WriteLine("No paired devices were found.");
                }
            }
            catch (Exception ex)
            {
                // If Bluetooth is turned off, we will get an exception. We catch it to return a zero-count list.
                Debug.WriteLine("Exception looking for Pebbles: " + ex.ToString());
            }

            return result;
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
                case Endpoint.PhoneVersion:
                    // We need to tell the Pebble what we are...
                    this._protocol.WriteMessage(new PhoneVersionMessage());
                    break;

                case Endpoint.Version:
                    // Store version info we got from the Pebble...
                    VersionMessage version = message as VersionMessage;
                    this.FirmwareVersion = version.Firmware;
                    this.RecoveryFirmwareVersion = version.RecoveryFirmware;
                    break;

                case Endpoint.Logs:
                    if (message as LogsMessage != null)
                    {
                        Debug.WriteLine("LOG: " + (message as LogsMessage).Message);
                    }

                    break;

                case Endpoint.MusicControl:
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

                    // PutBytes messages are state machines, so need special treatment...
                    if (message.Endpoint == Endpoint.PutBytes)
                    {
                        var putMessage = this._pendingMessage as PutBytesMessage;
                        if (putMessage.HandleStateMessage(message as PutBytesMessage))
                        {
                            this._pendingMessageSignal.Set();
                        }
                        else
                        {
                            this._protocol.WriteMessage(putMessage);
                        }
                    }
                    else
                    {
                        this._pendingMessage = message;
                        this._pendingMessageSignal.Set();
                    }
                }
                else
                {
                    // We've received a Log message when we were expecting something else,
                    // this means the protocol comms got messed up somehow, we should abort...
                    if (message.Endpoint == Endpoint.Logs)
                    {
                        this._pendingMessage = message;
                        this._pendingMessageSignal.Set();
                    }
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
                    await this._protocol.WriteMessage(message);

                    // Wait for the response...
                    this._pendingMessageSignal.Wait(millisecondsTimeout);

                    T pendingMessage = null;

                    if (this._pendingMessageSignal.IsSet)
                    {
                        // Store any response will be null if timed out...
                        pendingMessage = this._pendingMessage as T;
                    }

                    // See if we have a protocol error
                    LogsMessage logMessage = this._pendingMessage as LogsMessage;
                    Type pendingMessageType = this._pendingMessage.GetType();

                    // Clear the pending variables...
                    this._pendingMessageSignal = null;
                    this._pendingMessage = null;

                    int timeTaken = Environment.TickCount - startTicks;

                    if (pendingMessage != null)
                    {
                        Debug.WriteLine(pendingMessage.GetType().Name + " message received back in " + timeTaken.ToString() + "ms");
                    }
                    else if (logMessage != null)
                    {
                        // This is untested as it's an intermittent song...
                        throw new ProtocolException(logMessage);
                    }
                    else
                    {
                        Debug.WriteLine(message.GetType().Name + " message timed out in " + timeTaken.ToString() + "ms - type received was " + pendingMessageType.ToString());
                    }

                    return pendingMessage;
                });
        }
    }
}
