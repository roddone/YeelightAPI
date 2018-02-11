using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;

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

        /// <summary>
        /// Starts a background color flow 
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundStartColorFlow(ColorFlow flow)
        {
            List<object> parameters = new List<object>() { flow.RepetitionCount, (int)flow.EndAction, flow.GetColorFlowExpression() };

            CommandResult result = await ExecuteCommandWithResponse(
                method: METHODS.StartBackgroundLightColorFlow,
                id: (int)METHODS.StartBackgroundLightColorFlow,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Stops the background color flow
        /// </summary>
        /// <returns></returns>
        public async Task<bool> BackgroundStopColorFlow()
        {
            CommandResult result = await ExecuteCommandWithResponse(
                            method: METHODS.StopBackgroundLightColorFlow,
                            id: (int)METHODS.StopBackgroundLightColorFlow);

            return result.IsOk();
        }

        /// <summary>
        /// Adjusts the background light state
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public async Task<bool> BackgroundAdjust(AdjustAction action, AdjustProperty property)
        {
            {
                List<object> parameters = new List<object>() { action.ToString(), property.ToString() };

                CommandResult result = await ExecuteCommandWithResponse(
                    method: METHODS.SetBackgroundLightAdjust,
                    id: (int)METHODS.SetBackgroundLightAdjust,
                    parameters: parameters);

                return result.IsOk();
            }
        }
    }
}
