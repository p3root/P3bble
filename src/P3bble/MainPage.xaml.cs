using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using P3bble.Resources;

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
            List<P3bble.Core.P3bble> pebbles = P3bble.Core.P3bble.DetectPebbles();

            if (pebbles.Count >= 1)
            {
                _peb = pebbles[0];
                _peb.Connected += PebbleConnected;
                _peb.Connect();
                
                
            }

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

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}