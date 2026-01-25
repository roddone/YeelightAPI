using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Scene;

namespace YeelightAPI.UnitTests;

public class SceneTests
{
    #region FromRGBColor Tests

    [Test]
    [Arguments(255, 0, 0, 100, 16711680)]   // Red
    [Arguments(0, 255, 0, 50, 65280)]       // Green
    [Arguments(0, 0, 255, 1, 255)]          // Blue
    [Arguments(255, 255, 255, 100, 16777215)] // White
    [Arguments(0, 0, 0, 50, 0)]             // Black
    public async Task FromRGBColor_CreatesCorrectScene(int r, int g, int b, int brightness, int expectedColor)
    {
        var scene = Scene.FromRGBColor(r, g, b, brightness);

        await Assert.That(scene.Count).IsEqualTo(3);
        await Assert.That(scene[0]).IsEqualTo("color");
        await Assert.That(scene[1]).IsEqualTo(expectedColor);
        await Assert.That(scene[2]).IsEqualTo(brightness);
    }

    [Test]
    public async Task FromRGBColor_Red_CreatesCorrectColorValue()
    {
        var scene = Scene.FromRGBColor(255, 0, 0, 100);

        // Red = (255 << 16) | (0 << 8) | 0 = 16711680
        await Assert.That(scene[1]).IsEqualTo(16711680);
    }

    [Test]
    public async Task FromRGBColor_CustomColor_CreatesCorrectColorValue()
    {
        var scene = Scene.FromRGBColor(128, 64, 32, 75);

        // (128 << 16) | (64 << 8) | 32 = 8405024
        int expectedColor = (128 << 16) | (64 << 8) | 32;
        await Assert.That(scene[1]).IsEqualTo(expectedColor);
    }

    #endregion

    #region FromHSVColor Tests

    [Test]
    [Arguments(0, 100, 100)]     // Red (hue=0)
    [Arguments(120, 100, 50)]    // Green (hue=120)
    [Arguments(240, 100, 1)]     // Blue (hue=240)
    [Arguments(359, 50, 75)]     // Near-red with lower saturation
    public async Task FromHSVColor_CreatesCorrectScene(int hue, int sat, int brightness)
    {
        var scene = Scene.FromHSVColor(hue, sat, brightness);

        await Assert.That(scene.Count).IsEqualTo(4);
        await Assert.That(scene[0]).IsEqualTo("hsv");
        await Assert.That(scene[1]).IsEqualTo(hue);
        await Assert.That(scene[2]).IsEqualTo(sat);
        await Assert.That(scene[3]).IsEqualTo(brightness);
    }

    [Test]
    public async Task FromHSVColor_BoundaryValues()
    {
        var sceneMin = Scene.FromHSVColor(0, 0, 1);
        var sceneMax = Scene.FromHSVColor(359, 100, 100);

        await Assert.That(sceneMin[1]).IsEqualTo(0);
        await Assert.That(sceneMin[2]).IsEqualTo(0);
        await Assert.That(sceneMin[3]).IsEqualTo(1);

        await Assert.That(sceneMax[1]).IsEqualTo(359);
        await Assert.That(sceneMax[2]).IsEqualTo(100);
        await Assert.That(sceneMax[3]).IsEqualTo(100);
    }

    #endregion

    #region FromColorTemperature Tests

    [Test]
    [Arguments(1700, 100)]  // Warm white (minimum)
    [Arguments(4000, 50)]   // Neutral white
    [Arguments(6500, 1)]    // Cool white (maximum)
    public async Task FromColorTemperature_CreatesCorrectScene(int temperature, int brightness)
    {
        var scene = Scene.FromColorTemperature(temperature, brightness);

        await Assert.That(scene.Count).IsEqualTo(3);
        await Assert.That(scene[0]).IsEqualTo("ct");
        await Assert.That(scene[1]).IsEqualTo(temperature);
        await Assert.That(scene[2]).IsEqualTo(brightness);
    }

    [Test]
    public async Task FromColorTemperature_WarmWhite()
    {
        var scene = Scene.FromColorTemperature(2700, 80);

        await Assert.That(scene[0]).IsEqualTo("ct");
        await Assert.That(scene[1]).IsEqualTo(2700);
        await Assert.That(scene[2]).IsEqualTo(80);
    }

    #endregion

    #region FromAutoDelayOff Tests

