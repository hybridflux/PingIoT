# PingIoTHub - variable settings branch

This is a simple set of console programs to ping the IoT Hub with arbitrary MQTT messages and get its response time. It can prove useful if you want to find out the latency from your current location to the IoT Hub Azure datacenter of choice. It takes a settings.json file, where you enter the information and keys about your device and hub, and creates a csv file with timestamp, timespan from sending and receiving the message, and the running calculated mean time from start to current sent message. 

I took the [Connect your device to your IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted) documentation as starting point and modified it from there.

## Getting started

First, create an IoT Hub on the Azure portal. Once you have the IoT Hub, get its connection string and enter it in the settings.json file. Also enter the URI for the IoT Hub in the settings file. 

Second, create a json file, choose a name for your device and enter it in the file as the "deviceId". 

```
{
  "connectionString": "HostName=<your IoT Hub>.azure-devices.net;SharedAccessKeyName=iothubowner .....",
  "deviceId": "<your device id>",
  "deviceKey": "",
  "iotHubUri": "<your IoT Hub>.azure-devices.net"
}
```
You can place the json file anywhere. Just pass the path as an argument to *CreateDeviceIdentity*, when running it in the console. 
Exactly as in the [documentation](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted#create-a-device-identity), you will receive your device key. Copy it and paste the key as "deviceKey" into the settings.json file. 

Your settings.json file should now be complete with the device key.
```
{
  "connectionString": "HostName=<your IoT Hub>.azure-devices.net;SharedAccessKeyName=iothubowner .....",
  "deviceId": "<your device id>",
  "deviceKey": "<your device key>",
  "iotHubUri": "<your IoT Hub>.azure-devices.net"
}
```

## Run the programs

You can run the *SimulatedDevice* program, followed by the *ReadDeviceToCloudMessages* program. Do not run them direclty from Visual Studio, as they need the path to the settings-file as an argument. 

*ReadDeviceToCloudMessages* should output the message sent, the current datetime, the timespan from sent to received, and the mean timespan starting from the execution of the program to the current message. It also creates a csv-file named __log.csv__, which you can import in Excel and perform other calculations if necessary. 

## Authors

Founded on [Connect your device to your IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted)

* **Sherryl Manalo** - [particlehub.net](http://www.particlehub.net)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
