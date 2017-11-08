# PingIoTHub

This is a simple set of console programs to ping the IoT Hub with arbitrary MQTT messages and get its response time. It can prove useful if you want to find out the latency from your current location to the IoT Hub Azure datacenter of choice. It takes a settings.json file, where you enter the information and keys about your device and hub, and creates a csv file with timestamp, timespan from sending and receiving the message, and the running calculated mean time from start to current sent message. 

I took the [Connect your device to your IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted) documentation as starting point and modified it from there.

## Getting started

First, create an IoT Hub on the Azure portal. Once you have the IoT Hub, get its connection string and enter it in the settings.json file. Also enter the URI for the IoT Hub in the settings file. 

Second, choose a name for your device and enter it as the "deviceId" settings.json.

```
{
  "connectionString": "HostName=<your IoT Hub>.azure-devices.net;SharedAccessKeyName=iothubowner .....",
  "deviceId": "<your device id>",
  "deviceKey": "",
  "iotHubUri": "<your IoT Hub>.azure-devices.net"
}
```
Then run *CreateDeviceIdentity* on the console. Exactly as in the __Getting Started__ documentation, you will receive your device key. Copy it and paste the key as "deviceKey" into the settings.json file. 

Your settings.json file should now be complete
```
{
  "connectionString": "HostName=<your IoT Hub>.azure-devices.net;SharedAccessKeyName=iothubowner .....",
  "deviceId": "<your device id>",
  "deviceKey": "<your device key>",
  "iotHubUri": "<your IoT Hub>.azure-devices.net"
}
```

## Run the programs

You can run the *SimulatedDevice* program, followed by the *ReadDeviceToCloudMessages* program. If you are in Visual Studio 2017, you can set the startup programs as described [here](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted#run-the-apps).

*ReadDeviceToCloudMessages* should output the message sent, the current datetime, the timespan from sent to received, and the mean timespan starting from the execution of the program to the current message. It also creates a csv-file named __log.csv__, which you can import in Excel and perform other calculations if necessary. 

## Authors

Founded on [Connect your device to your IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-getstarted)

* **Sherryl Manalo** - [particlehub.net](http://www.particlehub.net)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
