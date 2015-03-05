using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeParams();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ServiceHost host = new ServiceHost(typeof(RegistrationService));
            host.Open();
        }

        private void InitializeParams()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<wp:Notification xmlns:wp=\"WPNotification\">");
            builder.Append("      <wp:Tile>");
            builder.Append("            <wp:BackgroundImage>{2}</wp:BackgroundImage>");
            builder.Append("            <wp:Count>{0}</wp:Count>");
            builder.Append("            <wp:Title>{1}</wp:Title>");
            builder.Append("      </wp:Tile>");
            builder.Append("</wp:Notification>");

            tilePushXml = builder.ToString();

            builder.Clear();
            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<wp:Notification xmlns:wp=\"WPNotification\">");
            builder.Append("      <wp:Toast>");
            builder.Append("            <wp:Text1>{0}</wp:Text1>");
            builder.Append("            <wp:Text2>{1}</wp:Text2>");
            builder.Append("            <wp:Param>?developer=Shokhrukh Umriyaev</wp:Param>");
            builder.Append("      </wp:Toast>");
            builder.Append("</wp:Notification>");

            toastPushXml = builder.ToString();
        }

        string toastPushXml = string.Empty;
        string tilePushXml = string.Empty;

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbxUrl.Text))
            {
                MessageBox.Show("Please, enter url");
                return;
            }
            if (string.IsNullOrEmpty(tbxText.Text) || string.IsNullOrEmpty(tbxTitle.Text) || string.IsNullOrEmpty(tbxImage.Text))
            {
                MessageBox.Show("Please, fill all the fields");
                return;
            }

            string url = tbxUrl.Text;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.Headers = new WebHeaderCollection();
            request.ContentType = "text/xml";

            /*headers for tile notification*/
            //request.Headers.Add("X-WindowsPhone-Target", "token");
            //request.Headers.Add("X-NotificationClass", "1");

            /*headers for raw notifications*/
            request.Headers.Add("X-WindowsPhone-Target", "");
            request.Headers.Add("X-NotificationClass", "3");

            string str = string.Format(tilePushXml, tbxTitle.Text, tbxText.Text, tbxImage.Text);
            byte[] strBytes = Encoding.Default.GetBytes(str);
            request.ContentLength = strBytes.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(strBytes, 0, strBytes.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string notificationStatus = response.Headers["X-NotificationStatus"];
            string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];
            string subscriptionStatus = response.Headers["X-SubscriptionStatus"];
            lblStatus.Text = "Notification status: " + notificationStatus + "; Device connection status : " + deviceConnectionStatus + "; Subscription status: " + subscriptionStatus;
        }

        private void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbxText.Text) || string.IsNullOrEmpty(tbxTitle.Text))
            {
                MessageBox.Show("Please, fill text and title");
                return;
            }

            List<Uri> allSubscribersUri = RegistrationService.GetSubscribers();
            foreach (Uri subscriberUri in allSubscribersUri)
            {
                sendPushNotificationToClient(subscriberUri.ToString());
            }
        }

        private void sendPushNotificationToClient(string url)
        {
            HttpWebRequest pushNotification = (HttpWebRequest)WebRequest.Create(url);
            pushNotification.Method = "POST";
            pushNotification.Headers = new WebHeaderCollection();
            pushNotification.ContentType = "text/xml";

            pushNotification.Headers.Add("X-WindowsPhone-Target", "toast");
            pushNotification.Headers.Add("X-NotificationClass", "2");

            string str = string.Format(toastPushXml, tbxTitle.Text, tbxText.Text);
            byte[] strBytes = Encoding.Default.GetBytes(str);

            pushNotification.ContentLength = strBytes.Length;

            using (Stream requestStream = pushNotification.GetRequestStream())
            {
                requestStream.Write(strBytes, 0, strBytes.Length);
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)pushNotification.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];
                string subscriptionStatus = response.Headers["X-SubscriptionStatus"];
                lblStatus.Text += "Notification status: " + notificationStatus + "; Device connection status : " + deviceConnectionStatus + "; Subscription status: " + subscriptionStatus + "\n";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception occured while sending notification: " + ex.Message);
                lblStatus.Text = "Failed to connect, exception detail: " + ex.Message;
            }
        }


    }
}
