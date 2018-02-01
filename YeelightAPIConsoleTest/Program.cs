using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI;
using YeelightAPI.Models;

namespace YeelightAPIConsoleTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Choose a test mode, type 'd' for discovery mode, 's' for a static IP adress : ");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.WriteLine();

                while (keyInfo.Key != ConsoleKey.D && keyInfo.Key != ConsoleKey.S)
                {
                    Console.WriteLine($"'{keyInfo.KeyChar}' is not a valid key !");
                    Console.WriteLine("Choose a test mode, type 'd' for discovery mode, 's' for a static IP adress : ");
                    keyInfo = Console.ReadKey();
                    Console.WriteLine();
                }

                if (keyInfo.Key == ConsoleKey.D)
                {
                    List<Device> devices = await DeviceLocator.Discover();

                    if (devices != null && devices.Count >= 1)
                    {
                        Console.WriteLine($"{devices.Count} device(s) found !");
                        using (DeviceGroup group = new DeviceGroup(devices))
                        {

                            group.Connect();

                            foreach (Device device in group)
                            {
                                device.NotificationReceived += Device_OnNotificationReceived;
                                device.OnCommandError += Device_OnCommandError;
                            }

                            //without smooth value (sudden)
                            WriteLineWithColor("Processing tests", ConsoleColor.Cyan);
                            await ExecuteTests(group, null);

                            //with smooth value
                            WriteLineWithColor("Processing tests with smooth effect", ConsoleColor.Cyan);
                            await ExecuteTests(group, 1000);

                            //with smooth value
                            WriteLineWithColor("Processing async tests", ConsoleColor.Cyan);
                            await ExecuteAsyncTests(group, null);

                            //without smooth value (sudden)
                            WriteLineWithColor("Processing async tests with smooth effect", ConsoleColor.Cyan);
                            await ExecuteAsyncTests(group, 1000);
                        }
                    }
                    else
                    {
                        WriteLineWithColor("No devices Found via SSDP !", ConsoleColor.Red);
                        return;
                    }
                }
                else
                {
                    string hostname;
                    Console.Write("Give a hostname or IP adress to connect to the device : ");
                    hostname = Console.ReadLine();
                    Console.WriteLine();
                    Console.Write("Give a port number (or leave empty to use default port) : ");
                    Console.WriteLine();

                    if (!int.TryParse(Console.ReadLine(), out int port))
                    {
                        port = 55443;
                    }

                    using (Device device = new Device(hostname, port))
                    {
                        device.Connect();
                        device.NotificationReceived += Device_OnNotificationReceived;
                        device.OnCommandError += Device_OnCommandError;

                        Console.WriteLine("getting all props ...");
                        Dictionary<PROPERTIES, object> result = device.GetAllProps();
                        Console.WriteLine("\tprops : " + JsonConvert.SerializeObject(result));
                        await Task.Delay(2000);

                        //without smooth value (sudden)
                        WriteLineWithColor("Processing tests", ConsoleColor.Cyan);
                        await ExecuteTests(device, null);

                        //with smooth value
                        WriteLineWithColor("Processing tests with smooth effect", ConsoleColor.Cyan);
                        await ExecuteTests(device, 1000);

                        //with smooth value
                        WriteLineWithColor("Processing async tests", ConsoleColor.Cyan);
                        await ExecuteAsyncTests(device, 1000);

                        //without smooth value (sudden)
                        WriteLineWithColor("Processing async tests with smooth effect", ConsoleColor.Cyan);
                        await ExecuteAsyncTests(device, null);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLineWithColor($"An error has occurred : {ex.Message}", ConsoleColor.Red);
            }

            Console.WriteLine("Press Enter to continue ;)");
            Console.ReadLine();
        }

        private static void Device_OnCommandError(object sender, ErrorEventArgs arg)
        {
            WriteLineWithColor("An error occurred : " + JsonConvert.SerializeObject(arg.Error), ConsoleColor.Red);
        }

        private static void Device_OnNotificationReceived(object sender, NotificationReceivedEventArgs arg)
        {
            WriteLineWithColor("Notification received !! value : " + JsonConvert.SerializeObject(arg.Result), ConsoleColor.Yellow);
        }

        private static async Task ExecuteAsyncTests(IDeviceController device, int? smooth = null)
        {
            Console.WriteLine("powering on ...");
            await device.SetPowerAsync(true);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to One...");
            await device.SetBrightnessAsync(01);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to 100 %...");
            await device.SetBrightnessAsync(100, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to 50 %...");
            await device.SetBrightnessAsync(50, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to red ...");
            await device.SetRGBColorAsync(255, 0, 0, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to green...");
            await device.SetRGBColorAsync(0, 255, 0, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to blue...");
            await device.SetRGBColorAsync(0, 0, 255, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Color Saturation to 1700k ...");
            await device.SetColorTemperatureAsync(1700, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Color Saturation to 6500k ...");
            await device.SetColorTemperatureAsync(6500, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Toggling bulb state...");
            await device.ToggleAsync();
            await Task.Delay(2000);
        }

        private static async Task ExecuteTests(IDeviceController device, int? smooth = null)
        {
            Console.WriteLine("powering on ...");
            device.SetPower(true);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to One...");
            device.SetBrightness(01);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to 100 %...");
            device.SetBrightness(100, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to 50 %...");
            device.SetBrightness(50, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to red ...");
            device.SetRGBColor(255, 0, 0, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to green...");
            device.SetRGBColor(0, 255, 0, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Brightness to blue...");
            device.SetRGBColor(0, 0, 255, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Color Saturation to 1700k ...");
            device.SetColorTemperature(1700, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Setting Color Saturation to 6500k ...");
            device.SetColorTemperature(6500, smooth);
            await Task.Delay(2000);

            Console.WriteLine("Toggling bulb state...");
            device.Toggle();
            await Task.Delay(2000);
        }

        private static void WriteLineWithColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
