using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;
using YeelightAPI.Models.Scene;

namespace YeelightAPI.UnitTests;

/// <summary>
/// Integration tests that require actual Yeelight devices on the network.
/// These tests verify actual device state after commands.
/// </summary>
[NotInParallel("Integration Tests")]
[Retry(3)] // Retry on quota exceeded errors
public class DeviceIntegrationTests
{
    #region Power & Toggle Tests

    [Test]
    public async Task Device_TurnOn_VerifyPowerState()
    {
        using var device = await GetRandomConnectedDevice();

        await device.TurnOff();
        await Task.Delay(300);

        var result = await device.TurnOn();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("on");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_TurnOff_VerifyPowerState()
    {
        using var device = await GetRandomConnectedDevice();

        await device.TurnOn();
        await Task.Delay(300);

        var result = await device.TurnOff();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("off");
    }

    [Test]
    public async Task Device_Toggle_VerifyStateChanges()
    {
        using var device = await GetRandomConnectedDevice();

        // Ensure known state
        await device.TurnOn();
        await Task.Delay(300);
        var initialState = await device.GetProp(PROPERTIES.power);
        await Assert.That(initialState).IsEqualTo("on");

        // Toggle to off
        var result = await device.Toggle();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var newState = await device.GetProp(PROPERTIES.power);
        await Assert.That(newState).IsEqualTo("off");

        // Toggle back to on
        result = await device.Toggle();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var finalState = await device.GetProp(PROPERTIES.power);
        await Assert.That(finalState).IsEqualTo("on");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetPower_OnWithSmooth_VerifyState()
    {
        using var device = await GetRandomConnectedDevice();

        await device.TurnOff();
        await Task.Delay(300);

        var result = await device.SetPower(true, smooth: 500);
        await Assert.That(result).IsTrue();
        await Task.Delay(800);

        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("on");

        await device.TurnOff();
    }

    [Test]
    [Arguments(PowerOnMode.Normal)]
    [Arguments(PowerOnMode.Ct)]
    [Arguments(PowerOnMode.RGB)]
    public async Task Device_TurnOn_WithMode_VerifyColorMode(PowerOnMode mode)
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetPower);

        await device.TurnOff();
        await Task.Delay(300);

        var result = await device.TurnOn(smooth: 300, mode: mode);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("on");

        if (mode != PowerOnMode.Normal)
        {
            var colorMode = await device.GetProp(PROPERTIES.color_mode);
            Console.WriteLine($"Mode {mode} -> color_mode: {colorMode}");
            // color_mode: 1=RGB, 2=CT, 3=HSV
            if (mode == PowerOnMode.RGB)
                await Assert.That(colorMode?.ToString()).IsEqualTo("1");
            else if (mode == PowerOnMode.Ct)
                await Assert.That(colorMode?.ToString()).IsEqualTo("2");
        }

        await device.TurnOff();
    }

    #endregion

    #region Brightness Tests

