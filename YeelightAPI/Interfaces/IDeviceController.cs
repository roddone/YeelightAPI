using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models.ColorFlow;

namespace YeelightAPI
{
    public interface IDeviceController
    {
        bool Connect();

        Task<bool> ConnectAsync();

        void Disconnect();

        bool Toggle();

        bool SetPower(bool state = true);

        bool SetBrightness(int value, int? smooth = null);

        bool SetRGBColor(int r, int g, int b, int? smooth = null);

        bool SetHSVColor(int hue, int sat, int? smooth = null);

        bool SetColorTemperature(int temperature, int? smooth = null);

        bool StartColorFlow(ColorFlow flow);

        Task<bool> ToggleAsync();

        Task<bool> SetPowerAsync(bool state = true);

        Task<bool> SetBrightnessAsync(int value, int? smooth = null);

        Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth = null);

        Task<bool> SetHSVColorAsync(int hue, int sat, int? smooth = null);

        Task<bool> SetColorTemperatureAsync(int temperature, int? smooth = null);

        Task<bool> StartColorFlowAsync(ColorFlow flow);
        
    }
}
