using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI;
using YeelightAPI.Models;
using YeelightAPI.Models.ColorFlow;

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

                            await group.Connect();

                            foreach (Device device in group)
                            {
                                device.OnNotificationReceived += Device_OnNotificationReceived;
                                device.OnCommandError += Device_OnCommandError;
                            }

                            bool success = true;

                            //without smooth value (sudden)
                            WriteLineWithColor("Processing tests", ConsoleColor.Cyan);
                            success &= await ExecuteTests(group, null);

                            //with smooth value
                            WriteLineWithColor("Processing tests with smooth effect", ConsoleColor.Cyan);
                            success &= await ExecuteTests(group, 1000);

                            if (success)
                            {
                                WriteLineWithColor("All Tests are successfull", ConsoleColor.Green);
                            }
                            else
                            {
                                WriteLineWithColor("Some tests have failed", ConsoleColor.Red);
                            }
                        }
                    }
                    else
                    {
                        WriteLineWithColor("No devices Found via SSDP !", ConsoleColor.Red);
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
                        await device.Connect();
                        device.OnNotificationReceived += Device_OnNotificationReceived;
                        device.OnCommandError += Device_OnCommandError;

                        Console.WriteLine("getting all props synchronously...");
                        Dictionary<PROPERTIES, object> result = await device.GetAllProps();
                        Console.WriteLine("\tprops : " + JsonConvert.SerializeObject(result));
                        await Task.Delay(2000);

                        Console.WriteLine("getting all props asynchronously...");
                        result = await device.GetAllProps();
                        Console.WriteLine("\tprops : " + JsonConvert.SerializeObject(result));
                        await Task.Delay(2000);

                        bool success = true;

                        //without smooth value (sudden)
                        WriteLineWithColor("Processing tests", ConsoleColor.Cyan);
                        success &= await ExecuteTests(device, null);

                        //with smooth value
                        WriteLineWithColor("Processing tests with smooth effect", ConsoleColor.Cyan);
                        success &= await ExecuteTests(device, 1000);

                        if (success)
                        {
                            WriteLineWithColor("All Tests are successfull", ConsoleColor.Green);
                        }
                        else
                        {
                            WriteLineWithColor("Some tests have failed", ConsoleColor.Red);
                        }
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

        private static void Device_OnCommandError(object sender, CommandErrorEventArgs arg)
        {
            WriteLineWithColor($"An error occurred : {arg.Error}", ConsoleColor.DarkRed);
        }

        private static void Device_OnNotificationReceived(object sender, NotificationReceivedEventArgs arg)
        {
            WriteLineWithColor("Notification received !! value : " + JsonConvert.SerializeObject(arg.Result), ConsoleColor.DarkGray);
        }


        private static async Task<bool> ExecuteTests(IDeviceController device, int? smooth = null)
        {
            bool success = true;
            int delay = 1500;

            Console.WriteLine("powering on ...");
            success &= await device.SetPower(true);
            await Task.Delay(delay);

            Console.WriteLine("Setting Brightness to One...");
            success &= await device.SetBrightness(01);
            await Task.Delay(delay);

            Console.WriteLine("Setting Brightness to 100 %...");
            success &= await device.SetBrightness(100, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting Brightness to 50 %...");
            success &= await device.SetBrightness(50, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting RGB color to red ...");
            success &= await device.SetRGBColor(255, 0, 0, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting RGB color to green...");
            success &= await device.SetRGBColor(0, 255, 0, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting RGB color to blue...");
            success &= await device.SetRGBColor(0, 0, 255, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting HSV color to red...");
            success &= await device.SetHSVColor(0, 100, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting HSV color to green...");
            success &= await device.SetHSVColor(120, 100, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting HSV color to blue...");
            success &= await device.SetHSVColor(240, 100, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting Color Saturation to 1700k ...");
            success &= await device.SetColorTemperature(1700, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Setting Color Saturation to 6500k ...");
            success &= await device.SetColorTemperature(6500, smooth);
            await Task.Delay(delay);

            Console.WriteLine("Starting color flow ...");
            int repeat = 0;
            ColorFlow flow = new ColorFlow(repeat, ColorFlowEndAction.Restore);
            flow.Add(new ColorFlowRGBExpression(255, 0, 0, 1, 500));
            flow.Add(new ColorFlowRGBExpression(0, 255, 0, 100, 500));
            flow.Add(new ColorFlowRGBExpression(0, 0, 255, 50, 500));
            flow.Add(new ColorFlowSleepExpression(2000));
            flow.Add(new ColorFlowTemperatureExpression(2700, 100, 500));
            flow.Add(new ColorFlowTemperatureExpression(5000, 1, 500));
            success &= await device.StartColorFlow(flow);
            await Task.Delay(10 * 1000);

            Console.WriteLine("Stoping color flow ...");
            success &= await device.StopColorFlow();
            await Task.Delay(delay);

            Console.WriteLine("Toggling bulb state...");
            success &= await device.Toggle();
            await Task.Delay(delay);

            if (success)
            {
                WriteLineWithColor($"Tests are successful", ConsoleColor.DarkGreen);
            }
            else
            {
                WriteLineWithColor($"Tests failed", ConsoleColor.DarkRed);
            }

            return success;
        }

        private static void WriteLineWithColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
