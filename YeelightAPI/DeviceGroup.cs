using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    /// <summary>
    /// Group of devices
    /// </summary>
    public class DeviceGroup : List<Device>, IDeviceController
    {

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; set; }

        #endregion PUBLIC PROPERTIES

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="device"></param>
        public DeviceGroup(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="device"></param>
        public DeviceGroup(Device device, string name = null)
        {
            this.Add(device);
            this.Name = name;
        }

        /// <summary>
        /// Constructor with devices as params
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(params Device[] devices)
        {
            this.AddRange(devices);
        }

        /// <summary>
        /// Constructor with a list (IEnumerable) of devices
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(IEnumerable<Device> devices, string name = null)
        {
            this.AddRange(devices);
            this.Name = name;
        }

        #endregion CONSTRUCTOR

        #region IDeviceController

        /// <summary>
        /// Connect all the devices
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            bool result = true;
            foreach (Device device in this)
            {
                result &= device.Connect();
            }

            return result;
        }

        /// <summary>
        /// Connect all the devices asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.ConnectAsync());
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
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
        /// Set the brightness for all the devices
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetBrightness(int value, int? smooth = null)
        {
            bool result = true;
            foreach (Device device in this)
            {
                result &= device.SetBrightness(value, smooth);
            }

            return result;
        }

        /// <summary>
        /// Set the brightness for all the devices asynchronously
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetBrightnessAsync(int value, int? smooth = null)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.SetBrightnessAsync(value, smooth));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        /// <summary>
        /// Set the color temperature for all the devices
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetColorTemperature(int temperature, int? smooth)
        {
            bool result = true;
            foreach (Device device in this)
            {
                result &= device.SetColorTemperature(temperature, smooth);
            }

            return result;
        }

        /// <summary>
        /// Set the color temperature for all the devices asynchronously
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetColorTemperatureAsync(int temperature, int? smooth)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.SetColorTemperatureAsync(temperature, smooth));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        /// <summary>
        /// Set the power for all the devices
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetPower(bool state = true)
        {
            bool result = true;
            foreach (Device device in this)
            {
                result &= device.SetPower(state);
            }

            return result;
        }

        /// <summary>
        /// Set the power for all the devices asynchronously
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> SetPowerAsync(bool state = true)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.SetPowerAsync(state));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        /// <summary>
        /// Set the RGB Color for all the devices
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetRGBColor(int r, int g, int b, int? smooth)
        {
            bool result = true;
            foreach (Device device in this)
            {
                result &= device.SetRGBColor(r, g, b, smooth);
            }

            return result;
        }

        /// <summary>
        /// Set the RGB Color for all the devices asynchronously
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.SetRGBColorAsync(r, g, b, smooth));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        /// <summary>
        /// Toggle the power for all the devices
        /// </summary>
        /// <returns></returns>
        public bool Toggle()
        {
            bool result = true;
            foreach (Device device in this)
            {
                result &= device.Toggle();
            }

            return result;
        }
        /// <summary>
        /// Toggle the power for all the devices asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ToggleAsync()
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.ToggleAsync());
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        #endregion IDeviceController

    }
}
