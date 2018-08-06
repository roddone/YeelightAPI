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
        /// <returns></returns>
        public async Task<bool> AdjustBright(int percent, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.AdjustBright(percent, smooth);
            });
        }

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> AdjustColor(int percent, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.AdjustColor(percent, smooth);
            });
        }

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> AdjustColorTemperature(int percent, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.AdjustColorTemperature(percent, smooth);
            });
        }

        /// <summary>
        /// Connect all the devices
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Connect()
        {
            return await Process((Device device) =>
            {
                return device.Connect();
            });
        }

        /// <summary>
        /// Add a cron task for all devices
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> CronAdd(int value, CronType type = CronType.PowerOff)
        {
            return await Process((Device device) =>
            {
                return device.CronAdd(value, type);
            });
        }

        /// <summary>
        /// Delete a cron task for all devices
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<bool> CronDelete(CronType type = CronType.PowerOff)
        {
            return await Process((Device device) =>
            {
                return device.CronDelete(type);
            });
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
            return new FluentFlow(this, StartColorFlow, StopColorFlow);
        }

        /// <summary>
        /// Adjusts the state of all the devices
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public async Task<bool> SetAdjust(AdjustAction action, AdjustProperty property)
        {
            return await Process((Device device) =>
            {
                return device.SetAdjust(action, property);
            });
        }

        /// <summary>
        /// Set the brightness for all the devices
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetBrightness(int value, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.SetBrightness(value, smooth);
            });
        }

        /// <summary>
        /// Set the color temperature for all the devices
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetColorTemperature(int temperature, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.SetColorTemperature(temperature, smooth);
            });
        }

        /// <summary>
        /// Set the current state as the default one
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SetDefault()
        {
            return await Process((Device device) =>
            {
                return device.SetDefault();
            });
        }

        /// <summary>
        /// Change HSV color asynchronously for all devices
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetHSVColor(int hue, int sat, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.SetHSVColor(hue, sat, smooth);
            });
        }

        /// <summary>
        /// Set the power for all the devices
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<bool> SetPower(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal)
        {
            return await Process((Device device) =>
            {
                return device.SetPower(state, smooth, mode);
            });
        }

        /// <summary>
        /// Set the RGB Color for all the devices
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetRGBColor(int r, int g, int b, int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.SetRGBColor(r, g, b, smooth);
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public async Task<bool> SetScene(Scene scene)
        {
            return await Process((Device device) =>
            {
                return device.SetScene(scene);
            });
        }

        /// <summary>
        /// Starts a color flow for all devices
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public async Task<bool> StartColorFlow(ColorFlow flow)
        {
            return await Process((Device device) =>
            {
                return device.StartColorFlow(flow);
            });
        }

        /// <summary>
        /// starts the music mode for all devices
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task<bool> StartMusicMode(string hostName, int port)
        {
            return await Process((Device device) =>
            {
                return device.StartMusicMode(hostName, port);
            });
        }

        /// <summary>
        /// stops the color flow of all devices
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopColorFlow()
        {
            return await Process((Device device) =>
            {
                return device.StopColorFlow();
            });
        }

        /// <summary>
        /// stops the music mode for all devices
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopMusicMode()
        {
            return await Process((Device device) =>
            {
                return device.StopMusicMode();
            });
        }

        /// <summary>
        /// Toggle the power for all the devices
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Toggle()
        {
            return await Process((Device device) =>
            {
                return device.Toggle();
            });
        }

        /// <summary>
        /// Turn-Off the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> TurnOff(int? smooth = null)
        {
            return await Process((Device device) =>
            {
                return device.TurnOff(smooth);
            });
        }

        /// <summary>
        /// Turn-On the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task<bool> TurnOn(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal)
        {
            return await Process((Device device) =>
            {
                return device.TurnOn(smooth, mode);
            });
        }

        #endregion Public Methods
    }
}