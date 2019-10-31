# iot-edge-echo
IoT Edge module which visualizes incoming messages and outputs them unchanged. Great for getting insights in your IoT Edge routing.

## Introduction

This is a C# .Net Standard module, written for Azure IoTEdge version GA.

This module is a simple module which helps to go get insights about module outputs and outputted messages.

## Docker Hub

A version generated for Docker Linux can be found at https://hub.docker.com/r/svelde/iot-edge-echo/

You can pull it with **docker pull svelde/iot-edge-echo** but I suggest to use **svelde/iot-edge-echo:1.0** when you deploy it using the Azure portal.

## Module Twin

This module does not support 'desired' properties.

## Routing input and outputs

This module ingest telemetry messages using input **input1**.

The messages passed through are sent using output **output1**

## Output messages

The output messages are the same as inputted message except for an extra property like:

```
Properties.Add("content-type", "application/edge-echo-json");
```

or

```
Properties.Add("content-type", "application/edge-output1-json");
```

## Environment variables

On initialize, this module exposes all available environment variables in the logging.

## Contribute

This logic is licenced under the MIT license.

Want to contribute? Throw in a pull request....

Want to know more about me? Check out my [blog](http://blog.vandevelde-online.com)








