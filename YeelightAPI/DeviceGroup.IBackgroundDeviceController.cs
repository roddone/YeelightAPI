using System.Threading;
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
        /// Adjusts the brightness
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundAdjustBrightAsync(int percent, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundAdjustBrightAsync(percent, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundAdjustColorAsync(int percent, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundAdjustColorAsync(percent, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundAdjustColorTemperatureAsync(int percent, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundAdjustColorTemperatureAsync(percent, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Initiate a new background Color Flow
        /// </summary>
        /// <returns></returns>
        public FluentFlow BackgroundFlow()
        {
            return new FluentFlow(this, BackgroundStartColorFlowAsync, BackgroundStopColorFlowAsync);
        }

        /// <summary>
        /// Adjusts background light
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetAdjustAsync(AdjustAction action, AdjustProperty property, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetAdjustAsync(action, property, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set background light color
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetBrightnessAsync(int value, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetBrightnessAsync(value, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set background light temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetColorTemperatureAsync(int temperature, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetColorTemperatureAsync(temperature, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set the background current state as the default one
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetDefaultAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set background light HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetHSVColorAsync(int hue, int sat, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetHSVColorAsync(hue, sat, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set background light power
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetPowerAsync(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetPowerAsync(state, smooth, mode, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set background light RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetRGBColorAsync(int r, int g, int b, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetRGBColorAsync(r, g, b, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set background scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundSetSceneAsync(Scene scene, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundSetSceneAsync(scene, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Starts a color flow
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundStartColorFlowAsync(ColorFlow flow, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundStartColorFlowAsync(flow, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Stops the color flow
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundStopColorFlowAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundStopColorFlowAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Toggle background light
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundToggleAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundToggleAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Turn-Off the devices background light
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundTurnOffAsync(int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundTurnOffAsync(smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Turn-On the devices background light
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundTurnOnAsync(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.BackgroundTurnOnAsync(smooth, mode, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Toggle Both Background and normal light
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> DevToggleAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.DevToggleAsync(cancellationToken);
            }, cancellationToken);
        }

        #endregion Public Methods
    }
}
