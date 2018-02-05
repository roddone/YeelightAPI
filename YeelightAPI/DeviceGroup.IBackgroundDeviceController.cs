using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    public partial class DeviceGroup : IBackgroundDeviceController
    {

        #region synchronous

        public bool BackgroundSetBrightness(int value, int? smooth = null)
        {
            return BackgroundSetBrightnessAsync(value, smooth).Result;
        }

        public bool BackgroundSetColorTemperature(int temperature, int? smooth)
        {
            return BackgroundSetColorTemperatureAsync(temperature, smooth).Result;
        }

        public bool BackgroundSetPower(bool state = true)
        {
            return BackgroundSetPowerAsync(state).Result;
        }

        public bool BackgroundSetRGBColor(int r, int g, int b, int? smooth)
        {
            return BackgroundSetRGBColorAsync(r, g, b, smooth).Result;
        }

        public bool BackgroundToggle()
        {
            return BackgroundToggleAsync().Result;
        }

        #endregion synchronous

        #region asynchronous

        public async Task<bool> BackgroundSetBrightnessAsync(int value, int? smooth = null)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.BackgroundSetBrightnessAsync(value, smooth));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        public async Task<bool> BackgroundSetColorTemperatureAsync(int temperature, int? smooth)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.BackgroundSetColorTemperatureAsync(temperature, smooth));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        public async Task<bool> BackgroundSetPowerAsync(bool state = true)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.BackgroundSetPowerAsync(state));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        public async Task<bool> BackgroundSetRGBColorAsync(int r, int g, int b, int? smooth)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.BackgroundSetRGBColorAsync(r, g, b, smooth));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        public async Task<bool> BackgroundToggleAsync()
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(device.BackgroundToggleAsync());
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

        #endregion asynchyronous
    }
}
