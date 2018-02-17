using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YeelightAPI;
using YeelightAPI.Models;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;

namespace YeelightAPIConsoleTest
{
    public class Program
    {
        #region Public Methods

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
                        bool success = true;

                        Console.WriteLine("connecting device ...");
                        success &= await device.Connect();

                        device.OnNotificationReceived += Device_OnNotificationReceived;

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

        #endregion Public Methods

        #region Private Methods

        private static void Device_OnNotificationReceived(object sender, NotificationReceivedEventArgs arg)
        {
            WriteLineWithColor($"Notification received !! value : {JsonConvert.SerializeObject(arg.Result)}", ConsoleColor.DarkGray);
        }

        private static async Task<bool> ExecuteTests(IDeviceController device, int? smooth = null)
        {
            bool success = true, globalSuccess = true;
            int delay = 1500;

            Console.WriteLine("powering on ...");
            success = await device.SetPower(true);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("add cron ...");
            success = await device.CronAdd(15, YeelightAPI.Models.Cron.CronType.PowerOff);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            if (device is IDeviceReader deviceReader)
            {
                Console.WriteLine("get cron ...");
                CronResult cronResult = await deviceReader.CronGet(YeelightAPI.Models.Cron.CronType.PowerOff);
                globalSuccess &= (cronResult != null);
                WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
                await Task.Delay(delay);

                Console.WriteLine("getting current name ...");
                string name = (await deviceReader.GetProp(PROPERTIES.name))?.ToString();
                Console.WriteLine($"current name : {name}");

                Console.WriteLine("setting name 'test' ...");
                success &= await deviceReader.SetName("test");
                WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
                await Task.Delay(2000);

                Console.WriteLine("restoring name '{0}' ...", name);
                success &= await deviceReader.SetName(name);
                WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
                await Task.Delay(2000);

                Console.WriteLine("getting all props ...");
                Dictionary<PROPERTIES, object> result = await deviceReader.GetAllProps();
                Console.WriteLine($"\tprops : {JsonConvert.SerializeObject(result)}");
                await Task.Delay(2000);
            }
            Console.WriteLine("delete cron ...");
            success = await device.CronDelete(YeelightAPI.Models.Cron.CronType.PowerOff);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting Brightness to One...");
            success = await device.SetBrightness(01);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting brightness increase...");
            success = await device.SetAdjust(YeelightAPI.Models.Adjust.AdjustAction.increase, YeelightAPI.Models.Adjust.AdjustProperty.bright);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting Brightness to 100 %...");
            success = await device.SetBrightness(100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting brightness decrease...");
            success = await device.SetAdjust(YeelightAPI.Models.Adjust.AdjustAction.decrease, YeelightAPI.Models.Adjust.AdjustProperty.bright);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting Brightness to 50 %...");
            success = await device.SetBrightness(50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting RGB color to red ...");
            success = await device.SetRGBColor(255, 0, 0, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting RGB color to green...");
            success = await device.SetRGBColor(0, 255, 0, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting color increase circle...");
            success = await device.SetAdjust(YeelightAPI.Models.Adjust.AdjustAction.circle, YeelightAPI.Models.Adjust.AdjustProperty.color);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting RGB color to blue...");
            success = await device.SetRGBColor(0, 0, 255, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting HSV color to red...");
            success = await device.SetHSVColor(0, 100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting HSV color to green...");
            success = await device.SetHSVColor(120, 100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting HSV color to blue...");
            success = await device.SetHSVColor(240, 100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting Color Temperature to 1700k ...");
            success = await device.SetColorTemperature(1700, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting color temperature increase ...");
            success = await device.SetAdjust(YeelightAPI.Models.Adjust.AdjustAction.increase, YeelightAPI.Models.Adjust.AdjustProperty.ct);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Setting Color Temperature to 6500k ...");
            success = await device.SetColorTemperature(6500, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Starting color flow ...");
            int repeat = 0;
            ColorFlow flow = new ColorFlow(repeat, ColorFlowEndAction.Restore)
            {
                new ColorFlowRGBExpression(255, 0, 0, 1, 500),
                new ColorFlowRGBExpression(0, 255, 0, 100, 500),
                new ColorFlowRGBExpression(0, 0, 255, 50, 500),
                new ColorFlowSleepExpression(2000),
                new ColorFlowTemperatureExpression(2700, 100, 500),
                new ColorFlowTemperatureExpression(5000, 1, 500)
            };
            success = await device.StartColorFlow(flow);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(10 * 1000);

            Console.WriteLine("Stoping color flow ...");
            success = await device.StopColorFlow();
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            Console.WriteLine("Toggling bulb state...");
            success = await device.Toggle();
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);

            if (success)
            {
                WriteLineWithColor($"Tests are successful", ConsoleColor.DarkGreen);
            }
            else
            {
                WriteLineWithColor($"Tests failed", ConsoleColor.DarkRed);
            }

            return globalSuccess;
        }

        private static void WriteLineWithColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        #endregion Private Methods
    }
}