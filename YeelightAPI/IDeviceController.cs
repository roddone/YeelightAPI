using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        bool SetRGBColor(int r, int g, int b, int? smooth);

        bool SetColorTemperature(int temperature, int? smooth);

        Task<bool> ToggleAsync();

        Task<bool> SetPowerAsync(bool state = true);

        Task<bool> SetBrightnessAsync(int value, int? smooth = null);

        Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth);

        Task<bool> SetColorTemperatureAsync(int temperature, int? smooth);
    }
}
