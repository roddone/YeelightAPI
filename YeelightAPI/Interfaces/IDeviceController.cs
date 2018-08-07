using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Cron;
using YeelightAPI.Models.Scene;

namespace YeelightAPI
{
    /// <summary>
    /// Descriptor for Devices operations
    /// </summary>
    public interface IDeviceController
    {
        #region Public Methods

        /// <summary>
        /// Adjusts the brightness
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task<bool> AdjustBright(int percent, int? duration = null);

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task<bool> AdjustColor(int percent, int? duration = null);

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task<bool> AdjustColorTemperature(int percent, int? duration = null);

        /// <summary>
        /// Establish a connection to the device
        /// </summary>
        /// <returns></returns>
        Task<bool> Connect();

        /// <summary>
        /// Add a cron task
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> CronAdd(int value, CronType type = CronType.PowerOff);

        /// <summary>
        /// Delete a cron task
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<bool> CronDelete(CronType type = CronType.PowerOff);

        /// <summary>
        /// Disconnect the device
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Initiate a new Color Flow
        /// </summary>
        /// <returns></returns>
        FluentFlow Flow();

        /// <summary>
        /// Adjust settings
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        Task<bool> SetAdjust(AdjustAction action, AdjustProperty property);

        /// <summary>
        /// Set the brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> SetBrightness(int value, int? smooth = null);

        /// <summary>
        /// Set the color temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> SetColorTemperature(int temperature, int? smooth = null);

        /// <summary>
        /// Save the current state as default
        /// </summary>
        /// <returns></returns>
        Task<bool> SetDefault();

        /// <summary>
        /// Set the HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> SetHSVColor(int hue, int sat, int? smooth = null);

        /// <summary>
        /// Set Power
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        Task<bool> SetPower(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal);

        /// <summary>
        /// Set RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> SetRGBColor(int r, int g, int b, int? smooth = null);

        /// <summary>
        /// Set a scene
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        Task<bool> SetScene(Scene scene);

        /// <summary>
        /// Start a color flow
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        Task<bool> StartColorFlow(ColorFlow flow);

        /// <summary>
        /// Start the music mode
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task<bool> StartMusicMode(string hostName, int port);

        /// <summary>
        /// Stop the current color flow
        /// </summary>
        /// <returns></returns>
        Task<bool> StopColorFlow();

        /// <summary>
        /// Stop the music mode
        /// </summary>
        /// <returns></returns>
        Task<bool> StopMusicMode();

        /// <summary>
        /// Toggle the power state
        /// </summary>
        /// <returns></returns>
        Task<bool> Toggle();

        /// <summary>
        /// Turn-Off the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> TurnOff(int? smooth = null);

        /// <summary>
        /// Turn-On the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        Task<bool> TurnOn(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal);

        #endregion Public Methods
    }
}