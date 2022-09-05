# iot-edge-echo

IoT Edge module which visualizes incoming messages and outputs them unchanged. It's great for getting insights into your IoT Edge routing.

## Introduction

This is a C# .Net Standard module, written for Azure IoTEdge version GA.

This module is a simple module that helps to go get insights about module outputs and outputted messages.

It's great to check the output from third party modules (eg. the Azure Stream Analytics module).

You can just put it next to another route to listen to the communication between these two routes.

*Note:* This module is not capable of splitting multiple streams of input data flowing through it.

## Docker Hub

A version generated for Docker Linux can be found at [https://hub.docker.com/r/iotedgefoundation/iot-edge-echo/](https://hub.docker.com/r/iotedgefoundation/iot-edge-echo/)

You can pull it with **docker pull iotedgefoundation/iot-edge-echo** but I suggest using a more specific name like **iotedgefoundation/iot-edge-echo:3.0.7-amd64** (for Linux containers) when you deploy it using the Azure portal or the IoT Edge deployment manifest.

## Module Twin

This module does not support 'desired' or 'reported' properties.

## Routing input and outputs

This module ingests telemetry messages using input **input1**.

The messages passed through are sent using output **output1**

### Output messages

The output messages are the same as inputted message except for one extra property:

```javascript
Properties.Add("content-type", "application/edge-output1-echo-json");
```

_Note_: If the incoming message is not a valid JSON message, the message will throw an exception instead of output.

### Routes

Use this example route:

```json
{
  "routes": {
    "heartbeatToEcho": "FROM /messages/modules/heartbeat/outputs/output1 INTO BrokeredEndpoint(\"/modules/echo/inputs/input1\")",
    "route": "FROM /messages/modules/echo/outputs/output1/* INTO $upstream"
  }
}
```

## Environment variables

On initialize, this module exposes all available environment variables in the logging.


## GZip, Deflate

Messages which ContentEncoding is "gzip" and ContentType is "application/zip" will be unzipped using Gzip decompression. 

Messages which ContentEncoding is "defalte" and ContentType is "application/zip" will be unzipped using Deflate decompression.

The library [ZipHelper](https://www.nuget.org/packages/ZipHelperLib/) used, follows the [System.IO.Compression](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression?view=net-5.0) guidelines.

## Contribute

This logic is licensed under the MIT license.

Want to contribute? Throw in a pull request....

Sourcecode is available [here](https://github.com/iot-edge-foundation/iot-edge-echo).

Want to know more about me? Check out my [blog](http://blog.vandevelde-online.com)
