using Microsoft.Phone.Controls;
using P3bble.Core.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;

namespace P3bble
{
    public partial class MainPage : PhoneApplicationPage
    {
        private P3bble.Core.P3bble _pebble;

        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<P3bble.Core.P3bble> pebbles = await P3bble.Core.P3bble.DetectPebbles();

            if (pebbles.Count >= 1)
            {
                _pebble = pebbles[0];
                await _pebble.ConnectAsync();

                if (_pebble != null && _pebble.IsConnected)
                {
                    PebbleName.Text = "Connected to Pebble " + _pebble.DisplayName;
                    PebbleVersion.Text = "Version " + _pebble.FirmwareVersion.Version + " - " + _pebble.FirmwareVersion.Timestamp.ToShortDateString();
                }
                else
                {
                    PebbleName.Text = "Not connected";
                    PebbleVersion.Text = string.Empty;
                }
            }

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                await _pebble.PingAsync();
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //    if (_pebble != null && _pebble.IsConnected)
        //        _pebble.BadPing();
        //    else
        //    {
        //        MessageBox.Show("Pebble not connected");
        //    }
        //}

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.SmsNotificationAsync("+436604908028", "wow, what a cool app :)");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.EmailNotificationAsync("root@p3.co.at", "P3bble", "youre sooo cooool :)");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.SetNowPlayingAsync("artist", "album", "track");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                DateTime time = await _pebble.GetTimeAsync();
                MessageBox.Show("Time is " + time.ToString() + " - " + Convert.ToInt32((DateTime.Now - time).TotalMinutes).ToString() + " minute(s) different from phone");
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                await _pebble.SetTimeAsync(DateTime.Now);
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.FacebookNotificationAsync("test", "testmessage");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                byte[] cookie = new byte[] { 0x00, 0xEB, 0x00, 0x00 };
                _pebble.PhoneCallAsync("P3root", "555 555 555", cookie);
                Thread.Sleep(2000);
                _pebble.RingAsync(cookie);
                Thread.Sleep(2000);
                _pebble.RingAsync(cookie);
                Thread.Sleep(2000);
                _pebble.RingAsync(cookie);

                Thread.Sleep(3000);
                _pebble.StartCallAsync(cookie);
                Thread.Sleep(5000);
                _pebble.EndCallAsync(cookie);
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private async void Button_Click_10(object sender, RoutedEventArgs e)
        {
            string fileName = await P3bbleBundle.DownloadFileAsync("https://pebblefw.s3.amazonaws.com/pebble/ev2_4/release/pbz/normal_ev2_4_v1.7.1.pbz");
           // fileName = P3bble.Core.Bundle.P3bbleBundle.DownloadFileAsync("http://pebble-static.s3.amazonaws.com/watchfaces/apps/simplicity.pbw");
            if (fileName != null)
            {
                P3bbleBundle bundle = new P3bbleBundle(fileName);

                MessageBox.Show("bundle is " + bundle.BundleType.ToString() + " - "); 
                    //bundle.BundleType == BundleType.Application ? bundle.Manifest);
            }
        }

        private async void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                if (_pebble.FirmwareVersion != null)
                {
                    var latest = await _pebble.GetLatestFirmwareVersionAsync();
                    if (latest.FirmwareVersion > _pebble.FirmwareVersion)
                    {
                        MessageBox.Show("new version available (" + latest.FirmwareVersion.Version.ToString() + " available, you have " + _pebble.FirmwareVersion.Version.ToString() + ")");
                    }
                }
                else
                    MessageBox.Show("does not receive version info from p3bble");
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private async void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                var apps = await _pebble.GetInstalledAppsAsync();
                if(apps != null)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendLine(apps.ApplicationBanks.ToString() + " app banks available");
                    msg.AppendLine(apps.ApplicationsInstalled.Count.ToString() + " apps installed");

                    foreach (var app in apps.ApplicationsInstalled)
                    {
                        msg.AppendLine(app.Name + " by " + app.Company);
                    }

                    MessageBox.Show(msg.ToString());
                }
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }
    }
}