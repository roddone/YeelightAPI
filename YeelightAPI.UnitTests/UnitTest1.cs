using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using Xunit;

namespace YeelightAPI.UnitTests
{
    public class UnitTest1
    {
        private IConfigurationRoot _config;
        public UnitTest1()
        {
            _config = new ConfigurationBuilder()
             .AddJsonFile("config.json")
             .Build();
        }

        [Fact]
        public async void Discovery_should_find_devices()
        {
            int expectedDevicesCount = GetConfig<int>("discovery_devices_expected");
            var devices = await DeviceLocator.Discover();

            Assert.True(devices?.Count != expectedDevicesCount);
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
            catch(Exception ex)
            {
                throw new Exception($"Cannot convert '{value}' (key: {key}) to {t}", ex);
            } 
        }
    }
}
