using P3bble.Core.Communication;
using P3bble.Core.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Windows.Networking;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;

namespace P3bble.Core
{
    public class P3bble
    {
        private Protocol _prot;

        public P3bble(string displayName, HostName hostName, string serviceName)
        {
            DisplayName = displayName;
            HostName = hostName;
            ServiceName = serviceName;
        }

        public EventHandler Connected;
        public EventHandler ConnectionError;

        public string DisplayName { get; private set;  }
        public HostName HostName { get; private set; }
        public string ServiceName { get; private set;  }

        public static List<P3bble> DetectPebbles()
        {
            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
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
                    lst.Add(new P3bble(pi.DisplayName, pi.HostName, pi.ServiceName));
                }
            }
            return lst;
        }

        public async void Connect()
        {
            StreamSocket socket = new StreamSocket();
            try
            {
                await socket.ConnectAsync(HostName, "1");
                _prot = new Protocol(socket);
                if(Connected != null)
                    Connected(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if(ConnectionError != null)
                    ConnectionError(this, EventArgs.Empty);
            }
        }

        public void Ping()
        {
            _prot.WriteMessage(new PingMessage());
            P3bbleMessage message = _prot.ReadMessage();
        }

        public void Reset()
        {
            _prot.WriteMessage(new ResetMessage());
        }


    }
}
