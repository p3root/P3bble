using P3bble.Core.Communication;
using P3bble.Core.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows.Input;
using System.Windows;
using Windows.Networking;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

using System.Linq;
using System.Threading.Tasks;

#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;

using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using P3bble.Core.Constants;
using P3bble.Core.Firmware;
using System.Runtime.Serialization.Json;
using System.IO;
using P3bble.Core.Helper;
using P3bble.Core.EventArguments;
#endif

namespace P3bble.Core
{
    public class P3bble
    {
        private Protocol _prot;
        public P3bble(PeerInformation peerInformation)
        {
            PeerInformation = peerInformation;
        }

        public EventHandler Connected;
        public EventHandler ConnectionError;
		public EventHandler<MusicControlEventArgs> MusicControlReceived;

        public PeerInformation PeerInformation { get; private set; }
        public P3bbleFirmwareVersion FirmwareVersion { get; private set; }
        public P3bbleFirmwareVersion RecoveryFirmwareVersion { get; private set; }

        public static Task<List<P3bble>> DetectPebbles()
        {
            return Task<List<P3bble>>.Factory.StartNew(() => FindPebbles());
        }

        private static List<P3bble> FindPebbles()
        {
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";

#if NETFX_CORE
            PeerFinder.Start();
#endif

            IReadOnlyList<PeerInformation> pairedDevices = PeerFinder.FindAllPeersAsync().AsTask().Result;
            List<P3bble> lst = new List<P3bble>();
            if (pairedDevices.Count == 0)
            {
                Debug.WriteLine("No paired devices were found.");
            }
            else
            {
                foreach (PeerInformation pi in pairedDevices)
                {
                    lst.Add(new P3bble(pi));
                }
            }
            return lst;
        }

        public async void Connect()
        {
            StreamSocket socket = new StreamSocket();
            try
            {
#if WINDOWS_PHONE
                await socket.ConnectAsync(PeerInformation.HostName, new Guid(0x00001101, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB).ToString("B"));
#elif NETFX_CORE
                socket = Windows.Networking.Proximity.PeerFinder.ConnectAsync(PeerInformation).AsTask().Result;
#endif
               
                _prot = new Protocol(socket);
                _prot.MessageReceived += AsynMessageRecived;

                if(Connected != null)
                    Connected(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if(ConnectionError != null)
                    ConnectionError(this, EventArgs.Empty);
            }
        }

        private void AsynMessageRecived(object sender, P3bbleMessageReceivedEventArgs e)
        {
	        switch (e.Message.Endpoint)
	        {
		        case P3bbleEndpoint.PhoneVersion:
			        _prot.WriteMessage(new PhoneVersionMessage());
			        break;
		        case P3bbleEndpoint.Version:
			        var vMessage = e.Message as VersionMessage;
			        if (vMessage != null)
			        {
						FirmwareVersion = vMessage.Firmware;
						RecoveryFirmwareVersion = vMessage.RecoveryFirmware;
			        }
			        break;
				case P3bbleEndpoint.MusicControl:
			        var mcMessage = e.Message as MusicControlMessage;
			        if (mcMessage != null && MusicControlReceived != null)
				        MusicControlReceived(this, new MusicControlEventArgs() {Command = mcMessage.Command});
			        break;
	        }
	        Debug.WriteLine(e.Message.Endpoint);
        }

	    public void Ping()
        {
            _prot.WriteMessage(new PingMessage());
        }

        public void BadPing()
        {
            _prot.WriteMessage(new PingMessage(new byte[7]{1, 2, 3, 4, 5, 6, 7 }));
        }

        public void GetVersion()
        {
            _prot.WriteMessage(new VersionMessage());
        }

        public void Reset()
        {
            _prot.WriteMessage(new ResetMessage());
        }

        public void SmsNotification(string sender, string message)
        {
            _prot.WriteMessage(new NotificationMessage(NotificationType.SMS, sender, message));
        }

        public void FacebookNotification(string sender, string message)
        {
            _prot.WriteMessage(new NotificationMessage(NotificationType.Facebook, sender, message));
        }

        public void EmailNotification(string sender, string subject, string body)
        {
            _prot.WriteMessage(new NotificationMessage(NotificationType.EMAIL, sender, body, subject));
        }

        public void SetNowPlaying(string artist, string album, string track)
        {
            _prot.WriteMessage(new SetMusicMessage(artist, album, track));
        }

        public void GetTime()
        {
            _prot.WriteMessage(new TimeMessage());
        }

        public void SetTime(DateTime dt)
        {
            _prot.WriteMessage(new TimeMessage(TimeMessageAction.SetTime, dt));
        }

        public void PhoneCall(string name, string number, byte[] cookie)
        {
            _prot.WriteMessage(new PhoneControlMessage(PhoneControlType.INCOMING_CALL, cookie, number, name));
        }
        public void Ring(byte[] cookie)
        {
            _prot.WriteMessage(new PhoneControlMessage(PhoneControlType.RING, cookie));
        }
        public void StartCall(byte[] cookie)
        {
            _prot.WriteMessage(new PhoneControlMessage(PhoneControlType.START, cookie));
        }
        public void EndCall(byte[] cookie)
        {
            _prot.WriteMessage(new PhoneControlMessage(PhoneControlType.END, cookie));
        }

        public event EventHandler<CheckForNewFirmwareVersionEventArgs> CheckForNewFirmwareCompleted;

        public void CheckForNewFirmwareAsync(P3bbleFirmwareVersion version, bool useNightlyBuild = false)
        {
            string url = version.GetFirmwareServerUrl(useNightlyBuild);

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
                    if (Util.IsNewerVersionAvailable(FirmwareVersion.Version, info.Normal.FriendlyVersion))
                    {
                        eventArgs = new CheckForNewFirmwareVersionEventArgs(true, info);
                    }

                    if (CheckForNewFirmwareCompleted != null)
                        CheckForNewFirmwareCompleted(this, eventArgs);
                };
           
        }

    }
}
