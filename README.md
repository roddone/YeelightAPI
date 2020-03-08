[![Build Status](https://romainoddone.visualstudio.com/Github%20Builds/_apis/build/status/YeelightAPI?branchName=master)](https://romainoddone.visualstudio.com/Github%20Builds/_build/latest?definitionId=7&branchName=master)
[![NuGet Package](https://img.shields.io/nuget/v/YeelightAPI.svg)](https://www.nuget.org/packages/YeelightAPI/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/YeelightAPI.svg)](https://www.nuget.org/packages/YeelightAPI/)
[![Donate](https://img.shields.io/badge/%24-donate-ff00ff.svg)](https://www.paypal.me/roddone)

# YeelightAPI
C# API (.Net) to control Xiaomi Yeelight Color Bulbs

## Contribution
If you find this package useful, please make a gift on Paypal : [https://www.paypal.me/roddone](https://www.paypal.me/roddone)

## Prerequisites
* The console project uses C# 7.1 "Async Main Method" Feature, make sure your visual studio version is up to date !
* [Enable the "developer mode"](https://www.yeelight.com/en_US/developer) on your devices, otherwhise they will neither be discovered nor usable by this API

## Installation
To install the latest release from [NuGet package manager](https://www.nuget.org/packages/YeelightAPI/):

    Install-Package YeelightAPI

## Usage
### Single Device
The `YeelightAPI.Device` allows you to create a device. Just instanciate a new Device with an ip adress or a hostname: ` Device device = new Device("hos.tna.meo.rIP");` and initiate connection : `device.Connect();`.
Then you can use the device object to control the device : 
* Power on / off : `device.SetPower(true);`
* Toggle State : `device.Toggle();`
* Change brightness level : `device.SetBrightness(100);`
* Change color : `device.SetRGBColor(80, 244, 255);`
* ...

Some methods use an optional parameter named "smooth", it refers to the duration in milliseconds of the effect you want to apply. For a progressive brightness change, use `device.SetBrightness(100, 3000);`.

If you need a method that is not implemented, you can use the folowing methods :
* `ExecuteCommandWithResponse(METHODS method, int id = 0, List<object> parameters = null)` (with response) 
* `ExecuteCommand(METHODS method, int id = 0, List<object> parameters = null)` (without response).

These methods are generic and use the `METHODS` enumeration and a list of parameters, which allows you to call any known method with any parameter.
All the parameters are defined in the doc ["Yeelight WiFi Light Inter-Operation Specification"](http://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf "Link to Yeelight WiFi Light Inter-Operation Specification"), section 4.1 : COMMAND Message.

### Multiple-devices
If you need to control multiple devices at a time, you can use the `YeelightAPI.DeviceGroup` class. 
This class simply ihnerits from native .net `List<Device>` and implements the `IDeviceController` interface, allowing you to control multiple devices the exact same way you control a single device.
```csharp
DeviceGroup group = new DeviceGroup();
group.Add(device1);
group.Add(device2);

group.Connect();
group.Toggle();
...
```

### Color Flow
#### Old School
You can create a Color Flow to "program" your device with different state changes. Changes can be : RGB color, color temperature and brightness.
Just create a `new ColorFlow()`, add some `new ColorFlowExpression()` to it, and starts the color flow with your ColorFlow object.
```csharp
ColorFlow flow = new ColorFlow(0, ColorFlowEndAction.Restore);
flow.Add(new ColorFlowRGBExpression(255, 0, 0, 1, 500)); // color : red / brightness : 1% / duration : 500
flow.Add(new ColorFlowRGBExpression(0, 255, 0, 100, 500)); // color : green / brightness : 100% / duration : 500
flow.Add(new ColorFlowRGBExpression(0, 0, 255, 50, 500)); // color : blue / brightness : 50% / duration : 500
flow.Add(new ColorFlowSleepExpression(2000)); // sleeps for 2 seconds
flow.Add(new ColorFlowTemperatureExpression(2700, 100, 500)); // color temperature : 2700k / brightness : 100 / duration : 500
flow.Add(new ColorFlowTemperatureExpression(5000, 1, 500)); // color temperature : 5000k / brightness : 100 / duration : 500

device.StartColorFlow(flow); // start

/* Do Some amazing stuff ... */

device.StopColorFlow(); // stop the color flow
```

The `ColorFlow` constructor has 2 parameters : the first one defines the number of repetitions (or 0 for infinite), the second one defines what to do when the flow is stopped. you can choose to restore to the previous state, keep the last state or turn off the device.

#### Fluent
Another way to create color flow is to use the `device.Flow()` method. This method returns a `FluentFLow` object you can use to create a flow in a "Fluent-syntax" way.
example : 
```csharp
FluentFlow flow = await backgroundDevice.BackgroundFlow()
  .RgbColor(255, 0, 0, 50, 1000)
  .Sleep(2000)
  .RgbColor(0, 255, 0, 50) //without timing
  .During(1000) // set the timing of the previous instruction
  .Sleep(2000)
  .RgbColor(0, 0, 255, 50, 1000)
  .Sleep(2000)
  .Temperature(2700, 100, 1000)
  .Sleep(2000)
  .Temperature(6500, 100, 1000)
  .Play(ColorFlowEndAction.Keep);

await flow.StopAfter(5000);

//use the same object to create a new flow
await flow.Reset()
  .RgbColor(0, 255, 0, 50, 1000)
  .Temperature(3000, 100, 1000)
  .Play(ColorFlowEndAction.Keep);
```

### Find devices
If you want to find what devices are connected, you can use `YeelightAPI.DeviceLocator` to find them: 
```csharp
List<Device> discoveredDevices = await DeviceLocator.DiscoverAsync();
```

### Async / Await
Almost every methods are asynchronous and are awaitable tasks. you can either call them with await, or wait the result : 
Example : 
```csharp
// with single device
await device.Connect();
device.Toggle().Result;

//with groups
group.Connect().Result;
await group.Toggle();
...
```

## Events
### Notifications
When you call a method that changes the state of the device, it sends a notification to inform that its state really change. You can receive these notification using the "OnNotificationReceived" event.
Example : 
```csharp
device.OnNotificationReceived += (object sender, NotificationReceivedEventArgs arg) =>
{
  Console.WriteLine("Notification received !! value : " + JsonConvert.SerializeObject(arg.Result));
};
```

### Errors
When an unknown error occurs, a "OnError" event is fired.
Example : 
```csharp
device.OnError += (object sender, UnhandledExceptionEventArgs e) =>
{
  Console.WriteLine($"An error occurred : {e.ExceptionObject}");
};
```

### Device Found
If you don't want to wait until all devices are discovered, you can make use of the [`IProgress<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.iprogress-1?view=netframework-4.7), to receive intermediate results.
Create instance of [`Progress<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.progress-1?view=netframework-4.7) and pass to the appropriate DiscoverAsync overload. The callback will always execute oin the caller thread.
When DeviceLocator.DiscoverAsync(Progress<T>) finds a device, the `Progress<T>.Report` method is invoked.
Example : 
```csharp
private void OnDeviceFound(Device device) 
{
  // Do Something with the discovered device   
}
	
private	async Task GetDevicesAsync()
{
  var progressReporter = new Progress<Device>(OnDeviceFound);
  List<Devices> discoveredDevices = await DeviceLocator.DiscoverAsync(progresReporter);
}
```

## VNext
* correct bugs if needed

Nothing else planned, if you have any ideas please contact me at ["romain.oddone.github@outlook.com"](mailto:romain.oddone.github@outlook.com) 

## Help
If there is a functionality that you need which is not implemented, or even worse if there is a bug, you can create a pull request or contacts me at ["romain.oddone.github@outlook.com"](mailto:romain.oddone.github@outlook.com)

## Licence

Apache Licence

## Source
This code is an implementation of the ["Yeelight WiFi Light Inter-Operation Specification"](http://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf "Link to Yeelight WiFi Light Inter-Operation Specification") as defined on January 1st, 2018
