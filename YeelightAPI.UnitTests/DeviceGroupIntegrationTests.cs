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
        await group.ConnectAsync();

        var result = await group.TurnOnAsync();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices are on
        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");
        }

        result = await group.TurnOffAsync();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices are off
        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
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
        await group.ConnectAsync();

        // Set known state (all on)
        await group.TurnOnAsync();
        await Task.Delay(500);

        // Toggle to off
        var result = await group.ToggleAsync();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("off");
        }

        // Toggle back to on
        result = await group.ToggleAsync();
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();

        var result = await group.SetPowerAsync(true, smooth: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");
        }

        result = await group.SetPowerAsync(false, smooth: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        var result = await group.SetBrightnessAsync(65);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var brightness = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.bright));
            await Assert.That(brightness).IsEqualTo(65);
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await group.SetBrightnessAsync(50);
        await Task.Delay(500);

        // Get initial brightness for each device
        var initialBrightness = new Dictionary<Device, int>();
        foreach (var device in group)
        {
            initialBrightness[device] = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.bright));
        }

        var result = await group.AdjustBrightAsync(20);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var newBrightness = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.bright));
            await Assert.That(newBrightness > initialBrightness[device]).IsTrue();
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await group.SetBrightnessAsync(40);
        await Task.Delay(500);

        var initialBrightness = new Dictionary<Device, int>();
        foreach (var device in group)
        {
            initialBrightness[device] = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.bright));
        }

        var result = await group.SetAdjustAsync(AdjustAction.increase, AdjustProperty.bright);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var newBrightness = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.bright));
            await Assert.That(newBrightness > initialBrightness[device]).IsTrue();
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        var result = await group.SetColorTemperatureAsync(3500);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var ct = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.ct));
            await Assert.That(ct).IsEqualTo(3500);
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await group.SetColorTemperatureAsync(3000);
        await Task.Delay(500);

        var initialCt = new Dictionary<Device, int>();
        foreach (var device in group)
        {
            initialCt[device] = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.ct));
        }

        var result = await group.AdjustColorTemperatureAsync(30);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var newCt = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.ct));
            await Assert.That(newCt > initialCt[device]).IsTrue();
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        var result = await group.SetRGBColorAsync(128, 0, 255); // Purple
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        int expectedRgb = (128 << 16) | (0 << 8) | 255;
        foreach (var device in group)
        {
            var rgb = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.rgb));
            await Assert.That(rgb).IsEqualTo(expectedRgb);
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        var result = await group.SetHSVColorAsync(180, 80); // Cyan
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        foreach (var device in group)
        {
            var hue = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.hue));
            var sat = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.sat));
            await Assert.That(hue).IsEqualTo(180);
            await Assert.That(sat).IsEqualTo(80);
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        var flow = new ColorFlow(0, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowRGBExpression(0, 255, 0, 100, 500),
            new ColorFlowRGBExpression(0, 0, 255, 100, 500)
        };

        var result = await group.StartColorFlowAsync(flow);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices are flowing
        foreach (var device in group)
        {
            var flowingState = await device.GetPropAsync(PROPERTIES.flowing);
            await Assert.That(flowingState?.ToString()).IsEqualTo("1");
        }

        result = await group.StopColorFlowAsync();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        // Verify all devices stopped flowing
        foreach (var device in group)
        {
            var flowingState = await device.GetPropAsync(PROPERTIES.flowing);
            await Assert.That(flowingState?.ToString()).IsEqualTo("0");
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        var flow = group.Flow()
            .RgbColor(255, 0, 0, 100, 300)
            .RgbColor(0, 255, 0, 100, 300)
            .Temperature(4000, 100, 300);

        await flow.PlayAsync(ColorFlowEndAction.Restore, repetition: 1);
        await Task.Delay(300);

        // Verify all devices are flowing
        foreach (var device in group)
        {
            var flowingState = await device.GetPropAsync(PROPERTIES.flowing);
            await Assert.That(flowingState?.ToString()).IsEqualTo("1");
        }

        await group.StopColorFlowAsync();
        await group.TurnOffAsync();
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
        await group.ConnectAsync();

        var scene = Scene.FromRGBColor(0, 200, 100, 70); // Greenish at 70%
        var result = await group.SetSceneAsync(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        int expectedRgb = (0 << 16) | (200 << 8) | 100;
        foreach (var device in group)
        {
            var powerState = await device.GetPropAsync(PROPERTIES.power);
            await Assert.That(powerState).IsEqualTo("on");

            var rgb = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.rgb));
            await Assert.That(rgb).IsEqualTo(expectedRgb);

            var brightness = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.bright));
            await Assert.That(brightness).IsEqualTo(70);
        }

        await group.TurnOffAsync();
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
        await group.ConnectAsync();
        await group.TurnOnAsync();
        await Task.Delay(300);

        // Add cron
        var result = await group.CronAddAsync(5, CronType.PowerOff);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify all devices have delayoff set
        foreach (var device in group)
        {
            var delayoff = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.delayoff));
            await Assert.That(delayoff > 0 && delayoff <= 5).IsTrue();
        }

        // Delete cron
        result = await group.CronDeleteAsync(CronType.PowerOff);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        // Verify all devices have delayoff cleared
        foreach (var device in group)
        {
            var delayoff = Convert.ToInt32(await device.GetPropAsync(PROPERTIES.delayoff));
            await Assert.That(delayoff).IsEqualTo(0);
        }

        await group.TurnOffAsync();
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
        var result = await group.ConnectAsync();
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
        result = await group.ConnectAsync();
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
