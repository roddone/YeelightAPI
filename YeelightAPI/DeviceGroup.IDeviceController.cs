using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;

namespace YeelightAPI
{
    /// <summary>
    /// Yeelight group of devices
    /// </summary>
    public partial class DeviceGroup : IDeviceController
    {

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
        /// Adjusts the state of all the devices
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public async Task<bool> Adjust(AdjustAction action, AdjustProperty property)
        {
            return await Process((Device device) =>
            {
                return device.Adjust(action, property);
            });
        }
    }
}
