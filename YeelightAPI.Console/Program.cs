using System.Text.Json;
using YeelightAPI;
using YeelightAPI.Models;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;

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
        Progress<Device> reporter = new(d =>
        {
            WriteLineWithColor($"Device found : {d}", ConsoleColor.Blue);
        });
        List<Device> devices = (await DeviceLocator.DiscoverAsync(reporter)).ToList();

        if (devices != null && devices.Count >= 1)
        {
            Console.WriteLine($"{devices.Count} device(s) found !");
            using DeviceGroup group = new(devices);
            await group.ConnectAsync();

            foreach (Device device in group)
            {
                device.OnNotificationReceived += Device_OnNotificationReceived;
                device.OnError += Device_OnError;
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
        else
        {
            WriteLineWithColor("No devices Found via SSDP !", ConsoleColor.Red);
        }
    }
    else
    {
        Console.Write("Give a hostname or IP adress to connect to the device : ");
        string hostname = Console.ReadLine()!;
        Console.WriteLine();
        Console.Write("Give a port number (or leave empty to use default port) : ");
        Console.WriteLine();

        if (!int.TryParse(Console.ReadLine(), out int port))
        {
            port = 55443;
        }

        using Device device = new(hostname, port);
        bool success = true;

        Console.WriteLine("connecting device ...");
        success &= await device.ConnectAsync();

        device.OnNotificationReceived += Device_OnNotificationReceived;
        device.OnError += Device_OnError;

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
catch (Exception ex)
{
    WriteLineWithColor($"An error has occurred : {ex.Message}", ConsoleColor.Red);
}

Console.WriteLine("Press Enter to continue ;)");
Console.ReadLine();

void Device_OnError(object sender, UnhandledExceptionEventArgs e)
{
    WriteLineWithColor($"An Error ocurred !! {e.ExceptionObject}", ConsoleColor.Red);
}

void Device_OnNotificationReceived(object sender, NotificationReceivedEventArgs arg)
{
    WriteLineWithColor($"Notification received !! value : {JsonSerializer.Serialize(arg.Result)}", ConsoleColor.DarkGray);
}

async Task<bool> ExecuteTests(IDeviceController device, int? smooth = null)
{
    bool success = true, globalSuccess = true;
    int delay = 1500;

    await Try(async () =>
    {
        Console.WriteLine("powering on ...");
        success = await device.SetPowerAsync(true, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("turn off ...");
        success = await device.TurnOffAsync(smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("turn on ...");
        success = await device.TurnOnAsync(smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("add cron ...");
        success = await device.CronAddAsync(15, CronType.PowerOff);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    if (device is IDeviceReader deviceReader)
    {
        await Try(async () =>
        {
            Console.WriteLine("get cron ...");
            CronResult cronResult = await deviceReader.CronGetAsync(CronType.PowerOff);
            globalSuccess &= (cronResult != null);
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        string? name = null;
        await Try(async () =>
        {
            Console.WriteLine("getting current name ...");
            name = (await deviceReader.GetPropAsync(PROPERTIES.name))?.ToString();
            Console.WriteLine($"current name : {name}");
        });
        await Task.Delay(delay);

        await Try(async () =>
        {
            Console.WriteLine("setting name 'test' ...");
            success &= await deviceReader.SetNameAsync("test");
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(2000);
        });
        await Task.Delay(delay);

        await Try(async () =>
        {
            Console.WriteLine("restoring name '{0}' ...", name);
            success &= await deviceReader.SetNameAsync(name);
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(2000);
        });

        await Try(async () =>
        {
            Console.WriteLine("getting all props ...");
            Dictionary<PROPERTIES, object> result = await deviceReader.GetAllPropsAsync();
            Console.WriteLine($"\tprops : {JsonSerializer.Serialize(result)}");
            await Task.Delay(2000);
        });
    }

    await Try(async () =>
    {
        Console.WriteLine("delete cron ...");
        success = await device.CronDeleteAsync(CronType.PowerOff);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    if (device is IBackgroundDeviceController backgroundDevice)
    {
        await Try(async () =>
        {
            Console.WriteLine("powering on ...");
            success = await backgroundDevice.BackgroundSetPowerAsync(true, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("turn off ...");
            success = await backgroundDevice.BackgroundTurnOffAsync(smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("turn on ...");
            success = await backgroundDevice.BackgroundTurnOnAsync(smooth, PowerOnMode.RGB);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting Brightness to One...");
            success = await backgroundDevice.BackgroundSetBrightnessAsync(1, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting brightness increase...");
            success = await backgroundDevice.BackgroundSetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.increase, YeelightAPI.Models.Adjust.AdjustProperty.bright);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting Brightness to 100 %...");
            success = await backgroundDevice.BackgroundSetBrightnessAsync(100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting brightness decrease...");
            success = await backgroundDevice.BackgroundSetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.decrease, YeelightAPI.Models.Adjust.AdjustProperty.bright);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting Brightness to 50 %...");
            success = await backgroundDevice.BackgroundSetBrightnessAsync(50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting RGB color to red ...");
            success = await backgroundDevice.BackgroundSetRGBColorAsync(255, 0, 0, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting RGB color to green...");
            success = await backgroundDevice.BackgroundSetRGBColorAsync(0, 255, 0, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting color increase circle...");
            success = await backgroundDevice.BackgroundSetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.circle, YeelightAPI.Models.Adjust.AdjustProperty.color);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting RGB color to blue...");
            success = await backgroundDevice.BackgroundSetRGBColorAsync(0, 0, 255, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting HSV color to red...");
            success = await backgroundDevice.BackgroundSetHSVColorAsync(0, 100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting HSV color to green...");
            success = await backgroundDevice.BackgroundSetHSVColorAsync(120, 100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting HSV color to blue...");
            success = await backgroundDevice.BackgroundSetHSVColorAsync(240, 100, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting Color Temperature to 1700k ...");
            success = await backgroundDevice.BackgroundSetColorTemperatureAsync(1700, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting color temperature increase ...");
            success = await backgroundDevice.BackgroundSetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.increase, YeelightAPI.Models.Adjust.AdjustProperty.ct);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Setting Color Temperature to 6500k ...");
            success = await backgroundDevice.BackgroundSetColorTemperatureAsync(6500, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Starting color flow ...");
            int repeat = 0;
            ColorFlow flow = new(repeat, ColorFlowEndAction.Restore)
            {
                new ColorFlowRGBExpression(255, 0, 0, 1, 500),
                new ColorFlowRGBExpression(0, 255, 0, 100, 500),
                new ColorFlowRGBExpression(0, 0, 255, 50, 500),
                new ColorFlowSleepExpression(2000),
                new ColorFlowTemperatureExpression(2700, 100, 500),
                new ColorFlowTemperatureExpression(5000, 1, 500)
            };
            success = await backgroundDevice.BackgroundStartColorFlowAsync(flow);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(10 * 1000);
        });

        await Try(async () =>
        {
            Console.WriteLine("Stoping color flow ...");
            success = await backgroundDevice.BackgroundStopColorFlowAsync();
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Starting fluent color flow ...");
            FluentFlow fflow = await backgroundDevice.BackgroundFlow()
                .RgbColor(255, 0, 0, 50, 1000)
                .Sleep(2000)
                .RgbColor(0, 255, 0, 50, 1000)
                .Sleep(2000)
                .RgbColor(0, 0, 255, 50, 1000)
                .Sleep(2000)
                .Temperature(2700, 100, 1000)
                .Sleep(2000)
                .Temperature(6500, 100, 1000)
                .PlayAsync(ColorFlowEndAction.Keep);

            await fflow.StopAfterAsync(5000);

            WriteLineWithColor($"Color flow ended", ConsoleColor.DarkCyan);
        });

        await Try(async () =>
        {
            Console.WriteLine("adjust brightness ++");
            success = await backgroundDevice.BackgroundAdjustBrightAsync(50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("adjust brightness --");
            success = await backgroundDevice.BackgroundAdjustBrightAsync(-50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("adjust color ++");
            success = await backgroundDevice.BackgroundAdjustColorAsync(50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("adjust color --");
            success = await backgroundDevice.BackgroundAdjustColorAsync(-50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("adjust color temperature ++");
            success = await backgroundDevice.BackgroundAdjustColorTemperatureAsync(50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("adjust color temperature --");
            success = await backgroundDevice.BackgroundAdjustColorTemperatureAsync(-50, smooth);
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });

        await Try(async () =>
        {
            Console.WriteLine("Toggling bulb state...");
            success = await backgroundDevice.BackgroundToggleAsync();
            globalSuccess &= success;
            WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
            await Task.Delay(delay);
        });
    }

    await Try(async () =>
    {
        Console.WriteLine("Setting Brightness to One...");
        success = await device.SetBrightnessAsync(1, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting brightness increase...");
        success = await device.SetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.increase, YeelightAPI.Models.Adjust.AdjustProperty.bright);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting Brightness to 100 %...");
        success = await device.SetBrightnessAsync(100, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting brightness decrease...");
        success = await device.SetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.decrease, YeelightAPI.Models.Adjust.AdjustProperty.bright);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting Brightness to 50 %...");
        success = await device.SetBrightnessAsync(50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting RGB color to red ...");
        success = await device.SetRGBColorAsync(255, 0, 0, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting RGB color to green...");
        success = await device.SetRGBColorAsync(0, 255, 0, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting color increase circle...");
        success = await device.SetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.circle, YeelightAPI.Models.Adjust.AdjustProperty.color);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting RGB color to blue...");
        success = await device.SetRGBColorAsync(0, 0, 255, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting HSV color to red...");
        success = await device.SetHSVColorAsync(0, 100, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting HSV color to green...");
        success = await device.SetHSVColorAsync(120, 100, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting HSV color to blue...");
        success = await device.SetHSVColorAsync(240, 100, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting Color Temperature to 1700k ...");
        success = await device.SetColorTemperatureAsync(1700, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting color temperature increase ...");
        success = await device.SetAdjustAsync(YeelightAPI.Models.Adjust.AdjustAction.increase, YeelightAPI.Models.Adjust.AdjustProperty.ct);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Setting Color Temperature to 6500k ...");
        success = await device.SetColorTemperatureAsync(6500, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Starting color flow ...");
        int repeat = 0;
        ColorFlow flow = new(repeat, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 1, 500),
            new ColorFlowRGBExpression(0, 255, 0, 100, 500),
            new ColorFlowRGBExpression(0, 0, 255, 50, 500),
            new ColorFlowSleepExpression(2000),
            new ColorFlowTemperatureExpression(2700, 100, 500),
            new ColorFlowTemperatureExpression(5000, 1, 500)
        };
        success = await device.StartColorFlowAsync(flow);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(10 * 1000);
    });

    await Try(async () =>
    {
        Console.WriteLine("Stoping color flow ...");
        success = await device.StopColorFlowAsync();
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Starting fluent color flow ...");
        FluentFlow fflow = await device.Flow()
            .RgbColor(255, 0, 0, 50, 1000)
            .Sleep(2000)
            .RgbColor(0, 255, 0, 50, 1000)
            .Sleep(2000)
            .RgbColor(0, 0, 255, 50, 1000)
            .Sleep(2000)
            .Temperature(2700, 100, 1000)
            .Sleep(2000)
            .Temperature(6500, 100, 1000)
            .PlayAsync(ColorFlowEndAction.Keep);

        await fflow.StopAfterAsync(5000);

        WriteLineWithColor($"Color flow ended", ConsoleColor.DarkCyan);
    });

    await Try(async () =>
    {
        Console.WriteLine("adjust brightness ++");
        success = await device.AdjustBrightAsync(50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("adjust brightness --");
        success = await device.AdjustBrightAsync(-50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("adjust color ++");
        success = await device.AdjustColorAsync(50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("adjust color --");
        success = await device.AdjustColorAsync(-50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("adjust color temperature ++");
        success = await device.AdjustColorTemperatureAsync(50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("adjust color temperature --");
        success = await device.AdjustColorTemperatureAsync(-50, smooth);
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

    await Try(async () =>
    {
        Console.WriteLine("Toggling bulb state...");
        success = await device.ToggleAsync();
        globalSuccess &= success;
        WriteLineWithColor($"command success : {success}", ConsoleColor.DarkCyan);
        await Task.Delay(delay);
    });

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

async Task Try(Func<Task> action)
{
    try
    {
        await action();
    }
    catch (Exception ex)
    {
        WriteLineWithColor(ex.Message, ConsoleColor.Magenta);
    }
}

void WriteLineWithColor(string text, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(text);
    Console.ResetColor();
}
