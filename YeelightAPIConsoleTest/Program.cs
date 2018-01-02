using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI;
using YeelightClient.Models;

namespace YeelightAPIConsoleTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                DeviceManager manager = new DeviceManager();
                manager.Connect("192.168.0.16");
                manager.NotificationReceived += (object sender, NotificationReceivedEventArgs arg) =>
                {
                    Console.WriteLine("Notification received !! value : " + JsonConvert.SerializeObject(arg.Result));
                };

                Console.WriteLine("powering on ...");
                manager.SetPower(true);
                await Task.Delay(3000);

                Console.WriteLine("getting all props ...");
                Dictionary<string, object> result = manager.GetAllProps();
                Console.WriteLine("\tprops : " + JsonConvert.SerializeObject(result));

                Console.WriteLine("Setting Brightness to One...");
                manager.SetBrightness(01);

                await Task.Delay(3000);
                Console.WriteLine("Setting Brightness to 100 %...");
                manager.SetBrightness(100, 500);
                await Task.Delay(3000);

                Console.WriteLine("Setting Brightness to 50 %...");
                manager.SetBrightness(50, 500);
                await Task.Delay(3000);

                Console.WriteLine("Setting Brightness to red ...");
                manager.SetRGBColor(255, 0, 0, 500);
                await Task.Delay(3000);

                Console.WriteLine("Setting Brightness to green...");
                manager.SetRGBColor(0, 255, 0, 500);
                await Task.Delay(3000);

                Console.WriteLine("Setting Brightness to blue...");
                manager.SetRGBColor(0, 0, 255, 500);
                await Task.Delay(3000);

                Console.WriteLine("Toggling bulb state...");
                manager.Toggle();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred : {ex.Message}");
            }

            Console.WriteLine("Press Enter to continue ;)");
            Console.ReadLine();
        }
    }
}
