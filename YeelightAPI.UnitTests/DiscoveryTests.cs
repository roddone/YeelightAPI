using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Diagnostics;

namespace YeelightAPI.UnitTests;

/// <summary>
/// Discovery tests that require actual Yeelight devices on the network.
/// </summary>
[NotInParallel("Integration Tests")]
[Retry(3)] // Retry on quota exceeded errors
public class DiscoveryTests
{
    private readonly IConfigurationRoot _config;

    public DiscoveryTests()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("config.json")
            .Build();
    }

    #region TESTS

    [Test]
    public async Task Discovery_enumerate_async_should_find_devices()
    {
        int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
        int count = 0;
        await foreach (Device device in DeviceLocator.EnumerateDevicesAsync())
        {
            ++count;
        }

        await Assert.That(count).IsEqualTo(expectedDevicesCount);
    }

    [Test]
    public async Task Discovery_should_find_devices()
    {
        int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
        var devices = (await DeviceLocator.DiscoverAsync()).ToList();

        await Assert.That(devices?.Count).IsEqualTo(expectedDevicesCount);
    }

    [Test]
    public async Task Discovery_should_find_devices_with_multiple_tries()
    {
        int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");

        DeviceLocator.MaxRetryCount = 3;
        try
        {
            var devices = (await DeviceLocator.DiscoverAsync()).ToList();
            await Assert.That(devices?.Count).IsEqualTo(expectedDevicesCount);
        }
        finally
        {
            DeviceLocator.MaxRetryCount = 1;
        }
    }

    [Test]
    public async Task Discovery_should_throw_when_using_wrong_multicast_address()
    {
        string initialMulticastAddress = DeviceLocator.DefaultMulticastIPAddress;
        bool initialUseAllAddresses = DeviceLocator.UseAllAvailableMulticastAddresses;

        try
        {
            DeviceLocator.UseAllAvailableMulticastAddresses = false;
            DeviceLocator.DefaultMulticastIPAddress = "foo";

            await Assert.That(async () =>
            {
                _ = (await DeviceLocator.DiscoverAsync()).ToList();
            }).Throws<FormatException>();
        }
        finally
        {
            DeviceLocator.DefaultMulticastIPAddress = initialMulticastAddress;
            DeviceLocator.UseAllAvailableMulticastAddresses = initialUseAllAddresses;
        }
    }

    [Test]
    public async Task Discovery_should_find_devices_on_all_multicast_addresses()
    {
        int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
        bool initialValue = DeviceLocator.UseAllAvailableMulticastAddresses;

        try
        {
            DeviceLocator.UseAllAvailableMulticastAddresses = true;
            var devices = (await DeviceLocator.DiscoverAsync()).ToList();
            await Assert.That(devices?.Count).IsEqualTo(expectedDevicesCount);
        }
        finally
        {
            DeviceLocator.UseAllAvailableMulticastAddresses = initialValue;
        }
    }

    [Test]
    public async Task Discovery_should_not_last_long()
    {
        Stopwatch sw = Stopwatch.StartNew();
        _ = await DeviceLocator.DiscoverAsync();
        sw.Stop();

        await Assert.That(sw.ElapsedMilliseconds).IsBetween(0, 1500);
    }

    #endregion TESTS

    #region PRIVATE METHODS

    private T GetConfig<T>(string key)
    {
        Type t = typeof(T);
        var value = _config[key];

        TypeConverter converter = TypeDescriptor.GetConverter(t);
        try
        {
            return (T)converter.ConvertFromString(value!)!;
        }
        catch (Exception ex)
        {
            throw new Exception($"Cannot convert '{value}' (key: {key}) to {t}", ex);
        }
    }

    #endregion PRIVATE METHODS
}
