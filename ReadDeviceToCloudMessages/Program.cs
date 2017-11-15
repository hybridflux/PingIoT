using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Newtonsoft.Json;


namespace ReadDeviceToCloudMessages
{
    // Settings parameters class (settings.json)
    public class Settings
    {
        public string iotHubUri;
        public string deviceKey;
        public string deviceId;
        public string connectionString;
    }
    class Program
    {
   
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            int i = 1;
            double meantime = 0;
            double summeantime = 0;
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

          
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. DateTime: {0} Partition: {1} Data: '{2}'", DateTime.UtcNow , partition, data);

                dynamic MyData = JsonConvert.DeserializeObject(data);

                string messageId = MyData.messageId;
                string deviceid = MyData.deviceid;
                DateTime datetime = MyData.datetime;
                float temperature = MyData.temperature;

                //calculate time difference from sent to received and write to console
                TimeSpan timedifference = DateTime.UtcNow - datetime;
                double timespan = timedifference.Milliseconds;
                Console.WriteLine("Datetime difference (ms): {0}", timespan);

                //calculate mean time 
                summeantime = summeantime + timespan;
                meantime = summeantime/i;
                Console.WriteLine("Mean: {0}", meantime);
                i = i+1;

                //write current timestamp, time difference, and mean time to csv file
                string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
                string csvText = Directory.GetParent(Directory.GetParent(Directory.GetParent(filePath).FullName).FullName).FullName + "\\log.csv";
              
                using (StreamWriter w = File.AppendText(csvText))
                {
                    Log(Convert.ToString(timespan) + "," + Convert.ToString(meantime), w);
                }


            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("{0},{1}", DateTime.UtcNow.ToLongTimeString(), logMessage);
        }

        static void Main(string[] args)
        {
            string jsonText = "";
            if (args.Length == 1)
            {
                try
                {
                    jsonText = File.ReadAllText(args[0]);
                    Settings IoTHubsettings = JsonConvert.DeserializeObject<Settings>(jsonText);
                    string iotHubUri = IoTHubsettings.iotHubUri;
                    string deviceId = IoTHubsettings.deviceId;

                    string connectionString = IoTHubsettings.connectionString;

                    Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
                    eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

                    var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

                    CancellationTokenSource cts = new CancellationTokenSource();

                    System.Console.CancelKeyPress += (s, e) =>
                    {
                        e.Cancel = true;
                        cts.Cancel();
                        Console.WriteLine("Exiting...");
                    };

                    var tasks = new List<Task>();
                    foreach (string partition in d2cPartitions)
                    {
                        tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
                    }
                    Task.WaitAll(tasks.ToArray());

                    }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                }

            }
            
            else
            {
                Console.WriteLine("Usage: ReadDeviceToCloudMessages <path to json configuration file>");
            }
        }
    }
}
