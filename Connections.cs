using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Connections
    {
        static Connections()
        {
            StorageConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            BlobContainerName = CloudConfigurationManager.GetSetting("BlobContainerName");
            TopicName = CloudConfigurationManager.GetSetting("TopicName");
            ServiceBusNS = CloudConfigurationManager.GetSetting("ServiceBusNS");

            AccessKeyName = CloudConfigurationManager.GetSetting("AccessKeyName");
            AccessKeyKey = CloudConfigurationManager.GetSetting("AccessKeyKey");

            ServiceBusConnectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
        }

        public static string StorageConnectionString { get; set; }

        public static string BlobContainerName { get; set; }
        public static string TopicName { get; set; }
        public static string ServiceBusNS { get; set; }

        public static string AccessKeyName { get; set; }
        public static string AccessKeyKey { get; set; }

        public static string ServiceBusConnectionString { get; set; }
    }
}
