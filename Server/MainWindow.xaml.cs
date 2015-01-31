using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        }

        private void InitializeParams()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            builder.Append("<wp:Notification xmlns:wp=\"WPNotification\">");
            builder.Append("      <wp:Tile>");
            builder.Append("            <wp:BackgroundImage>http://hattrickunited.org/cyclonesfc/files/2011/07/boobs1.jpg</wp:BackgroundImage>");
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
            if(string.IsNullOrEmpty(tbxUrl.Text))
            {
                MessageBox.Show("Please, enter url");
                return;
            }
            if (string.IsNullOrEmpty(tbxText.Text) || string.IsNullOrEmpty(tbxTitle.Text))
            {
                MessageBox.Show("Please, fill all the fields");
                return;
            }

            string url = tbxUrl.Text;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.Headers = new WebHeaderCollection();
            request.ContentType = "text/xml";

            request.Headers.Add("X-WindowsPhone-Target", "token");
            request.Headers.Add("X-NotificationClass", "1");

            string str = string.Format(tilePushXml, tbxTitle.Text, tbxText.Text);
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


    }
}
