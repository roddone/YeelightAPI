# YeelightAPI
C# API (.Net) to control Xiaomi Yeelight Color Bulbs

## Prerequisites
The console project uses C# 7.1 "Async Main Method" Feature, make sure your visual studio version is up to date !

## Usage
The `YeelightAPI.DeviceManager` allows you to connect to a bulb. Just instanciate the manager : ` DeviceManager manager = new DeviceManager();` and initiate connection to your bulb IP adress : `manager.Connect("XXX.XXX.XXX.XXX");`.
Then you can use the manager to control the bulb : 
* Power on / off : `manager.SetPower(true);`
* Toggle State : `manager.Toggle();`
* Change brightness level : `manager.SetBrightness(100);`
* Change color : `manager.SetRGBColor(80, 244, 255);`

Some methods use an optional parameter named "smooth", it refers to the duration in milliseconds of the effect you want to apply. For a progressive brightness change, use `manager.SetBrightness(100, 3000);`.

If you need a method that is not implemented, you can use the folowing methods :
* `ExecuteCommandWithResponse(METHODS method, int id = 0, List<object> parameters = null)` (with response) 
* `ExecuteCommand(METHODS method, int id = 0, List<object> parameters = null)` (without response).

These methods are generic and use the `METHODS` enumeration and a list of parameters, which allows you to call any known method with any parameter.

## Event
When you call a method that changes the state of the bulb, it sends a notification to inform that its state really change. You can receive these notification using the "NotificationReceived" 
Example : 
```csharp
   manager.NotificationReceived += (object sender, NotificationReceivedEventArgs arg) =>
   {
       Console.WriteLine("Notification received !! value : " + JsonConvert.SerializeObject(arg.Result));
   };
```

## VNext
* ~~turn into a nuget package~~
* ~~handle dns host name for bulb discovery~~
* add device discovery method in the DeviceManager
* add more native methods in the DeviceManager
* correct bugs if needed

## Help
If there is a functionality that you need which is not implemented, or even worse if there is a bug, you can create a pull request.

## Licence

Apache Licence

## Source
This code is an implementation of the ["Yeelight WiFi Light Inter-Operation Specification"](http://www.yeelight.com/download/Yeelight_Inter-Operation_Spec.pdf "Link to Yeelight WiFi Light Inter-Operation Specification") as defined on January 1st, 2018
