﻿using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using P3bble.Core;
using P3bble.Core.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await TryConnection();
        }

        private async Task TryConnection()
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
                    _pebble.MusicControlReceived += new MusicControlReceivedHandler(this.MusicControlReceived);
                    _pebble.InstallProgress += new InstallProgressHandler(this.InstallProgressReceived);
                    RetryConnection.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PebbleName.Text = "Not connected";
                    PebbleVersion.Text = string.Empty;
                    RetryConnection.Visibility = Visibility.Visible;
                }
            }
        }

        private async void Retry_Click(object sender, RoutedEventArgs e)
        {
            await TryConnection();
        }

        private async void Ping_Click(object sender, RoutedEventArgs e)
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

        private void SmsNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.SmsNotificationAsync("+436604908028", "wow, what a cool app :)");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void EmailNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.EmailNotificationAsync("root@p3.co.at", "P3bble", "youre sooo cooool :)");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.Queue.ActiveSong != null)
            {
                _pebble.SetNowPlayingAsync(MediaPlayer.Queue.ActiveSong.Artist.Name, MediaPlayer.Queue.ActiveSong.Album.Name, MediaPlayer.Queue.ActiveSong.Name);
            }
            else
            {
                _pebble.SetNowPlayingAsync(string.Empty, string.Empty, string.Empty);
            }
        }

        private void MusicControlReceived(MusicControlAction action)
        {
            switch (action)
            {
                case MusicControlAction.PlayPause:
                    if (MediaPlayer.State == MediaState.Playing)
                    {
                        MediaPlayer.Pause();
                    }
                    else
                    {
                        MediaPlayer.Resume();
                    }

                    break;

                case MusicControlAction.Next:
                    MediaPlayer.MoveNext();
                    break;

                case MusicControlAction.Previous:
                    MediaPlayer.MovePrevious();
                    break;
            }
        }

        private void PlayMusic_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                MediaLibrary lib = new MediaLibrary();
                MediaPlayer.Play(lib.Songs, new Random().Next(lib.Songs.Count));
                MediaPlayer.IsShuffled = true;
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private async void GetTime_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                DateTime time = await _pebble.GetTimeAsync();
                MessageBox.Show("Time is " + time.ToString() + " - " + Math.Abs(Convert.ToInt32((DateTime.Now - time).TotalMinutes)).ToString() + " minute(s) different from phone");
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private async void SetTime_Click(object sender, RoutedEventArgs e)
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

        private void FacebookNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
                _pebble.FacebookNotificationAsync("test", "testmessage");
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }

        private void PhoneCall_Click(object sender, RoutedEventArgs e)
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

        private async void DownloadApp_Click(object sender, RoutedEventArgs e)
        {
            this.InstallAppProgress.IsIndeterminate = true;
            this.InstallAppProgress.Value = 0;
            this.InstallAppProgress.Visibility = Visibility.Visible;

            try
            {
                //P3bbleBundle bundle = await this._pebble.DownloadBundleAsync("https://pebblefw.s3.amazonaws.com/pebble/ev2_4/release/pbz/normal_ev2_4_v1.7.1.pbz");
                //P3bbleBundle bundle = await this._pebble.DownloadBundleAsync("http://pebble-static.s3.amazonaws.com/watchfaces/apps/simplicity.pbw");
                P3bbleBundle bundle = await this._pebble.DownloadBundleAsync("http://u.jdiez.me/pixel.pbw");
                if (bundle != null)
                {
                    MessageBox.Show("bundle is " + bundle.BundleType.ToString() + (bundle.BundleType == BundleType.Application ? (" - " + bundle.Application.AppName) : ""));
                    await this._pebble.InstallApp(bundle, true);
                }
            }
            catch (Exception ex)
            {
                this.InstallAppProgress.IsIndeterminate = false;
                this.InstallAppProgress.Value = 0;
                this.InstallAppProgress.Visibility = Visibility.Collapsed;

                MessageBox.Show(ex.Message);
            }
        }

        private void InstallProgressReceived(int percentComplete)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    this.InstallAppProgress.IsIndeterminate = false;
                    this.InstallAppProgress.Value = percentComplete;
                    if (percentComplete == 0 || percentComplete == 100)
                    {
                        this.InstallAppProgress.Visibility = Visibility.Collapsed;
                    }
                });
        }

        //private void LaunchApp_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_pebble != null && _pebble.IsConnected)
        //    {
        //        _pebble.LaunchApp(new Guid("deadefde-acfe-efbe-99ef-beefbeefbeef"));
        //    }
        //}

        private async void CheckFirmware_Click(object sender, RoutedEventArgs e)
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

        private async void GetInstalledApps_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                var apps = await _pebble.GetInstalledAppsAsync();
                if (apps != null)
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

        private async void RemoveApp_Click(object sender, RoutedEventArgs e)
        {
            if (_pebble != null && _pebble.IsConnected)
            {
                var apps = await _pebble.GetInstalledAppsAsync();
                if (apps != null && apps.ApplicationsInstalled.Count > 0)
                {
                    if (MessageBox.Show("This example will remove the first app found: " + apps.ApplicationsInstalled[0].Name + " - are you sure you want to continue?", "DANGER!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        await _pebble.RemoveAppAsync(apps.ApplicationsInstalled[0]);
                    }
                }
                else
                {
                    MessageBox.Show("No apps installed");
                }
            }
            else
            {
                MessageBox.Show("Pebble not connected");
            }
        }
    }
}