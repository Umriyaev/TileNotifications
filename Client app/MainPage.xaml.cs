using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Notification;
using System.Diagnostics;
using Client_app.Resources;

namespace Client_app
{
    public partial class MainPage : PhoneApplicationPage
    {
        HttpNotificationChannel notificationChannel;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        Uri channelUri;
        public Uri ChannelUri
        {
            get { return this.channelUri; }
            set
            {
                this.channelUri = value;
                Debug.WriteLine(value.ToString());
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            SetupChannel();
        }

        private void SetupChannel()
        {
            string channelName = "Demo notification channel";
            notificationChannel = HttpNotificationChannel.Find(channelName);
            if (notificationChannel != null)
            {
                notificationChannel.ChannelUriUpdated += notificationChannel_ChannelUriUpdated;
                notificationChannel.ErrorOccurred += notificationChannel_ErrorOccurred;
                notificationChannel.HttpNotificationReceived += notificationChannel_HttpNotificationReceived;
                notificationChannel.ShellToastNotificationReceived += notificationChannel_ShellToastNotificationReceived;
                notificationChannel.ConnectionStatusChanged += notificationChannel_ConnectionStatusChanged;
                Debug.WriteLine(notificationChannel.ChannelUri.ToString());
            }
            else
            {
                notificationChannel = new HttpNotificationChannel(channelName);

                notificationChannel.ChannelUriUpdated += notificationChannel_ChannelUriUpdated;
                notificationChannel.ErrorOccurred += notificationChannel_ErrorOccurred;
                notificationChannel.HttpNotificationReceived += notificationChannel_HttpNotificationReceived;
                notificationChannel.ShellToastNotificationReceived += notificationChannel_ShellToastNotificationReceived;
                notificationChannel.ConnectionStatusChanged+=notificationChannel_ConnectionStatusChanged;

                notificationChannel.Open();
                BindToShell();
            }
        }

        private void notificationChannel_ConnectionStatusChanged(object sender, NotificationChannelConnectionEventArgs e)
        {
            Debug.WriteLine("Channel connection status changed to " + e.ConnectionStatus.ToString());
        }

        private void BindToShell()
        {
            try
            {
                if (!notificationChannel.IsShellTileBound)
                {
                    notificationChannel.BindToShellTile();
                }
                if (!notificationChannel.IsShellToastBound)
                {
                    notificationChannel.BindToShellToast();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception occured binding to shell: " + e.Message);
            }
        }

        private void notificationChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbxInfo.Text = "Toast notification received: ";
                tbxInfo.Text += "\nCollection: ";
                foreach (var item in e.Collection.Keys)
                {
                    tbxInfo.Text += string.Format("\nKey: {0}, Value: {1}\n", item, e.Collection[item]);
                }
            });
        }

        private void notificationChannel_HttpNotificationReceived(object sender, HttpNotificationEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbxInfo.Text = string.Format("Http notification received: \nBody: {0}\nChannel: {1}", e.Notification.Body, e.Notification.Channel);
                tbxInfo.Text += "\nHeaders: ";
                foreach (var header in e.Notification.Headers)
                {
                    tbxInfo.Text += "\n" + header.ToString();
                }
            });
        }

        private void notificationChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Debug.WriteLine("Error occured: " + e.Message);
        }

        private void notificationChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            ChannelUri = e.ChannelUri;
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