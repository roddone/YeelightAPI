using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using YeelightAPI.Models;

namespace YeelightAPI.UnitTests
{
    public class DeviceTests
    {

        private readonly Xunit.Abstractions.ITestOutputHelper _output;

        public DeviceTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._output = testOutputHelper;
        }

        #region TESTS

        [Fact]
        public async Task Device_should_turnon_and_turnoff()
        {
            using (Device testedDevice = await GetRandomConnectedDevice())
            {
                await testedDevice.TurnOn();
                Assert.Equal("on", await testedDevice.GetProp(PROPERTIES.power));
                await testedDevice.TurnOff();
                Assert.Equal("off", await testedDevice.GetProp(PROPERTIES.power));
            }
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

        #region PRIVATE

        private async Task DoWithRandomDevice(Action<Device> a, METHODS? supportedMethod = null)
        {
            using (Device testedDevice = await GetRandomConnectedDevice(supportedMethod))
            {
                await testedDevice.TurnOn();

                a?.Invoke(testedDevice);

                await testedDevice.TurnOff();
            }
        }

        private async Task<Device> GetRandomConnectedDevice(METHODS? supportedMethod = null)
        {
            List<Device> devices = (await DeviceLocator.DiscoverAsync()).Where(d => !supportedMethod.HasValue || d.SupportedOperations.Contains(supportedMethod.Value)).ToList();

            Assert.NotEmpty(devices);

            int randomIndex = new Random().Next(0, devices.Count);
            Device d = devices.ElementAt(randomIndex);
            _output.WriteLine($"Used device : {d}");
            await d.Connect();
            return d;
        }

        #endregion PRIVATE
    }
}
