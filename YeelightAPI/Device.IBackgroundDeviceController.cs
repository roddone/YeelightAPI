using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    public partial class Device : IBackgroundDeviceController
    {
        public async Task<bool> BackgroundToggle()
        {
            CommandResult result = await ExecuteCommandWithResponse(METHODS.ToggleBackgroundLight, id: (int)METHODS.ToggleBackgroundLight);

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetPower(bool state = true)
        {
            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetBackgroundLightPower,
                id: (int)METHODS.SetBackgroundLightPower,
                parameters: new List<object>() { state ? "on" : "off" }
            );

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetBrightness(int value, int? smooth = null)
        {
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetBackgroundLightBrightness,
                id: (int)METHODS.SetBackgroundLightBrightness,
                parameters: parameters);

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetRGBColor(int r, int g, int b, int? smooth)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetBackgroundLightRGBColor,
                id: (int)METHODS.SetBackgroundLightRGBColor,
                parameters: parameters);

            return result.IsOk();
        }

        public async Task<bool> BackgroundSetColorTemperature(int temperature, int? smooth)
        {
            List<object> parameters = new List<object>() { temperature };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.SetBackgroundColorTemperature,
                id: (int)METHODS.SetBackgroundColorTemperature,
                parameters: parameters);

            return result.IsOk();
        }
        
    }
}
