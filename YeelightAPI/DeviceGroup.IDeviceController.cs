using System.Threading;
using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;
using YeelightAPI.Models.Scene;

namespace YeelightAPI
{
    /// <summary>
    /// Group of Yeelight Devices : IDeviceController implementation
    /// </summary>
    public partial class DeviceGroup : IDeviceController
    {
        #region Public Methods

        /// <summary>
        /// Adjusts the brightness
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AdjustBrightAsync(int percent, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.AdjustBrightAsync(percent, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AdjustColorAsync(int percent, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.AdjustColorAsync(percent, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> AdjustColorTemperatureAsync(int percent, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.AdjustColorTemperatureAsync(percent, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Connect all the devices
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.ConnectAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Add a cron task for all devices
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CronAddAsync(int value, CronType type = CronType.PowerOff, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.CronAddAsync(value, type, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Delete a cron task for all devices
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> CronDeleteAsync(CronType type = CronType.PowerOff, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.CronDeleteAsync(type, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Disconnect all the devices
        /// </summary>
        public void Disconnect()
        {
            foreach (Device device in this)
            {
                device.Disconnect();
            }
        }

        /// <summary>
        /// Initiate a new Color Flow
        /// </summary>
        /// <returns></returns>
        public FluentFlow Flow()
        {
            return new FluentFlow(this, StartColorFlowAsync, StopColorFlowAsync);
        }

        /// <summary>
        /// Adjusts the state of all the devices
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetAdjustAsync(AdjustAction action, AdjustProperty property, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetAdjustAsync(action, property, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set the brightness for all the devices
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetBrightnessAsync(int value, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetBrightnessAsync(value, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set the color temperature for all the devices
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetColorTemperatureAsync(int temperature, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetColorTemperatureAsync(temperature, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set the current state as the default one
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetDefaultAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Change HSV color asynchronously for all devices
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetHSVColorAsync(int hue, int sat, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetHSVColorAsync(hue, sat, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set the power for all the devices
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetPowerAsync(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetPowerAsync(state, smooth, mode, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set the RGB Color for all the devices
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetRGBColorAsync(r, g, b, smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Set a scene for all the devices
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetSceneAsync(Scene scene, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.SetSceneAsync(scene, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Starts a color flow for all devices
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> StartColorFlowAsync(ColorFlow flow, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.StartColorFlowAsync(flow, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// starts the music mode for all devices
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> StartMusicModeAsync(string hostName, int port, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.StartMusicModeAsync(hostName, port, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// stops the color flow of all devices
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> StopColorFlowAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.StopColorFlowAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// stops the music mode for all devices
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> StopMusicModeAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.StopMusicModeAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Toggle the power for all the devices
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> ToggleAsync(CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.ToggleAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Turn-Off the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> TurnOffAsync(int? smooth = null, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.TurnOffAsync(smooth, cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// Turn-On the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> TurnOnAsync(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default)
        {
            return await Process((Device device) =>
            {
                return device.TurnOnAsync(smooth, mode, cancellationToken);
            }, cancellationToken);
        }

        #endregion Public Methods
    }
}
