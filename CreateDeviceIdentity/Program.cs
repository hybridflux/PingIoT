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

        
        private static async Task AddDeviceAsync(string deviceId)
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
                    registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                    AddDeviceAsync(deviceId).Wait();
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
            else {
                Console.WriteLine("Usage: CreateDeviceIdentity <path to json configuration file>");
            }

       
        }
    }
}
