using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    public interface IBackgroundDeviceController
    {

        Task<bool> BackgroundToggle();

        Task<bool> BackgroundSetPower(bool state = true);

        Task<bool> BackgroundSetBrightness(int value, int? smooth = null);

        Task<bool> BackgroundSetRGBColor(int r, int g, int b, int? smooth);

        Task<bool> BackgroundSetColorTemperature(int temperature, int? smooth);
    }
}
