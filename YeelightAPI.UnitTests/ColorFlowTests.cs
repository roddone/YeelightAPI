using YeelightAPI.Models.ColorFlow;

namespace YeelightAPI.UnitTests;

public class ColorFlowTests
{
    #region ColorFlowExpression Tests

    [Test]
    public async Task ColorFlowExpression_GetFlow_ReturnsCorrectList()
    {
        var expression = new ColorFlowExpression
        {
            Duration = 500,
            Mode = ColorFlowMode.Color,
            Value = 16711680,
            Brightness = 100
        };

        var flow = expression.GetFlow();

        await Assert.That(flow.Count).IsEqualTo(4);
        await Assert.That(flow[0]).IsEqualTo(500);
        await Assert.That(flow[1]).IsEqualTo(1); // ColorFlowMode.Color = 1
        await Assert.That(flow[2]).IsEqualTo(16711680);
        await Assert.That(flow[3]).IsEqualTo(100);
    }

    [Test]
    public async Task ColorFlowExpression_GetFlow_WithSleepMode()
    {
        var expression = new ColorFlowExpression
        {
            Duration = 1000,
            Mode = ColorFlowMode.Sleep,
            Value = 0,
            Brightness = 0
        };

        var flow = expression.GetFlow();

        await Assert.That(flow[0]).IsEqualTo(1000);
        await Assert.That(flow[1]).IsEqualTo(7); // ColorFlowMode.Sleep = 7
        await Assert.That(flow[2]).IsEqualTo(0);
        await Assert.That(flow[3]).IsEqualTo(0);
    }

    [Test]
    public async Task ColorFlowExpression_GetFlow_WithTemperatureMode()
    {
        var expression = new ColorFlowExpression
        {
            Duration = 200,
            Mode = ColorFlowMode.ColorTemperature,
            Value = 4000,
            Brightness = 50
        };

        var flow = expression.GetFlow();

        await Assert.That(flow[0]).IsEqualTo(200);
        await Assert.That(flow[1]).IsEqualTo(2); // ColorFlowMode.ColorTemperature = 2
        await Assert.That(flow[2]).IsEqualTo(4000);
        await Assert.That(flow[3]).IsEqualTo(50);
    }

    #endregion

    #region ColorFlowRGBExpression Tests

    [Test]
    [Arguments(255, 0, 0, 16711680)]   // Red
    [Arguments(0, 255, 0, 65280)]      // Green
    [Arguments(0, 0, 255, 255)]        // Blue
    [Arguments(255, 255, 255, 16777215)] // White
    [Arguments(0, 0, 0, 0)]            // Black
    [Arguments(128, 128, 128, 8421504)] // Gray
    [Arguments(255, 128, 0, 16744448)] // Orange
    public async Task ColorFlowRGBExpression_ComputesCorrectRGBValue(int r, int g, int b, int expectedValue)
    {
        var expression = new ColorFlowRGBExpression(r, g, b, 100, 500);

        await Assert.That(expression.Value).IsEqualTo(expectedValue);
        await Assert.That(expression.Mode).IsEqualTo(ColorFlowMode.Color);
        await Assert.That(expression.Brightness).IsEqualTo(100);
        await Assert.That(expression.Duration).IsEqualTo(500);
    }

    [Test]
    public async Task ColorFlowRGBExpression_UsesDefaultDuration()
    {
        var expression = new ColorFlowRGBExpression(255, 0, 0, 100);

        await Assert.That(expression.Duration).IsEqualTo(50); // Constants.MinimumFlowExpressionDuration
    }

    [Test]
    public async Task ColorFlowRGBExpression_GetFlow_ReturnsCorrectValues()
    {
        var expression = new ColorFlowRGBExpression(255, 0, 0, 50, 1000);
        var flow = expression.GetFlow();

        await Assert.That(flow[0]).IsEqualTo(1000);
        await Assert.That(flow[1]).IsEqualTo(1); // Color mode
        await Assert.That(flow[2]).IsEqualTo(16711680); // Red color value
        await Assert.That(flow[3]).IsEqualTo(50);
    }

    #endregion

    #region ColorFlowTemperatureExpression Tests

    [Test]
    public async Task ColorFlowTemperatureExpression_SetsCorrectValues()
    {
        var expression = new ColorFlowTemperatureExpression(4000, 80, 500);

        await Assert.That(expression.Value).IsEqualTo(4000);
        await Assert.That(expression.Mode).IsEqualTo(ColorFlowMode.ColorTemperature);
        await Assert.That(expression.Brightness).IsEqualTo(80);
        await Assert.That(expression.Duration).IsEqualTo(500);
    }

    [Test]
    public async Task ColorFlowTemperatureExpression_UsesDefaultDuration()
    {
        var expression = new ColorFlowTemperatureExpression(6500, 100);

        await Assert.That(expression.Duration).IsEqualTo(50);
    }

    [Test]
    [Arguments(1700, 1, 100)]   // Min temp, min brightness
    [Arguments(6500, 100, 50)]  // Max temp, max brightness
    [Arguments(4000, 50, 200)]  // Mid values
    public async Task ColorFlowTemperatureExpression_GetFlow_ReturnsCorrectValues(int temp, int brightness, int duration)
    {
        var expression = new ColorFlowTemperatureExpression(temp, brightness, duration);
        var flow = expression.GetFlow();

        await Assert.That(flow[0]).IsEqualTo(duration);
        await Assert.That(flow[1]).IsEqualTo(2); // ColorTemperature mode
        await Assert.That(flow[2]).IsEqualTo(temp);
        await Assert.That(flow[3]).IsEqualTo(brightness);
    }

    #endregion