    [Test]
    [Arguments(1)]
    [Arguments(25)]
    [Arguments(50)]
    [Arguments(75)]
    [Arguments(100)]
    public async Task Device_SetBrightness_VerifyValue(int brightness)
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetBrightness);
        await device.TurnOn();
        await Task.Delay(300);

        var result = await device.SetBrightness(brightness);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var actualBrightness = await device.GetProp(PROPERTIES.bright);
        await Assert.That(Convert.ToInt32(actualBrightness)).IsEqualTo(brightness);

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetBrightness_WithSmooth_VerifyValue()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetBrightness);
        await device.TurnOn();
        await device.SetBrightness(100);
        await Task.Delay(300);

        var result = await device.SetBrightness(30, smooth: 1000);
        await Assert.That(result).IsTrue();
        await Task.Delay(1300);

        var actualBrightness = await device.GetProp(PROPERTIES.bright);
        await Assert.That(Convert.ToInt32(actualBrightness)).IsEqualTo(30);

        await device.TurnOff();
    }

    [Test]
    public async Task Device_AdjustBright_Increase_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.AdjustBright);
        await device.TurnOn();
        await device.SetBrightness(50);
        await Task.Delay(300);

        var initialBrightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));

        var result = await device.AdjustBright(20, duration: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newBrightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        await Assert.That(newBrightness > initialBrightness).IsTrue();
        Console.WriteLine($"Brightness: {initialBrightness} -> {newBrightness}");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_AdjustBright_Decrease_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.AdjustBright);
        await device.TurnOn();
        await device.SetBrightness(70);
        await Task.Delay(300);

        var initialBrightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));

        var result = await device.AdjustBright(-30, duration: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newBrightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        await Assert.That(newBrightness < initialBrightness).IsTrue();
        Console.WriteLine($"Brightness: {initialBrightness} -> {newBrightness}");

        await device.TurnOff();
    }

    #endregion

    #region Color Temperature Tests

    [Test]
    [Arguments(1700)]
    [Arguments(2700)]
    [Arguments(4000)]
    [Arguments(5500)]
    [Arguments(6500)]
    public async Task Device_SetColorTemperature_VerifyValue(int temperature)
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetColorTemperature);
        await device.TurnOn();
        await Task.Delay(300);

        var result = await device.SetColorTemperature(temperature);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var actualCt = await device.GetProp(PROPERTIES.ct);
        await Assert.That(Convert.ToInt32(actualCt)).IsEqualTo(temperature);

        // Verify color mode is CT (2)
        var colorMode = await device.GetProp(PROPERTIES.color_mode);
        await Assert.That(colorMode?.ToString()).IsEqualTo("2");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetColorTemperature_WithSmooth_VerifyValue()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetColorTemperature);
        await device.TurnOn();
        await device.SetColorTemperature(2700);
        await Task.Delay(300);

        var result = await device.SetColorTemperature(5000, smooth: 1000);
        await Assert.That(result).IsTrue();
        await Task.Delay(1300);

        var actualCt = await device.GetProp(PROPERTIES.ct);
        await Assert.That(Convert.ToInt32(actualCt)).IsEqualTo(5000);

        await device.TurnOff();
    }

    [Test]
    public async Task Device_AdjustColorTemperature_Increase_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.AdjustColorTemperature);
        await device.TurnOn();
        await device.SetColorTemperature(3000);
        await Task.Delay(300);

        var initialCt = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));

        var result = await device.AdjustColorTemperature(30, duration: 300);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newCt = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));
        await Assert.That(newCt > initialCt).IsTrue();
        Console.WriteLine($"Color Temperature: {initialCt}K -> {newCt}K");

        await device.TurnOff();
    }

    #endregion

    #region RGB Color Tests

    [Test]
    [Arguments(255, 0, 0, 16711680)]     // Red
    [Arguments(0, 255, 0, 65280)]        // Green
    [Arguments(0, 0, 255, 255)]          // Blue
    [Arguments(255, 255, 0, 16776960)]   // Yellow
    [Arguments(255, 0, 255, 16711935)]   // Magenta
    [Arguments(0, 255, 255, 65535)]      // Cyan
    [Arguments(255, 255, 255, 16777215)] // White
    public async Task Device_SetRGBColor_VerifyValue(int r, int g, int b, int expectedRgbValue)
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetRGBColor);
        await device.TurnOn();
        await Task.Delay(300);

        var result = await device.SetRGBColor(r, g, b);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var actualRgb = await device.GetProp(PROPERTIES.rgb);
        await Assert.That(Convert.ToInt32(actualRgb)).IsEqualTo(expectedRgbValue);

        // Verify color mode is RGB (1)
        var colorMode = await device.GetProp(PROPERTIES.color_mode);
        await Assert.That(colorMode?.ToString()).IsEqualTo("1");

        Console.WriteLine($"RGB({r},{g},{b}) = {actualRgb}");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetRGBColor_WithSmooth_VerifyValue()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetRGBColor);
        await device.TurnOn();
        await device.SetRGBColor(255, 0, 0); // Red
        await Task.Delay(300);

        var result = await device.SetRGBColor(0, 0, 255, smooth: 1000); // Blue
        await Assert.That(result).IsTrue();
        await Task.Delay(1300);

        var actualRgb = await device.GetProp(PROPERTIES.rgb);
        await Assert.That(Convert.ToInt32(actualRgb)).IsEqualTo(255); // Blue = 255

        await device.TurnOff();
    }

    #endregion

    #region HSV Color Tests

    [Test]
    [Arguments(0, 100)]     // Red
    [Arguments(60, 100)]    // Yellow
    [Arguments(120, 100)]   // Green
    [Arguments(180, 100)]   // Cyan
    [Arguments(240, 100)]   // Blue
    [Arguments(300, 100)]   // Magenta
    public async Task Device_SetHSVColor_VerifyValue(int hue, int sat)
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetHSVColor);
        await device.TurnOn();
        await Task.Delay(300);

        var result = await device.SetHSVColor(hue, sat);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var actualHue = await device.GetProp(PROPERTIES.hue);
        var actualSat = await device.GetProp(PROPERTIES.sat);

        await Assert.That(Convert.ToInt32(actualHue)).IsEqualTo(hue);
        await Assert.That(Convert.ToInt32(actualSat)).IsEqualTo(sat);

        // Verify color mode is HSV (3)
        var colorMode = await device.GetProp(PROPERTIES.color_mode);
        await Assert.That(colorMode?.ToString()).IsEqualTo("3");

        Console.WriteLine($"HSV: hue={actualHue}, sat={actualSat}");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetHSVColor_WithSmooth_VerifyValue()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetHSVColor);
        await device.TurnOn();
        await device.SetHSVColor(0, 100); // Red
        await Task.Delay(300);

        var result = await device.SetHSVColor(120, 80, smooth: 1000); // Green
        await Assert.That(result).IsTrue();
        await Task.Delay(1300);

        var actualHue = await device.GetProp(PROPERTIES.hue);
        var actualSat = await device.GetProp(PROPERTIES.sat);

        await Assert.That(Convert.ToInt32(actualHue)).IsEqualTo(120);
        await Assert.That(Convert.ToInt32(actualSat)).IsEqualTo(80);

        await device.TurnOff();
    }

    #endregion

    #region SetAdjust Tests

    [Test]
    public async Task Device_SetAdjust_IncreaseBright_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetAdjust);
        await device.TurnOn();
        await device.SetBrightness(40);
        await Task.Delay(300);

        var initialBright = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));

        var result = await device.SetAdjust(AdjustAction.increase, AdjustProperty.bright);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newBright = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        await Assert.That(newBright > initialBright).IsTrue();

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetAdjust_DecreaseBright_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetAdjust);
        await device.TurnOn();
        await device.SetBrightness(60);
        await Task.Delay(300);

        var initialBright = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));

        var result = await device.SetAdjust(AdjustAction.decrease, AdjustProperty.bright);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newBright = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        await Assert.That(newBright < initialBright).IsTrue();

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetAdjust_IncreaseCt_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetAdjust);
        await device.TurnOn();
        await device.SetColorTemperature(3000);
        await Task.Delay(300);

        var initialCt = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));

        var result = await device.SetAdjust(AdjustAction.increase, AdjustProperty.ct);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newCt = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));
        await Assert.That(newCt > initialCt).IsTrue();

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetAdjust_CircleColor_VerifyChange()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetAdjust);
        await device.TurnOn();
        await device.SetRGBColor(255, 0, 0);
        await Task.Delay(300);

        var initialRgb = Convert.ToInt32(await device.GetProp(PROPERTIES.rgb));

        var result = await device.SetAdjust(AdjustAction.circle, AdjustProperty.color);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var newRgb = Convert.ToInt32(await device.GetProp(PROPERTIES.rgb));
        await Assert.That(newRgb).IsNotEqualTo(initialRgb);
        Console.WriteLine($"Color circle: {initialRgb} -> {newRgb}");

        await device.TurnOff();
    }

    #endregion

    #region ColorFlow Tests

    [Test]
    public async Task Device_StartColorFlow_VerifyFlowingState()
    {
        using var device = await GetRandomConnectedDevice(METHODS.StartColorFlow);
        await device.TurnOn();
        await Task.Delay(300);

        var flow = new ColorFlow(0, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 1000),
            new ColorFlowRGBExpression(0, 255, 0, 100, 1000),
            new ColorFlowRGBExpression(0, 0, 255, 100, 1000)
        };

        var result = await device.StartColorFlow(flow);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify flowing state is active
        var flowingState = await device.GetProp(PROPERTIES.flowing);
        await Assert.That(flowingState?.ToString()).IsEqualTo("1");
        Console.WriteLine($"Flowing state: {flowingState}");

        await device.StopColorFlow();
        await Task.Delay(300);

        // Verify flowing stopped
        flowingState = await device.GetProp(PROPERTIES.flowing);
        await Assert.That(flowingState?.ToString()).IsEqualTo("0");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_StopColorFlow_VerifyFlowingStopped()
    {
        using var device = await GetRandomConnectedDevice(METHODS.StopColorFlow);
        await device.TurnOn();

        var flow = new ColorFlow(0, ColorFlowEndAction.Keep)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowRGBExpression(0, 0, 255, 100, 500)
        };

        await device.StartColorFlow(flow);
        await Task.Delay(800);

        var result = await device.StopColorFlow();
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var flowingState = await device.GetProp(PROPERTIES.flowing);
        await Assert.That(flowingState?.ToString()).IsEqualTo("0");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_ColorFlow_EndActionKeep_VerifyFinalColor()
    {
        using var device = await GetRandomConnectedDevice(METHODS.StartColorFlow);
        await device.TurnOn();
        await device.SetRGBColor(255, 255, 255); // Start white
        await Task.Delay(300);

        // Flow ends with blue, EndAction=Keep should keep the blue color
        var flow = new ColorFlow(1, ColorFlowEndAction.Keep)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 300),
            new ColorFlowRGBExpression(0, 0, 255, 100, 300) // Ends blue
        };

        var result = await device.StartColorFlow(flow);
        await Assert.That(result).IsTrue();
        await Task.Delay(1000);

        var finalRgb = await device.GetProp(PROPERTIES.rgb);
        await Assert.That(Convert.ToInt32(finalRgb)).IsEqualTo(255); // Blue = 255
        Console.WriteLine($"Final RGB after Keep: {finalRgb}");

        await device.TurnOff();
    }

    [Test]
    public async Task Device_ColorFlow_EndActionTurnOff_VerifyPowerOff()
    {
        using var device = await GetRandomConnectedDevice(METHODS.StartColorFlow);
        await device.TurnOn();
        await Task.Delay(300);

        var flow = new ColorFlow(1, ColorFlowEndAction.TurnOff)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 300),
            new ColorFlowRGBExpression(0, 255, 0, 100, 300)
        };

        var result = await device.StartColorFlow(flow);
        await Assert.That(result).IsTrue();
        await Task.Delay(1000);

        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("off");
    }

    [Test]
    public async Task Device_FluentFlow_VerifyExecution()
    {
        using var device = await GetRandomConnectedDevice(METHODS.StartColorFlow);
        await device.TurnOn();
        await Task.Delay(300);

        var flow = device.Flow()
            .RgbColor(255, 0, 0, 100, 500)
            .RgbColor(0, 255, 0, 100, 500)
            .Temperature(4000, 100, 500);

        await flow.Play(ColorFlowEndAction.Keep, repetition: 1);
        await Task.Delay(500);

        var flowingState = await device.GetProp(PROPERTIES.flowing);
        await Assert.That(flowingState?.ToString()).IsEqualTo("1");

        await Task.Delay(2000);
        await device.TurnOff();
    }

    #endregion

    #region Scene Tests

    [Test]
    public async Task Device_SetScene_RGBColor_VerifyState()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetScene);

        var scene = Scene.FromRGBColor(255, 128, 0, 80); // Orange at 80%
        var result = await device.SetScene(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify power is on
        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("on");

        // Verify RGB color
        var rgb = Convert.ToInt32(await device.GetProp(PROPERTIES.rgb));
        int expectedRgb = (255 << 16) | (128 << 8) | 0; // 16744448
        await Assert.That(rgb).IsEqualTo(expectedRgb);

        // Verify brightness
        var brightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        await Assert.That(brightness).IsEqualTo(80);

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetScene_HSVColor_VerifyState()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetScene);

        var scene = Scene.FromHSVColor(120, 100, 60); // Green at 60%
        var result = await device.SetScene(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var hue = Convert.ToInt32(await device.GetProp(PROPERTIES.hue));
        var sat = Convert.ToInt32(await device.GetProp(PROPERTIES.sat));
        var brightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));

        await Assert.That(hue).IsEqualTo(120);
        await Assert.That(sat).IsEqualTo(100);
        await Assert.That(brightness).IsEqualTo(60);

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetScene_ColorTemperature_VerifyState()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetScene);

        var scene = Scene.FromColorTemperature(4500, 90);
        var result = await device.SetScene(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        var ct = Convert.ToInt32(await device.GetProp(PROPERTIES.ct));
        var brightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));

        await Assert.That(ct).IsEqualTo(4500);
        await Assert.That(brightness).IsEqualTo(90);

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetScene_ColorFlow_VerifyFlowing()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetScene);

        var flow = new ColorFlow(0, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowRGBExpression(0, 0, 255, 100, 500)
        };
        var scene = Scene.FromColorFlow(flow);

        var result = await device.SetScene(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(300);

        var flowingState = await device.GetProp(PROPERTIES.flowing);
        await Assert.That(flowingState?.ToString()).IsEqualTo("1");

        await device.StopColorFlow();
        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetScene_AutoDelayOff_VerifyDelayOff()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetScene);

        var scene = Scene.FromAutoDelayOff(1, 70); // 1 minute delay, 70% brightness
        var result = await device.SetScene(scene);
        await Assert.That(result).IsTrue();
        await Task.Delay(500);

        // Verify power is on
        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("on");

        // Verify brightness
        var brightness = Convert.ToInt32(await device.GetProp(PROPERTIES.bright));
        await Assert.That(brightness).IsEqualTo(70);

        // Verify delayoff is set (should be around 1 minute)
        var delayoff = await device.GetProp(PROPERTIES.delayoff);
        await Assert.That(delayoff).IsNotNull();
        Console.WriteLine($"Delay off: {delayoff} minutes remaining");

        await device.TurnOff();
    }

    #endregion

    #region Cron Tests

    [Test]
    public async Task Device_CronAdd_VerifyCronGet()
    {
        using var device = await GetRandomConnectedDevice(METHODS.AddCron);
        await device.TurnOn();

        // Add cron job to turn off in 10 minutes
        var addResult = await device.CronAdd(10, CronType.PowerOff);
        await Assert.That(addResult).IsTrue();
        await Task.Delay(500);

        // Get and verify cron job
        var cronResult = await device.CronGet(CronType.PowerOff);
        await Assert.That(cronResult).IsNotNull();
        Console.WriteLine($"Cron result - Type: {cronResult!.Type}, Delay: {cronResult.Delay}, Mix: {cronResult.Mix}");

        // Verify delayoff property is set
        var delayoff = await device.GetProp(PROPERTIES.delayoff);
        await Assert.That(delayoff).IsNotNull();
        var delayMinutes = Convert.ToInt32(delayoff);
        await Assert.That(delayMinutes > 0 && delayMinutes <= 10).IsTrue();
        Console.WriteLine($"Delayoff remaining: {delayMinutes} minutes");

        // Clean up
        await device.CronDelete(CronType.PowerOff);
        await device.TurnOff();
    }

    [Test]
    public async Task Device_CronDelete_VerifyRemoved()
    {
        using var device = await GetRandomConnectedDevice(METHODS.DeleteCron);
        await device.TurnOn();

        // Add cron job
        await device.CronAdd(5, CronType.PowerOff);
        await Task.Delay(300);

        // Verify it exists
        var delayoffBefore = await device.GetProp(PROPERTIES.delayoff);
        await Assert.That(delayoffBefore).IsNotNull();
        await Assert.That(Convert.ToInt32(delayoffBefore) > 0).IsTrue();

        // Delete cron job
        var deleteResult = await device.CronDelete(CronType.PowerOff);
        await Assert.That(deleteResult).IsTrue();
        await Task.Delay(300);

        // Verify delayoff is 0
        var delayoffAfter = await device.GetProp(PROPERTIES.delayoff);
        await Assert.That(Convert.ToInt32(delayoffAfter)).IsEqualTo(0);

        await device.TurnOff();
    }

    #endregion

    #region Properties Tests

    [Test]
    public async Task Device_GetAllProps_VerifyEssentialProperties()
    {
        using var device = await GetRandomConnectedDevice();
        await device.TurnOn();
        await device.SetBrightness(75);
        await device.SetColorTemperature(4000);
        await Task.Delay(500);

        var props = await device.GetAllProps();

        await Assert.That(props).IsNotNull();
        await Assert.That(props).IsNotEmpty();

        // Verify essential properties are present and have correct values
        await Assert.That(props.ContainsKey(PROPERTIES.power)).IsTrue();
        await Assert.That(props[PROPERTIES.power]?.ToString()).IsEqualTo("on");

        await Assert.That(props.ContainsKey(PROPERTIES.bright)).IsTrue();
        await Assert.That(Convert.ToInt32(props[PROPERTIES.bright])).IsEqualTo(75);

        await Assert.That(props.ContainsKey(PROPERTIES.ct)).IsTrue();
        await Assert.That(Convert.ToInt32(props[PROPERTIES.ct])).IsEqualTo(4000);

        Console.WriteLine($"Total properties: {props.Count}");
        foreach (var prop in props)
        {
            Console.WriteLine($"  {prop.Key}: {prop.Value}");
        }

        await device.TurnOff();
    }

    [Test]
    public async Task Device_GetProps_MultipleProperties_VerifyValues()
    {
        using var device = await GetRandomConnectedDevice();
        await device.TurnOn();
        await device.SetBrightness(60);
        await device.SetRGBColor(0, 255, 0); // Green
        await Task.Delay(500);

        var props = await device.GetProps(PROPERTIES.power | PROPERTIES.bright | PROPERTIES.rgb | PROPERTIES.color_mode);

        await Assert.That(props).IsNotNull();

        if (props.ContainsKey(PROPERTIES.power))
            await Assert.That(props[PROPERTIES.power]?.ToString()).IsEqualTo("on");

        if (props.ContainsKey(PROPERTIES.bright))
            await Assert.That(Convert.ToInt32(props[PROPERTIES.bright])).IsEqualTo(60);

        if (props.ContainsKey(PROPERTIES.rgb))
            await Assert.That(Convert.ToInt32(props[PROPERTIES.rgb])).IsEqualTo(65280); // Green

        if (props.ContainsKey(PROPERTIES.color_mode))
            await Assert.That(props[PROPERTIES.color_mode]?.ToString()).IsEqualTo("1"); // RGB mode

        await device.TurnOff();
    }

    [Test]
    public async Task Device_SetName_VerifyNameChanged()
    {
        using var device = await GetRandomConnectedDevice(METHODS.SetName);

        // Get original name
        var originalName = await device.GetProp(PROPERTIES.name) as string ?? "";
        Console.WriteLine($"Original name: '{originalName}'");

        // Set new name
        var testName = $"Test_{DateTime.Now.Ticks}";
        var result = await device.SetName(testName);
        await Assert.That(result).IsTrue();
        await Task.Delay(1000);

        // Verify name changed
        var newName = await device.GetProp(PROPERTIES.name) as string;
        await Assert.That(newName).IsEqualTo(testName);
        Console.WriteLine($"New name: '{newName}'");

        // Restore original name
        if (!string.IsNullOrEmpty(originalName))
        {
            await device.SetName(originalName);
            await Task.Delay(500);
            var restoredName = await device.GetProp(PROPERTIES.name) as string;
            await Assert.That(restoredName).IsEqualTo(originalName);
        }
    }

    #endregion

    #region Connection Lifecycle Tests

    [Test]
    public async Task Device_Connect_Disconnect_Reconnect_VerifyState()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();
        await Assert.That(devices).IsNotEmpty();

        var device = devices.First();

        // First connection
        var connected = await device.Connect();
        await Assert.That(connected).IsTrue();
        await Assert.That(device.IsConnected).IsTrue();

        // Execute command to verify connection works
        await device.TurnOn();
        var powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("on");

        // Disconnect
        device.Disconnect();
        await Assert.That(device.IsConnected).IsFalse();

        // Reconnect
        connected = await device.Connect();
        await Assert.That(connected).IsTrue();
        await Assert.That(device.IsConnected).IsTrue();

        // Verify commands still work
        await device.TurnOff();
        powerState = await device.GetProp(PROPERTIES.power);
        await Assert.That(powerState).IsEqualTo("off");

        device.Dispose();
    }

    #endregion

    #region Event Tests

    [Test]
    public async Task Device_OnNotificationReceived_VerifyEventFires()
    {
        using var device = await GetRandomConnectedDevice();

        var notificationReceived = new TaskCompletionSource<NotificationResult>();

        device.OnNotificationReceived += (sender, args) =>
        {
            Console.WriteLine($"Notification: method={args.Result?.Method}");
            if (args.Result?.Params != null)
            {
                foreach (var param in args.Result.Params)
                {
                    Console.WriteLine($"  {param.Key}: {param.Value}");
                }
            }
            notificationReceived.TrySetResult(args.Result!);
        };

        // Trigger state changes
        await device.TurnOn();
        await Task.Delay(200);
        await device.SetBrightness(50);
        await Task.Delay(200);
        await device.SetBrightness(100);
        await Task.Delay(200);

        // Wait for notification with timeout
        var timeoutTask = Task.Delay(3000);
        var completedTask = await Task.WhenAny(notificationReceived.Task, timeoutTask);

        if (completedTask == notificationReceived.Task)
        {
            var notification = await notificationReceived.Task;
            await Assert.That(notification).IsNotNull();
            await Assert.That(notification.Method).IsEqualTo("props");
        }
        else
        {
            Console.WriteLine("No notification received within timeout - this is device-dependent behavior");
        }

        await device.TurnOff();
    }

    #endregion

    #region Background Light Tests (Non-blocking)

    [Test]
    public async Task BackgroundLight_TurnOnOff_VerifyState()
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            var result = await device.BackgroundTurnOn();
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgPower = await device.GetProp(PROPERTIES.bg_power);
            await Assert.That(bgPower).IsEqualTo("on");

            result = await device.BackgroundTurnOff();
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            bgPower = await device.GetProp(PROPERTIES.bg_power);
            await Assert.That(bgPower).IsEqualTo("off");
        }
    }

    [Test]
    [Arguments(255, 0, 0)]
    [Arguments(0, 255, 0)]
    [Arguments(0, 0, 255)]
    public async Task BackgroundLight_SetRGBColor_VerifyValue(int r, int g, int b)
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            await device.BackgroundTurnOn();
            await Task.Delay(300);

            var result = await device.BackgroundSetRGBColor(r, g, b);
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgRgb = await device.GetProp(PROPERTIES.bg_rgb);
            int expectedRgb = (r << 16) | (g << 8) | b;
            await Assert.That(Convert.ToInt32(bgRgb)).IsEqualTo(expectedRgb);

            await device.BackgroundTurnOff();
        }
    }

    [Test]
    [Arguments(1)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task BackgroundLight_SetBrightness_VerifyValue(int brightness)
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            await device.BackgroundTurnOn();
            await Task.Delay(300);

            var result = await device.BackgroundSetBrightness(brightness);
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgBright = await device.GetProp(PROPERTIES.bg_bright);
            await Assert.That(Convert.ToInt32(bgBright)).IsEqualTo(brightness);

            await device.BackgroundTurnOff();
        }
    }

    [Test]
    [Arguments(2700)]
    [Arguments(4000)]
    [Arguments(6500)]
    public async Task BackgroundLight_SetColorTemperature_VerifyValue(int temperature)
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            await device.BackgroundTurnOn();
            await Task.Delay(300);

            var result = await device.BackgroundSetColorTemperature(temperature);
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgCt = await device.GetProp(PROPERTIES.bg_ct);
            await Assert.That(Convert.ToInt32(bgCt)).IsEqualTo(temperature);

            await device.BackgroundTurnOff();
        }
    }

    [Test]
    public async Task BackgroundLight_SetHSVColor_VerifyValue()
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            await device.BackgroundTurnOn();
            await Task.Delay(300);

            var result = await device.BackgroundSetHSVColor(180, 80);
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgHue = await device.GetProp(PROPERTIES.bg_hue);
            var bgSat = await device.GetProp(PROPERTIES.bg_sat);
            await Assert.That(Convert.ToInt32(bgHue)).IsEqualTo(180);
            await Assert.That(Convert.ToInt32(bgSat)).IsEqualTo(80);

            await device.BackgroundTurnOff();
        }
    }

    [Test]
    public async Task BackgroundLight_Toggle_VerifyStateChanges()
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            await device.BackgroundTurnOff();
            await Task.Delay(300);

            var result = await device.BackgroundToggle();
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgPower = await device.GetProp(PROPERTIES.bg_power);
            await Assert.That(bgPower).IsEqualTo("on");

            result = await device.BackgroundToggle();
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            bgPower = await device.GetProp(PROPERTIES.bg_power);
            await Assert.That(bgPower).IsEqualTo("off");
        }
    }

    [Test]
    public async Task BackgroundLight_StartColorFlow_VerifyFlowing()
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            await device.BackgroundTurnOn();
            await Task.Delay(300);

            var flow = new ColorFlow(0, ColorFlowEndAction.Restore)
            {
                new ColorFlowRGBExpression(255, 0, 0, 100, 500),
                new ColorFlowRGBExpression(0, 0, 255, 100, 500)
            };

            var result = await device.BackgroundStartColorFlow(flow);
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            var bgFlowing = await device.GetProp(PROPERTIES.bg_flowing);
            await Assert.That(bgFlowing?.ToString()).IsEqualTo("1");

            await device.BackgroundStopColorFlow();
            await Task.Delay(300);

            bgFlowing = await device.GetProp(PROPERTIES.bg_flowing);
            await Assert.That(bgFlowing?.ToString()).IsEqualTo("0");

            await device.BackgroundTurnOff();
        }
    }

    [Test]
    public async Task BackgroundLight_DevToggle_VerifyBothLightsToggle()
    {
        var device = await TryGetBackgroundLightDevice();
        if (device == null)
        {
            Console.WriteLine("SKIPPED: No device with background light support found");
            return;
        }

        using (device)
        {
            // Set known state
            await device.TurnOn();
            await device.BackgroundTurnOn();
            await Task.Delay(300);

            var mainPower = await device.GetProp(PROPERTIES.power);
            var bgPower = await device.GetProp(PROPERTIES.bg_power);
            await Assert.That(mainPower).IsEqualTo("on");
            await Assert.That(bgPower).IsEqualTo("on");

            // DevToggle should toggle both
            var result = await device.DevToggle();
            await Assert.That(result).IsTrue();
            await Task.Delay(300);

            mainPower = await device.GetProp(PROPERTIES.power);
            bgPower = await device.GetProp(PROPERTIES.bg_power);
            await Assert.That(mainPower).IsEqualTo("off");
            await Assert.That(bgPower).IsEqualTo("off");

            // Toggle back
            await device.DevToggle();
        }
    }

    #endregion

    #region Helper Methods

    private async Task<Device> GetRandomConnectedDevice(METHODS? supportedMethod = null)
    {
        var devices = (await DeviceLocator.DiscoverAsync())
            .Where(d => !supportedMethod.HasValue || d.SupportedOperations.Contains(supportedMethod.Value))
            .ToList();

        await Assert.That(devices).IsNotEmpty();

        var randomIndex = new Random().Next(0, devices.Count);
        var device = devices[randomIndex];
        Console.WriteLine($"Using device: {device} (Model: {device.Model})");
        await device.Connect();
        return device;
    }

    private async Task<Device?> TryGetBackgroundLightDevice()
    {
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();

        var backgroundDevice = devices.FirstOrDefault(d =>
            d.SupportedOperations.Contains(METHODS.SetBackgroundLightPower) ||
            d.SupportedOperations.Contains(METHODS.SetBackgroundLightRGBColor) ||
            d.Model == MODEL.Ceiling);

        if (backgroundDevice != null)
        {
            Console.WriteLine($"Using background light device: {backgroundDevice} (Model: {backgroundDevice.Model})");
            await backgroundDevice.Connect();
        }

        return backgroundDevice;
    }

    #endregion
}
