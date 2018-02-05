using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    public interface IBackgroundDeviceController
    {
        bool BackgroundToggle();

        bool BackgroundSetPower(bool state = true);

        bool BackgroundSetBrightness(int value, int? smooth = null);

        bool BackgroundSetRGBColor(int r, int g, int b, int? smooth);

        bool BackgroundSetColorTemperature(int temperature, int? smooth);

        Task<bool> BackgroundToggleAsync();

        Task<bool> BackgroundSetPowerAsync(bool state = true);

        Task<bool> BackgroundSetBrightnessAsync(int value, int? smooth = null);

        Task<bool> BackgroundSetRGBColorAsync(int r, int g, int b, int? smooth);

        Task<bool> BackgroundSetColorTemperatureAsync(int temperature, int? smooth);
    }
}