    #region ColorFlowSleepExpression Tests

    [Test]
    public async Task ColorFlowSleepExpression_SetsCorrectDefaults()
    {
        var expression = new ColorFlowSleepExpression(2000);

        await Assert.That(expression.Duration).IsEqualTo(2000);
        await Assert.That(expression.Mode).IsEqualTo(ColorFlowMode.Sleep);
        await Assert.That(expression.Value).IsEqualTo(0);
        await Assert.That(expression.Brightness).IsEqualTo(0);
    }

    [Test]
    public async Task ColorFlowSleepExpression_UsesDefaultDuration()
    {
        var expression = new ColorFlowSleepExpression();

        await Assert.That(expression.Duration).IsEqualTo(50);
    }

    [Test]
    public async Task ColorFlowSleepExpression_GetFlow_ReturnsCorrectValues()
    {
        var expression = new ColorFlowSleepExpression(3000);
        var flow = expression.GetFlow();

        await Assert.That(flow[0]).IsEqualTo(3000);
        await Assert.That(flow[1]).IsEqualTo(7); // Sleep mode
        await Assert.That(flow[2]).IsEqualTo(0);
        await Assert.That(flow[3]).IsEqualTo(0);
    }

    #endregion

    #region ColorFlow Tests

    [Test]
    public async Task ColorFlow_Constructor_SetsProperties()
    {
        var flow = new ColorFlow(5, ColorFlowEndAction.Keep);

        await Assert.That(flow.RepetitionCount).IsEqualTo(5);
        await Assert.That(flow.EndAction).IsEqualTo(ColorFlowEndAction.Keep);
        await Assert.That(flow).IsEmpty();
    }

    [Test]
    public async Task ColorFlow_Constructor_WithExpressions()
    {
        var expressions = new ColorFlowExpression[]
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowSleepExpression(1000),
            new ColorFlowTemperatureExpression(4000, 50, 500)
        };

        var flow = new ColorFlow(0, ColorFlowEndAction.Restore, expressions);

        await Assert.That(flow.Count).IsEqualTo(3);
        await Assert.That(flow.RepetitionCount).IsEqualTo(0);
        await Assert.That(flow.EndAction).IsEqualTo(ColorFlowEndAction.Restore);
    }

    [Test]
    public async Task ColorFlow_GetColorFlowExpression_SingleExpression()
    {
        var flow = new ColorFlow(0, ColorFlowEndAction.Keep)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500)
        };

        var expression = flow.GetColorFlowExpression();

        await Assert.That(expression).IsEqualTo("500,1,16711680,100");
    }

    [Test]
    public async Task ColorFlow_GetColorFlowExpression_MultipleExpressions()
    {
        var flow = new ColorFlow(0, ColorFlowEndAction.Keep)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowSleepExpression(1000),
            new ColorFlowRGBExpression(0, 255, 0, 100, 500)
        };

        var expression = flow.GetColorFlowExpression();

        await Assert.That(expression).IsEqualTo("500,1,16711680,100,1000,7,0,0,500,1,65280,100");
    }

    [Test]
    public async Task ColorFlow_GetColorFlowExpression_Empty()
    {
        var flow = new ColorFlow(0, ColorFlowEndAction.Keep);

        var expression = flow.GetColorFlowExpression();

        await Assert.That(expression).IsEqualTo("");
    }

    [Test]
    [Arguments(ColorFlowEndAction.Restore, 0)]
    [Arguments(ColorFlowEndAction.Keep, 1)]
    [Arguments(ColorFlowEndAction.TurnOff, 2)]
    public async Task ColorFlowEndAction_HasCorrectValues(ColorFlowEndAction action, int expected)
    {
        await Assert.That((int)action).IsEqualTo(expected);
    }

    [Test]
    [Arguments(ColorFlowMode.Color, 1)]
    [Arguments(ColorFlowMode.ColorTemperature, 2)]
    [Arguments(ColorFlowMode.Sleep, 7)]
    public async Task ColorFlowMode_HasCorrectValues(ColorFlowMode mode, int expected)
    {
        await Assert.That((int)mode).IsEqualTo(expected);
    }

    #endregion

    #region Complex Color Flow Scenarios

    [Test]
    public async Task ColorFlow_RGBRainbow_GeneratesCorrectExpression()
    {
        var flow = new ColorFlow(3, ColorFlowEndAction.Restore)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 1000),   // Red
            new ColorFlowRGBExpression(255, 127, 0, 100, 1000), // Orange
            new ColorFlowRGBExpression(255, 255, 0, 100, 1000), // Yellow
            new ColorFlowRGBExpression(0, 255, 0, 100, 1000),   // Green
            new ColorFlowRGBExpression(0, 0, 255, 100, 1000),   // Blue
            new ColorFlowRGBExpression(139, 0, 255, 100, 1000)  // Violet
        };

        var expression = flow.GetColorFlowExpression();
        var parts = expression.Split(',');

        await Assert.That(parts.Length).IsEqualTo(24); // 6 expressions * 4 values each
        await Assert.That(flow.RepetitionCount).IsEqualTo(3);
    }

    [Test]
    public async Task ColorFlow_FadeWithSleep_GeneratesCorrectExpression()
    {
        var flow = new ColorFlow(0, ColorFlowEndAction.Keep)
        {
            new ColorFlowRGBExpression(255, 0, 0, 100, 500),
            new ColorFlowSleepExpression(2000),
            new ColorFlowRGBExpression(255, 0, 0, 1, 500)
        };

        var expression = flow.GetColorFlowExpression();

        await Assert.That(expression).Contains("2000,7,0,0"); // Sleep expression
    }

    #endregion
}
