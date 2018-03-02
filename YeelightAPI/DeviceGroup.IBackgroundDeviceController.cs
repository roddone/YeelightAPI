using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Scene;

namespace YeelightAPI
{
    /// <summary>
    /// Group of Yeelight Devices : IBackgroundDeviceController implementation
    /// </summary>
    public partial class DeviceGroup : IBackgroundDeviceController
    {
        #region Public Methods

        /// <summary>
        /// Initiate a background flow on all devices
        /// </summary>
        /// <returns></returns>
        public FluentFlow BackgroundFLow()
        {
            return new FluentFlow(this);
        }

        /// <summary>
        /// Adjusts background light
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetAdjust(AdjustAction action, AdjustProperty property)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetAdjust(action, property);
            });
        }

        /// <summary>
        /// Set background light color
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetBrightness(int value, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetBrightness(value, smooth);
            });
        }

        /// <summary>
        /// Set background light temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetColorTemperature(int temperature, int? smooth)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetColorTemperature(temperature, smooth);
            });
        }

        /// <summary>
        /// Set the background current state as the default one
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BackgroundSetDefault()
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetDefault();
            });
        }

        /// <summary>
        /// Set background light HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetHSVColor(int hue, int sat, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetHSVColor(hue, sat, smooth);
            });
        }

        /// <summary>
        /// Set background light power
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetPower(bool state = true)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetPower(state);
            });
        }

        /// <summary>
        /// Set background light RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetRGBColor(int r, int g, int b, int? smooth)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetRGBColor(r, g, b, smooth);
            });
        }

        /// <summary>
        /// Set background scene
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetScene(Scene scene)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetScene(scene);
            });
        }

        /// <summary>
        /// Starts a color flow
        /// </summary>
        /// <param name="flow"></param>
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

        /// <summary>
        /// Toggle background light
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BackgroundToggle()
        {
            return await Process((Device device) =>
            {
                return device.BackgroundToggle();
            });
        }

        /// <summary>
        /// Toggle Both Background and normal light
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DevToggle()
        {
            return await Process((Device device) =>
            {
                return device.DevToggle();
            });
        }

        #endregion Public Methods
    }
}