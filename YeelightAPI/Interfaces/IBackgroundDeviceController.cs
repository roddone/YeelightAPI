using System.Threading.Tasks;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Scene;

namespace YeelightAPI
{
    public interface IBackgroundDeviceController
    {
        #region Public Methods

        Task<bool> BackgroundSetAdjust(AdjustAction action, AdjustProperty property);

        Task<bool> BackgroundSetBrightness(int value, int? smooth = null);

        Task<bool> BackgroundSetColorTemperature(int temperature, int? smooth);

        Task<bool> BackgroundSetDefault();

        Task<bool> BackgroundSetHSVColor(int hue, int sat, int? smooth = null);

        Task<bool> BackgroundSetPower(bool state = true);

        Task<bool> BackgroundSetRGBColor(int r, int g, int b, int? smooth);

        Task<bool> BackgroundSetScene(Scene scene);

        Task<bool> BackgroundStartColorFlow(ColorFlow flow);

        Task<bool> BackgroundStopColorFlow();

        Task<bool> BackgroundToggle();

        Task<bool> DevToggle();

        #endregion Public Methods
    }
}