using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;


namespace CreateDeviceIdentity
{
    public class Settings
    {
        public string iotHubUri;
        public string deviceKey;
        public string deviceId;
        public string connectionString;
    }
    class Program
    {
        static RegistryManager registryManager;

        static string filePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        static string _filePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(filePath).FullName).FullName).FullName + "\\settings.json";
        static string jsonText = File.ReadAllText(_filePath);

        static Settings IoTHubsettings = JsonConvert.DeserializeObject<Settings>(jsonText);
        static string iotHubUri = IoTHubsettings.iotHubUri;
        static string deviceId = IoTHubsettings.deviceId;

        static string connectionString = IoTHubsettings.connectionString;
        
        private static async Task AddDeviceAsync()
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
        }

        static void Main(string[] args)
        {
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }
    }
}
