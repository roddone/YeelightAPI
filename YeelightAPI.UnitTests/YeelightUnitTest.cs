using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using YeelightAPI.Models;
using System.Diagnostics;

namespace YeelightAPI.UnitTests
{
    public class YeelightUnitTest
    {
        private readonly IConfigurationRoot _config;
        private readonly Xunit.Abstractions.ITestOutputHelper _output;

        public YeelightUnitTest(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._config = new ConfigurationBuilder()
             .AddJsonFile("config.json")
             .Build();

            this._output = testOutputHelper;
        }

        #region TESTS

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

        [Fact]
        public async Task Device_should_turnon_and_turnoff()
        {
            Device testedDevice = await GetRandomConnectedDevice();
            await testedDevice.TurnOn();
            Assert.Equal("on", await testedDevice.GetProp(PROPERTIES.power));
            await testedDevice.TurnOff();
            Assert.Equal("off", await testedDevice.GetProp(PROPERTIES.power));
        }

        [Fact]
        public async Task Device_should_change_rgb_color_to_red() => await DoWithRandomDevice(async (device) =>
        {
            await device.SetRGBColor(255, 0, 0);
            Assert.Equal((255 << 16).ToString(), await device.GetProp(PROPERTIES.rgb));
        }, METHODS.SetRGBColor);

        [Fact]
        public async Task Device_should_change_hsv_color_to_red() => await DoWithRandomDevice(async (device) =>
        {
            await device.SetHSVColor(0, 100);
            Assert.Equal((255 << 16).ToString(), await device.GetProp(PROPERTIES.rgb));

        }, METHODS.SetHSVColor);

        [Fact]
        public async Task Device_should_change_brightness() => await DoWithRandomDevice(async (device) =>
        {
            await device.SetBrightness(52);
            Assert.Equal(52, await device.GetProp(PROPERTIES.bright));

        }, METHODS.SetBrightness);

        [Fact]
        public async Task Device_should_change_colortemperature() => await DoWithRandomDevice(async (device) =>
        {
            await device.SetColorTemperature(4654);
            Assert.Equal(4654, await device.GetProp(PROPERTIES.ct));

        }, METHODS.SetBrightness);

        #endregion TESTS

        #region PRIVATE METHODS

        private async Task DoWithRandomDevice(Action<Device> a, METHODS? supportedMethod = null)
        {
            Device testedDevice = await GetRandomConnectedDevice(supportedMethod);
            await testedDevice.TurnOn();

            a?.Invoke(testedDevice);

            await testedDevice.TurnOff();
        }

        private async Task<Device> GetRandomConnectedDevice(METHODS? supportedMethod = null)
        {
            List<Device> devices = (await DeviceLocator.DiscoverAsync()).Where(d => !supportedMethod.HasValue || d.SupportedOperations.Contains(supportedMethod.Value)).ToList() ;

            Assert.NotEmpty(devices);

            int randomIndex = new Random().Next(0, devices.Count);
            Device d = devices.ElementAt(randomIndex);
            _output.WriteLine($"Used device : {d}");
            await d.Connect();
            return d;
        }

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
