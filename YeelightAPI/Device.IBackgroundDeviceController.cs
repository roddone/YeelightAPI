using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    public partial class Device : IBackgroundDeviceController
    {

        #region synchronous

        public bool BackgroundToggle()
        {
            return BackgroundToggleAsync().Result;
        }

        public bool BackgroundSetPower(bool state = true)
        {
            return BackgroundSetPowerAsync(state).Result;
        }

        public bool BackgroundSetBrightness(int value, int? smooth = null)
        {
            return BackgroundSetBrightnessAsync(value, smooth).Result;
        }

        public bool BackgroundSetRGBColor(int r, int g, int b, int? smooth)
        {
            return BackgroundSetRGBColorAsync(r, g, b, smooth).Result;
        }

        public bool BackgroundSetColorTemperature(int temperature, int? smooth)
        {
            return BackgroundSetColorTemperatureAsync(temperature, smooth).Result;
        }

        #endregion synchronous

        #region asynchronous

        public async Task<bool> BackgroundToggleAsync()
        {
            CommandResult result = await ExecuteCommandWithResponseAsync(METHODS.ToggleBackgroundLight, id: (int)METHODS.ToggleBackgroundLight);

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetPowerAsync(bool state = true)
        {
            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetBackgroundLightPower,
                id: (int)METHODS.SetBackgroundLightPower,
                parameters: new List<object>() { state ? "on" : "off" }
            );

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetBrightnessAsync(int value, int? smooth = null)
        {
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetBackgroundLightBrightness,
                id: (int)METHODS.SetBackgroundLightBrightness,
                parameters: parameters);

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetRGBColorAsync(int r, int g, int b, int? smooth)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetBackgroundLightRGBColor,
                id: (int)METHODS.SetBackgroundLightRGBColor,
                parameters: parameters);

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetColorTemperatureAsync(int temperature, int? smooth)
        {
            List<object> parameters = new List<object>() { temperature };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetBackgroundColorTemperature,
                id: (int)METHODS.SetBackgroundColorTemperature,
                parameters: parameters);

            return result.IsOk();
        }

        #endregion asynchronous

    }
}