    [Test]
    [Arguments(1, 100)]    // 1 minute delay
    [Arguments(30, 50)]    // 30 minutes delay
    [Arguments(60, 1)]     // 60 minutes delay (max)
    public async Task FromAutoDelayOff_CreatesCorrectScene(int delay, int brightness)
    {
        var scene = Scene.FromAutoDelayOff(delay, brightness);

        await Assert.That(scene.Count).IsEqualTo(3);
        await Assert.That(scene[0]).IsEqualTo("auto_delay_off");
        await Assert.That(scene[1]).IsEqualTo(brightness);
        await Assert.That(scene[2]).IsEqualTo(delay);
    }

    [Test]
    public async Task FromAutoDelayOff_OrderOfParameters()
    {
        var scene = Scene.FromAutoDelayOff(15, 75);

        // Order is: class, brightness, delay
        await Assert.That(scene[0]).IsEqualTo("auto_delay_off");
        await Assert.That(scene[1]).IsEqualTo(75); // brightness
        await Assert.That(scene[2]).IsEqualTo(15); // delay
    }

    #endregion

    #region FromColorFlow Tests

    [Test]
    public async Task FromColorFlow_CreatesCorrectScene()
    {
        var flow = new ColorFlow(3, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500)
        };

        var scene = Scene.FromColorFlow(flow);

        await Assert.That(scene.Count).IsEqualTo(4);
        await Assert.That(scene[0]).IsEqualTo("cf");
        await Assert.That(scene[1]).IsEqualTo(3); // RepetitionCount
        await Assert.That(scene[2]).IsEqualTo(0); // EndAction.Restore = 0
        await Assert.That(scene[3]).IsEqualTo("500,1,16711680,100"); // Flow expression
    }

    [Test]
    public async Task FromColorFlow_WithKeepEndAction()
    {
        var flow = new ColorFlow(0, ColorFlowEndAction.Keep)
        {
            new ColorFlowRGBExpression(0, 255, 0, 50, 1000)
        };

        var scene = Scene.FromColorFlow(flow);

        await Assert.That(scene[2]).IsEqualTo(1); // EndAction.Keep = 1
    }

    [Test]
    public async Task FromColorFlow_WithTurnOffEndAction()
    {
        var flow = new ColorFlow(1, ColorFlowEndAction.TurnOff)
        {
            new ColorFlowTemperatureExpression(4000, 100, 500)
        };

        var scene = Scene.FromColorFlow(flow);

        await Assert.That(scene[2]).IsEqualTo(2); // EndAction.TurnOff = 2
    }

    [Test]
    public async Task FromColorFlow_ComplexFlow()
    {
        var flow = new ColorFlow(2, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowSleepExpression(1000),
            new ColorFlowRGBExpression(0, 0, 255, 100, 500)
        };

        var scene = Scene.FromColorFlow(flow);

        await Assert.That(scene[0]).IsEqualTo("cf");
        await Assert.That(scene[1]).IsEqualTo(2);
        await Assert.That(scene[2]).IsEqualTo(0);

        var flowExpression = scene[3] as string;
        await Assert.That(flowExpression).IsNotNull();
        await Assert.That(flowExpression!).Contains("500,1,16711680,100");
        await Assert.That(flowExpression).Contains("1000,7,0,0");
        await Assert.That(flowExpression).Contains("500,1,255,100");
    }

    [Test]
    public async Task FromColorFlow_NullFlow_ThrowsException()
    {
        await Assert.That(() => Scene.FromColorFlow(null!)).Throws<ArgumentNullException>();
    }

    #endregion

    #region Scene List Behavior Tests

    [Test]
    public async Task Scene_IsListOfObjects()
    {
        var scene = Scene.FromRGBColor(255, 0, 0, 100);

        await Assert.That(scene).IsAssignableTo<List<object>>();
    }

    [Test]
    public async Task Scene_CanBeEnumerated()
    {
        var scene = Scene.FromHSVColor(120, 100, 50);

        var items = scene.ToArray();

        await Assert.That(items.Length).IsEqualTo(4);
    }

    [Test]
    public async Task Scene_CanAccessByIndex()
    {
        var scene = Scene.FromColorTemperature(4000, 75);

        await Assert.That(scene[0]).IsEqualTo("ct");
        await Assert.That(scene[1]).IsEqualTo(4000);
        await Assert.That(scene[2]).IsEqualTo(75);
    }

    #endregion
}
