using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice
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
        static DeviceClient deviceClient;

    private static async void SendDeviceToCloudMessagesAsync(string deviceId)
        {
            // Send arbitrary data to IoT Hub and include timestamp
            double minTemperature = 20;
            double minHumidity = 60;
            int messageId = 1;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;
                DateTime currentDateTime = DateTime.UtcNow;

                var telemetryDataPoint = new
                {
                    messageId = messageId++,
                    deviceid = deviceId,
                    // Include datetime to process in ReadDeviceToCloudMessages 
                    datetime = currentDateTime,
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.UtcNow, messageString);

                await Task.Delay(1000);
            }
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
                    string deviceKey = IoTHubsettings.deviceKey;
                    string connectionString = IoTHubsettings.connectionString;

                    Console.WriteLine("Simulated device\n");
                    deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Amqp_WebSocket_Only);

                    SendDeviceToCloudMessagesAsync(deviceId);
                    Console.ReadLine();


                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                }

            }
            /*
            string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string _filePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(filePath).FullName).FullName).FullName + "\\settings.json";
            */
            else
            {
                Console.WriteLine("Usage: SimulatedDevice <path to json configuration file>");
            }


        
        }
    }
}
