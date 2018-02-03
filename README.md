# YeelightAPI
C# API (.Net) to control Xiaomi Yeelight Color Bulbs

## Prerequisites
The console project uses C# 7.1 "Async Main Method" Feature, make sure your visual studio version is up to date !

## Installation
To install the latest release from [NuGet package manager](https://www.nuget.org/packages/YeelightAPI/):

    Install-Package YeelightAPI

## Contribution
If you find this package useful, please make a gift on Paypal : [https://www.paypal.me/roddone](https://www.paypal.me/roddone)

## Usage
### Single Device
The `YeelightAPI.Device` allows you to create a device. Just instanciate a new Device with an ip adress or a hostname: ` Device device = new Device("hos.tna.meo.rIP");` and initiate connection : `device.Connect();`.
Then you can use the device object to control the device : 
* Power on / off : `device.SetPower(true);`
* Toggle State : `device.Toggle();`
* Change brightness level : `device.SetBrightness(100);`
* Change color : `device.SetRGBColor(80, 244, 255);`

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

### Find devices
If you want to find what devices are connected, you can use `YeelightAPI.DeviceLocator` to find them : 
```csharp
	List<Device> devices = await DeviceLocator.Discover();
```

### Async / Await
If you have to deal with asynchronism, the API fully supports async / await. The methos are the same, but they have a "Async" suffix.
Example : 
```csharp
	// with single device
	await device.Connect();
	await device.Toggle();

	//with groups
	await group.Connect();
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
When a method returns an error, it fires a "OnCommandError" event.
Example : 
```csharp
   device.OnCommandError += (object sender, CommandErrorEventArgs arg) =>
   {
       Console.WriteLine($"An error occurred : {arg.Error}");
   };
```

## VNext
* ~~turn into a nuget package~~
* ~~handle dns host name for bulb discovery~~
* ~~add device discovery method in the DeviceManager~~
* add more native methods in the DeviceManager
* ~~full use of async / await~~
* ~~allow to group devices to control multiple devices~~
* correct bugs if needed

## Help
If there is a functionality that you need which is not implemented, or even worse if there is a bug, you can create a pull request or contacts me at ["romain.oddone.github@outlook.com"](mailto:romain.oddone.github@outlook.com)

## Licence

Apache Licence

## Source
This code is an implementation of the ["Yeelight WiFi Light Inter-Operation Specification"](http://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf "Link to Yeelight WiFi Light Inter-Operation Specification") as defined on January 1st, 2018
