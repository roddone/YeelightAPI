using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;
using YeelightAPI.Models.Scene;

namespace YeelightAPI.UnitTests;

/// <summary>
/// Integration tests for DeviceGroup that require actual Yeelight devices on the network.
/// </summary>
[NotInParallel("Integration Tests")]
[Retry(3)] // Retry on quota exceeded errors
public class DeviceGroupIntegrationTests
{
    #region Power & Toggle Tests

    [Test]
    public async Task DeviceGroup_TurnOnOff_VerifyAllDevicesState()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();
        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices found");
            return;
        }

        using var group = new DeviceGroup(devices, "Test Group");
        await group.Connect();

        var result = await group.TurnOn();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices are on
        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");
        }

        result = await group.TurnOff();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices are off
        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("off");
        }
    }

    [Test]
    public async Task DeviceGroup_Toggle_VerifyAllDevicesStateChange()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();
        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();

        // Set known state (all on)
        await group.TurnOn();
        await Task.Delay(500);

        // Toggle to off
        var result = await group.Toggle();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("off");
        }

        // Toggle back to on
        result = await group.Toggle();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");
        }

        await group.TurnOff();
    }

    [Test]
    public async Task DeviceGroup_SetPower_VerifyAllDevicesState()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();
        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();

        var result = await group.SetPower(true, smooth: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");
        }

        result = await group.SetPower(false, smooth: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("off");
        }
    }

    #endregion

    #region Brightness Tests

    [Test]
    public async Task DeviceGroup_SetBrightness_VerifyAllDevicesValue()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.SetBrightness))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with brightness support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        var result = await group.SetBrightness(65);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var brightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
            await Assert.That(brightness).IsEqualTo(65);
        }

        await group.TurnOff();
    }

    [Test]
    public async Task DeviceGroup_AdjustBright_VerifyAllDevicesChange()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.AdjustBright))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with AdjustBright support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await group.SetBrightness(50);
        await Task.Delay(500);

        // Get initial brightness for each device
        var initialBrightness = new Dictionary<Device, int>();
        foreach (var device in group)
        {
            initialBrightness[device] = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        }

        var result = await group.AdjustBright(20);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var newBrightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
            await Assert.That(newBrightness > initialBrightness[device]).IsTrue();
        }

        await group.TurnOff();
    }

    [Test]
    public async Task DeviceGroup_SetAdjust_IncreaseBright_VerifyAllDevicesChange()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.SetAdjust))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with SetAdjust support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await group.SetBrightness(40);
        await Task.Delay(500);

        var initialBrightness = new Dictionary<Device, int>();
        foreach (var device in group)
        {
            initialBrightness[device] = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        }

        var result = await group.SetAdjust(AdjustAction.increase, AdjustProperty.bright);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var newBrightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
            await Assert.That(newBrightness > initialBrightness[device]).IsTrue();
        }

        await group.TurnOff();
    }

    #endregion

    #region Color Temperature Tests

    [Test]
    public async Task DeviceGroup_SetColorTemperature_VerifyAllDevicesValue()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.SetColorTemperature))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No CT devices found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        var result = await group.SetColorTemperature(3500);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var ct = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));
            await Assert.That(ct).IsEqualTo(3500);
        }

        await group.TurnOff();
    }

    [Test]
    public async Task DeviceGroup_AdjustColorTemperature_VerifyAllDevicesChange()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.AdjustColorTemperature))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with AdjustColorTemperature support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await group.SetColorTemperature(3000);
        await Task.Delay(500);

        var initialCt = new Dictionary<Device, int>();
        foreach (var device in group)
        {
            initialCt[device] = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));
        }

        var result = await group.AdjustColorTemperature(30);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var newCt = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));
            await Assert.That(newCt > initialCt[device]).IsTrue();
        }

        await group.TurnOff();
    }

    #endregion

    #region Color Tests

    [Test]
    public async Task DeviceGroup_SetRGBColor_VerifyAllDevicesValue()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.SetRGBColor))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No RGB devices found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        var result = await group.SetRGBColor(128, 0, 255); // Purple
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        int expectedRgb = (128 << 16) | (0 << 8) | 255;
        foreach (var device in group)
        {
            var rgb = Convert.ToInt32(await device.GetProp(PROPERTIES.rgb));
            await Assert.That(rgb).IsEqualTo(expectedRgb);
        }

        await group.TurnOff();
    }

    [Test]
    public async Task DeviceGroup_SetHSVColor_VerifyAllDevicesValue()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.SetHSVColor))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No HSV devices found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        var result = await group.SetHSVColor(180, 80); // Cyan
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var hue = Convert.ToInt32(await device.GetProp(PROPERTIES.hue));
            var sat = Convert.ToInt32(await device.GetProp(PROPERTIES.sat));
            await Assert.That(hue).IsEqualTo(180);
            await Assert.That(sat).IsEqualTo(80);
        }

        await group.TurnOff();
    }

    #endregion

    #region ColorFlow Tests

    [Test]
    public async Task DeviceGroup_StartStopColorFlow_VerifyAllDevicesFlowing()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.StartColorFlow))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with ColorFlow support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        var flow = new ColorFlow(0, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowRGBExpression(0, 255, 0, 100, 500),
            new ColorFlowRGBExpression(0, 0, 255, 100, 500)
        };

        var result = await group.StartColorFlow(flow);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices are flowing
        foreach (var device in group)
        {
            var flowingState = await device.GetProp(PROPERTIES.flowing);
            await Assert.That(flowingState?.ToString()).IsEqualTo("1");
        }

        result = await group.StopColorFlow();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        // Verify all devices stopped flowing
        foreach (var device in group)
        {
            var flowingState = await device.GetProp(PROPERTIES.flowing);
            await Assert.That(flowingState?.ToString()).IsEqualTo("0");
        }

        await group.TurnOff();
    }

    [Test]
    public async Task DeviceGroup_FluentFlow_VerifyExecution()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.StartColorFlow))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with ColorFlow support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        var flow = group.Flow()
            .RgbColor(255, 0, 0, 100, 300)
            .RgbColor(0, 255, 0, 100, 300)
            .Temperature(4000, 100, 300);

        await flow.Play(ColorFlowEndAction.Restore, repetition: 1);
        await Task.Delay(300);

        // Verify all devices are flowing
        foreach (var device in group)
        {
            var flowingState = await device.GetProp(PROPERTIES.flowing);
            await Assert.That(flowingState?.ToString()).IsEqualTo("1");
        }

        await group.StopColorFlow();
        await group.TurnOff();
    }

    #endregion

    #region Scene Tests

    [Test]
    public async Task DeviceGroup_SetScene_RGBColor_VerifyAllDevicesState()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.SetScene))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with Scene support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();

        var scene = Scene.FromRGBColor(0, 200, 100, 70); // Greenish at 70%
        var result = await group.SetScene(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        int expectedRgb = (0 << 16) | (200 << 8) | 100;
        foreach (var device in group)
        {
            var powerState = await device.GetProp(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");

            var rgb = Convert.ToInt32(await device.GetProp(PROPERTIES.rgb));
            await Assert.That(rgb).IsEqualTo(expectedRgb);

            var brightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
            await Assert.That(brightness).IsEqualTo(70);
        }

        await group.TurnOff();
    }

    #endregion

    #region Cron Tests

    [Test]
    public async Task DeviceGroup_CronAddDelete_VerifyAllDevices()
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => d.SupportedOperations.Contains(METHODS.AddCron))
            .ToList();

        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices with Cron support found");
            return;
        }

        using var group = new DeviceGroup(devices);
        await group.Connect();
        await group.TurnOn();
        await Task.Delay(300);

        // Add cron
        var result = await group.CronAdd(5, CronType.PowerOff);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices have delayoff set
        foreach (var device in group)
        {
            var delayoff = Convert.ToInt32(await device.GetProp(PROPERTIES.delayoff));
            await Assert.That(delayoff > 0 && delayoff <= 5).IsTrue();
        }

        // Delete cron
        result = await group.CronDelete(CronType.PowerOff);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        // Verify all devices have delayoff cleared
        foreach (var device in group)
        {
            var delayoff = Convert.ToInt32(await device.GetProp(PROPERTIES.delayoff));
            await Assert.That(delayoff).IsEqualTo(0);
        }

        await group.TurnOff();
    }

    #endregion

    #region Connection Tests

    [Test]
    public async Task DeviceGroup_ConnectDisconnect_VerifyAllDevicesState()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();
        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices found");
            return;
        }

        using var group = new DeviceGroup(devices);

        // Initially not connected
        foreach (var device in group)
        {
            await Assert.That(device.IsConnected).IsFalse();
        }

        // Connect
        var result = await group.Connect();
        await Assert.That(result).IsTrue();

        foreach (var device in group)
        {
            await Assert.That(device.IsConnected).IsTrue();
        }

        // Disconnect
        group.Disconnect();

        foreach (var device in group)
        {
            await Assert.That(device.IsConnected).IsFalse();
        }

        // Reconnect to verify it still works
        result = await group.Connect();
        await Assert.That(result).IsTrue();

        foreach (var device in group)
        {
            await Assert.That(device.IsConnected).IsTrue();
        }
    }

    #endregion

    #region Properties Tests

    [Test]
    public async Task DeviceGroup_Name_VerifyNameProperty()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();
        if (devices.Count < 1)
        {
            Console.WriteLine("SKIPPED: No devices found");
            return;
        }

        using var group = new DeviceGroup(devices, "Living Room Lights");
        await Assert.That(group.Name).IsEqualTo("Living Room Lights");

        var toString = group.ToString();
        await Assert.That(toString).Contains("Living Room Lights");
        await Assert.That(toString).Contains($"{devices.Count} devices");
        Console.WriteLine($"Group: {toString}");
    }

    #endregion
}
