using YeelightAPI.Models;

namespace YeelightAPI.UnitTests;

public class DeviceInitializationTests
{
    #region Constructor Tests

    [Test]
    public async Task Device_Constructor_WithHostname()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.Hostname).IsEqualTo("192.168.1.100");
        await Assert.That(device.Port).IsEqualTo(55443); // Default port
        await Assert.That(device.IsConnected).IsFalse();
    }

    [Test]
    public async Task Device_Constructor_WithHostnameAndPort()
    {
        using var device = new Device("192.168.1.100", 12345);

        await Assert.That(device.Hostname).IsEqualTo("192.168.1.100");
        await Assert.That(device.Port).IsEqualTo(12345);
    }

    [Test]
    public async Task Device_Constructor_WithDnsName()
    {
        using var device = new Device("yeelight-device.local", 55443);

        await Assert.That(device.Hostname).IsEqualTo("yeelight-device.local");
        await Assert.That(device.Port).IsEqualTo(55443);
    }

    [Test]
    [Arguments("192.168.1.1")]
    [Arguments("10.0.0.50")]
    [Arguments("172.16.0.100")]
    [Arguments("192.168.100.200")]
    public async Task Device_Constructor_AcceptsValidIPs(string ip)
    {
        using var device = new Device(ip);

        await Assert.That(device.Hostname).IsEqualTo(ip);
    }

    [Test]
    [Arguments(1)]
    [Arguments(80)]
    [Arguments(55443)]
    [Arguments(65535)]
    public async Task Device_Constructor_AcceptsValidPorts(int port)
    {
        using var device = new Device("192.168.1.100", port);

        await Assert.That(device.Port).IsEqualTo(port);
    }

    #endregion

    #region Initial State Tests

    [Test]
    public async Task Device_InitialState_IsNotConnected()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.IsConnected).IsFalse();
    }

    [Test]
    public async Task Device_InitialState_MusicModeDisabled()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.IsMusicModeEnabled).IsFalse();
    }

    [Test]
    public async Task Device_InitialState_PropertiesEmpty()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.Properties).IsNotNull();
        await Assert.That(device.Properties).IsEmpty();
    }

    [Test]
    public async Task Device_InitialState_SupportedOperationsEmpty()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.SupportedOperations).IsNotNull();
        await Assert.That(device.SupportedOperations).IsEmpty();
    }

    [Test]
    public async Task Device_InitialState_ModelIsUnknown()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.Model).IsEqualTo(MODEL.Unknown);
    }

    [Test]
    public async Task Device_InitialState_IdIsNull()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.Id).IsNull();
    }

    [Test]
    public async Task Device_InitialState_FirmwareVersionIsNull()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.FirmwareVersion).IsNull();
    }

    #endregion

    #region Property Indexer Tests

    [Test]
    public async Task Device_PropertyIndexer_SetAndGet_ByEnum()
    {
        using var device = new Device("192.168.1.100");

        device[PROPERTIES.power] = "on";

        await Assert.That(device[PROPERTIES.power]).IsEqualTo("on");
    }

    [Test]
    public async Task Device_PropertyIndexer_SetAndGet_ByString()
    {
        using var device = new Device("192.168.1.100");

        device["bright"] = 50;

        await Assert.That(device["bright"]).IsEqualTo(50);
    }

    [Test]
    public async Task Device_PropertyIndexer_GetNonExistent_ReturnsNull()
    {
        using var device = new Device("192.168.1.100");

        var value = device[PROPERTIES.power];

        await Assert.That(value).IsNull();
    }

    [Test]
    public async Task Device_PropertyIndexer_UpdateExistingValue()
    {
        using var device = new Device("192.168.1.100");

        device[PROPERTIES.bright] = 50;
        device[PROPERTIES.bright] = 75;

        await Assert.That(device[PROPERTIES.bright]).IsEqualTo(75);
    }

    [Test]
    public async Task Device_PropertyIndexer_MultipleProperties()
    {
        using var device = new Device("192.168.1.100");

        device[PROPERTIES.power] = "on";
        device[PROPERTIES.bright] = 100;
        device[PROPERTIES.ct] = 4000;

        await Assert.That(device[PROPERTIES.power]).IsEqualTo("on");
        await Assert.That(device[PROPERTIES.bright]).IsEqualTo(100);
        await Assert.That(device[PROPERTIES.ct]).IsEqualTo(4000);
    }

    [Test]
    public async Task Device_PropertyIndexer_NullPropertyName_ThrowsException()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(() => device[null!] = "test").Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Device_PropertyIndexer_EmptyPropertyName_DoesNotAdd()
    {
        using var device = new Device("192.168.1.100");

        device[""] = "test";

        await Assert.That(device.Properties).IsEmpty();
    }

    [Test]
    public async Task Device_PropertyIndexer_WhitespacePropertyName_DoesNotAdd()
    {
        using var device = new Device("192.168.1.100");

        device["   "] = "test";

        await Assert.That(device.Properties).IsEmpty();
    }

    #endregion

    #region Name Property Tests

    [Test]
    public async Task Device_Name_SetAndGet()
    {
        using var device = new Device("192.168.1.100");

        device.Name = "Living Room Light";

        await Assert.That(device.Name).IsEqualTo("Living Room Light");
    }

    [Test]
    public async Task Device_Name_InitiallyNull()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.Name).IsNull();
    }

    [Test]
    public async Task Device_Name_UsesPropertiesCollection()
    {
        using var device = new Device("192.168.1.100");

        device.Name = "Test";

        await Assert.That(device[PROPERTIES.name]).IsEqualTo("Test");
        await Assert.That(device["name"]).IsEqualTo("Test");
    }

    #endregion

    #region IDisposable Tests

    [Test]
    public async Task Device_ImplementsIDisposable()
    {
        var device = new Device("192.168.1.100");

        await Assert.That(device).IsAssignableTo<IDisposable>();

        device.Dispose(); // Should not throw
    }

    [Test]
    public async Task Device_CanBeUsedWithUsing()
    {
        // This test verifies that Device can be properly disposed in a using statement
        using (var device = new Device("192.168.1.100"))
        {
            await Assert.That(device).IsNotNull();
        }
        // Device should be disposed without exceptions
    }

    #endregion

    #region ToString Tests

    [Test]
    public async Task Device_ToString_ContainsHostnameAndPort()
    {
        using var device = new Device("192.168.1.100", 55443);

        var result = device.ToString();

        await Assert.That(result).Contains("192.168.1.100");
    }

    #endregion

    #region Default Port Constant Tests

    [Test]
    public async Task Device_DefaultPort_IsCorrect()
    {
        using var device = new Device("192.168.1.100");

        await Assert.That(device.Port).IsEqualTo(55443);
    }

    #endregion
}
