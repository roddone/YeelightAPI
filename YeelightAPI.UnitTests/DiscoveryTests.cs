using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Diagnostics;

namespace YeelightAPI.UnitTests
{
    public class DiscoveryTests
    {
        private readonly IConfigurationRoot _config;

        public DiscoveryTests()
        {
            this._config = new ConfigurationBuilder()
             .AddJsonFile("config.json")
             .Build();
        }

        #region TESTS

        [Fact]
        public async Task Discovery_enumerate_async_should_find_devices()
        {
            int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
            int count = 0;
            await foreach(var device in DeviceLocator.DiscoverAndEnumerateAsync())
            {
                ++count;
            }

            Assert.Equal(expectedDevicesCount, count);
        }

        [Fact]
        public async Task Discovery_should_find_devices()
        {
            int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
            var devices = (await DeviceLocator.DiscoverAsync()).ToList();

            Assert.Equal(expectedDevicesCount, devices?.Count);
        }

        [Fact]
        public async Task Discovery_should_throw_when_using_wrong_multicast_address()
        {
            int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
            string initialMulticastAddress = DeviceLocator.DefaultMulticastIPAddress;
            DeviceLocator.DefaultMulticastIPAddress = "foo";

            _ = await Assert.ThrowsAsync<FormatException>(async () =>
              {
                  _ = (await DeviceLocator.DiscoverAsync()).ToList();
              });

            //reset to default to ensure next tests don't crash
            DeviceLocator.DefaultMulticastIPAddress = initialMulticastAddress;
        }

        [Fact]
        public async Task Discovery_should_find_devices_on_all_multicast_addresses()
        {
            int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
            DeviceLocator.UseAllAvailableMulticastAddresses = true;
            var devices = (await DeviceLocator.DiscoverAsync()).ToList();

            Assert.Equal(expectedDevicesCount, devices?.Count);
        }

        [Fact]
        public async Task Discovery_obsolete_should_not_last_long()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _ = await DeviceLocator.Discover();
            sw.Stop();

            Assert.InRange(sw.ElapsedMilliseconds, 0, 1500);
        }

        [Fact]
        public async Task Discovery_should_not_last_long()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _ = await DeviceLocator.DiscoverAsync();
            sw.Stop();

            Assert.InRange(sw.ElapsedMilliseconds, 0, 1500);
        }

        #endregion TESTS

        #region PRIVATE METHODS

        private T GetConfig<T>(string key)
        {
            Type t = typeof(T);
            var value = _config[key];

            TypeConverter converter = TypeDescriptor.GetConverter(t);
            try
            //if (value != null && converter.CanConvertTo(t) && converter.CanConvertFrom(typeof(string)))
            {
                return (T)converter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot convert '{value}' (key: {key}) to {t}", ex);
            }
        }

        #endregion PRIVATE METHODS
    }
}
