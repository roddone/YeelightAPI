using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI;

namespace YeelightAPIConsoleTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Console.Write("Choose a test mode, type 'd' for discovery mode, 's' for a static IP adress : ");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                Console.WriteLine();

                while (keyInfo.Key != ConsoleKey.D && keyInfo.Key != ConsoleKey.S)
                {
                    Console.WriteLine($"'{keyInfo.KeyChar}' is not a valid key !");
                    Console.Write("Choose a test mode, type 'd' for discovery mode, 's' for a static IP adress : ");
                    keyInfo = Console.ReadKey();
                    Console.WriteLine();
                }

                if (keyInfo.Key == ConsoleKey.D)
                {
                    List<Device> devices = await DeviceLocator.Discover();

                    if (devices != null && devices.Count >= 1)
                    {
                        Console.WriteLine($"{devices.Count} found !");

                        Parallel.ForEach(devices, async device =>
                        {
                            device.Connect();
                            device.NotificationReceived += OnNotificationReceived;

                            await ExecuteTests(device, null);

                            await ExecuteTests(device, 1000);
                        });
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No devices Found via SSDP !");
                        return;
                    }
                }
                else
                {
                    int port;
                    string hostname;
                    Console.Write("Give a hostname or IP adress to connect to the device : ");
                    hostname = Console.ReadLine();
                    Console.WriteLine();
                    Console.Write("Give a port number (or leave empty to use default port) : ");
                    Console.WriteLine();

                    if (!int.TryParse(Console.ReadLine(), out port))
                    {
                        port = 55443;
                    }

                    Device manager = new Device(hostname, port);
                    manager.Connect();
                    manager.NotificationReceived += OnNotificationReceived;

                    //with smooth value
                    await ExecuteTests(manager, 1000);

                    //without smooth value (sudden)
                    await ExecuteTests(manager, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred : {ex.Message}");
            }

            Console.WriteLine("Press Enter to continue ;)");
            Console.ReadLine();
        }

        private static void OnNotificationReceived(object sender, NotificationReceivedEventArgs arg)
        {
            Console.WriteLine("Notification received !! value : " + JsonConvert.SerializeObject(arg.Result.Params));
        }

        private static async Task ExecuteTests(Device device, int? smooth = null)
        {
            Console.WriteLine("powering on ...");
            device.SetPower(true);
            await Task.Delay(2000);

            Console.WriteLine("getting all props ...");
            Dictionary<string, object> result = device.GetAllProps();
            Console.WriteLine("\tprops : " + JsonConvert.SerializeObject(result));
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
    }
}
