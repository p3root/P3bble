using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using P3bble.Core.EventArguments;
using P3bble.Resources;
using System.Threading;
using P3bble.Core.Bundle;

namespace P3bble
{
    public partial class MainPage : PhoneApplicationPage
    {
        private P3bble.Core.P3bble _peb;
        private bool _connected = false;

        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<P3bble.Core.P3bble> pebbles = P3bble.Core.P3bble.DetectPebbles().Result;

            if (pebbles.Count >= 1)
            {
                _peb = pebbles[0];
                _peb.Connected += PebbleConnected;
				_peb.MusicControlReceived += MusicControlReceived;
                _peb.Connect();
                
                
            }

        }

	    private void MusicControlReceived(object sender, MusicControlEventArgs musicControlEventArgs)
	    {
		    Dispatcher.BeginInvoke(() => MusicControlTextBlock.Text = string.Concat(musicControlEventArgs.Command, " received"));
	    }

	    private void PebbleConnected(object sender, EventArgs e)
        {
            _connected = true;
            _peb.GetVersion();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.Ping();
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.BadPing();
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.SmsNotification("+436604908028", "wow, what a cool app :)");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.EmailNotification("root@p3.co.at", "P3bble", "youre sooo cooool :)");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.SetNowPlaying("artist", "album", "track");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.GetTime();
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.GetVersion();
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (_connected)
                _peb.FacebookNotification("test", "testmessage");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (_connected)
            {
                byte[] cookie = new byte[] { 0x00, 0xEB, 0x00, 0x00 };
                _peb.PhoneCall("P3root", "555 555 555", cookie);
                Thread.Sleep(2000);
                _peb.Ring(cookie);
                Thread.Sleep(2000);
                _peb.Ring(cookie);
                Thread.Sleep(2000);
                _peb.Ring(cookie);

                Thread.Sleep(3000);
                _peb.StartCall(cookie);
                Thread.Sleep(5000);
                _peb.EndCall(cookie);
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        string fileName = "";
        
        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            fileName = P3bble.Core.Bundle.P3bbleBundle.DownloadFileAsync("https://pebblefw.s3.amazonaws.com/pebble/ev2_4/release/pbz/normal_ev2_4_v1.7.1.pbz");
           // fileName = P3bble.Core.Bundle.P3bbleBundle.DownloadFileAsync("http://pebble-static.s3.amazonaws.com/watchfaces/apps/simplicity.pbw");
            P3bble.Core.Bundle.P3bbleBundle.OpenReadCompleted += P3bbleBundle_OpenReadCompleted;
         
        }

        private void P3bbleBundle_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            P3bbleBundle bundle = new P3bbleBundle(fileName);
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (_connected)
            {
                if (_peb.FirmwareVersion != null)
                {
                    _peb.CheckForNewFirmwareCompleted += _peb_CheckForNewFirmwareCompleted;
                    _peb.CheckForNewFirmwareAsync(_peb.FirmwareVersion);
                }
                else
                    MessageBox.Show("does not receive version info from p3bble");
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void _peb_CheckForNewFirmwareCompleted(object sender, Core.EventArguments.CheckForNewFirmwareVersionEventArgs e)
        {
            if (e.NewVersionAvailable)
            {
                MessageBox.Show("new version available");
            }
        }
    }
}