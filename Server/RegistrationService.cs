using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RegistrationService" in both code and config file together.
    public class RegistrationService : IRegistrationService
    {
        private static List<Uri> subscribers = new List<Uri>();
        private static object obj = new object();
        public void Register(string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            Subscribe(channelUri);
        }

        private void Subscribe(Uri channelUri)
        {
            lock (obj)
            {
                if (!subscribers.Exists((u) => u == channelUri))
                {
                    subscribers.Add(channelUri);
                }
            }
        }

        public void Unregister(string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            Unsubscribe(channelUri);
        }

        private void Unsubscribe(Uri channelUri)
        {
            lock (obj)
            {
                subscribers.Remove(channelUri);
            }
        }

        public static List<Uri> GetSubscribers()
        {
            return subscribers;
        }
    }
}
