using YeelightAPI.Core;

namespace YeelightAPI.UnitTests;

public class ConstantsTests
{
    #region Default Values Tests

    [Test]
    public async Task DefaultPort_Is55443()
    {
        await Assert.That(Constants.DefaultPort).IsEqualTo(55443);
    }

    [Test]
    public async Task DefaultTimeout_Is5000ms()
    {
        await Assert.That(Constants.DefaultTimeout).IsEqualTo(5000);
    }

    [Test]
    public async Task LineSeparator_IsCRLF()
    {
        await Assert.That(Constants.LineSeparator).IsEqualTo("\r\n");
    }

    [Test]
    public async Task MinimumFlowExpressionDuration_Is50ms()
    {
        await Assert.That(Constants.MinimumFlowExpressionDuration).IsEqualTo(50);
    }

    [Test]
    public async Task MinimumSmoothDuration_Is30ms()
    {
        await Assert.That(Constants.MinimumSmoothDuration).IsEqualTo(30);
    }

    [Test]
    public async Task Off_IsCorrectString()
    {
        await Assert.That(Constants.Off).IsEqualTo("off");
    }

    [Test]
    public async Task On_IsCorrectString()
    {
        await Assert.That(Constants.On).IsEqualTo("on");
    }

    #endregion

    #region Serializer Options Tests

    [Test]
    public async Task DeviceSerializerOptions_IsNotNull()
    {
        await Assert.That(Constants.DeviceSerializerOptions).IsNotNull();
    }

    [Test]
    public async Task DeviceSerializerOptions_UsesCamelCase()
    {
        var options = Constants.DeviceSerializerOptions;

        await Assert.That(options.PropertyNamingPolicy).IsNotNull();
        await Assert.That(options.PropertyNamingPolicy).IsEqualTo(System.Text.Json.JsonNamingPolicy.CamelCase);
    }

    [Test]
    public async Task DeviceSerializerOptions_IsCaseInsensitive()
    {
        var options = Constants.DeviceSerializerOptions;

        await Assert.That(options.PropertyNameCaseInsensitive).IsTrue();
    }

    #endregion
}
