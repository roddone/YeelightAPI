using System.Threading.Tasks;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;

namespace YeelightAPI
{
    /// <summary>
    /// Group of Yeelight Devices : IDeviceController implementation
    /// </summary>
    public partial class DeviceGroup : IDeviceController
    {
        #region Public Methods

        /// <summary>
        /// Connect all the devices asynchronously
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
        /// Set the brightness for all the devices asynchronously
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
        /// Set the color temperature for all the devices asynchronously
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
        /// Set the power for all the devices asynchronously
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> SetPower(bool state = true)
        {
            return await Process((Device device) =>
            {
                return device.SetPower(state);
            });
        }

        /// <summary>
        /// Set the RGB Color for all the devices asynchronously
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
        /// Starts a color flow for all devices asynchronously
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<bool> StartColorFlow(ColorFlow flow)
        {
            return await Process((Device device) =>
            {
                return device.StartColorFlow(flow);
            });
        }

        /// <summary>
        /// stops the color flow of all devices asynchronously
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
        /// Toggle the power for all the devices asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Toggle()
        {
            return await Process((Device device) =>
            {
                return device.Toggle();
            });
        }

        #endregion Public Methods
    }
}