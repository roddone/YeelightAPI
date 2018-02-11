using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;

namespace YeelightAPI
{
    public partial class DeviceGroup : IBackgroundDeviceController
    {

        public async Task<bool> BackgroundSetBrightness(int value, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetBrightness(value, smooth);
            });

        }

        public async Task<bool> BackgroundSetColorTemperature(int temperature, int? smooth)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetColorTemperature(temperature, smooth);
            });
        }

        public async Task<bool> BackgroundSetPower(bool state = true)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetPower(state);
            });
        }

        public async Task<bool> BackgroundSetRGBColor(int r, int g, int b, int? smooth)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetRGBColor(r, g, b, smooth);
            });

        }

        public async Task<bool> BackgroundToggle()
        {
            return await Process((Device device) =>
            {
                return device.BackgroundToggle();
            });
            
        }

        /// <summary>
        /// Starts a color flow asynchronously
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundStartColorFlow(ColorFlow flow)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundStartColorFlow(flow);
            });
        }

        /// <summary>
        /// Stops the color flow
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BackgroundStopColorFlow()
        {
            return await Process((Device device) =>
            {
                return device.BackgroundStopColorFlow();
            });
            
        }

        public async Task<bool> BackgroundAdjust(AdjustAction action, AdjustProperty property)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundAdjust(action, property);
            });
        }
    }
}
