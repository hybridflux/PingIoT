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

        // Read settings.json and use parameters
        static string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        static string _filePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(filePath).FullName).FullName).FullName + "\\settings.json";
        static string jsonText = File.ReadAllText(_filePath);

        static Settings IoTHubsettings = JsonConvert.DeserializeObject<Settings>(jsonText);
        static string iotHubUri = IoTHubsettings.iotHubUri;
        static string deviceId = IoTHubsettings.deviceId;
        static string deviceKey = IoTHubsettings.deviceKey;


    private static async void SendDeviceToCloudMessagesAsync()
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
                DateTime currentDateTime = DateTime.Now;

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
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }



        static void Main(string[] args)
        {
         

                Console.WriteLine("Simulated device\n");
                deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey), TransportType.Mqtt);

                SendDeviceToCloudMessagesAsync();
                Console.ReadLine();
          

        
        }
    }
}
