using System.Threading.Tasks;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;
using YeelightAPI.Models.Scene;

namespace YeelightAPI
{
    public interface IDeviceController
    {
        #region Public Methods

        Task<bool> Connect();

        Task<bool> CronAdd(int value, CronType type = CronType.PowerOff);

        Task<bool> CronDelete(CronType type = CronType.PowerOff);

        void Disconnect();

        Task<bool> SetAdjust(AdjustAction action, AdjustProperty property);

        Task<bool> SetBrightness(int value, int? smooth = null);

        Task<bool> SetColorTemperature(int temperature, int? smooth = null);

        Task<bool> SetDefault();

        Task<bool> SetHSVColor(int hue, int sat, int? smooth = null);

        Task<bool> SetPower(bool state = true);

        Task<bool> SetRGBColor(int r, int g, int b, int? smooth = null);

        Task<bool> SetScene(Scene scene);

        Task<bool> StartColorFlow(ColorFlow flow);

        Task<bool> StopColorFlow();

        Task<bool> Toggle();

        #endregion Public Methods
    }
}