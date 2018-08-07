using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.ColorFlow;
using YeelightAPI.Models.Scene;

namespace YeelightAPI
{
    /// <summary>
    /// Descriptor for Devices Background Operations
    /// </summary>
    public interface IBackgroundDeviceController
    {
        #region Public Methods

        /// <summary>
        /// Adjusts the brightness
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task<bool> BackgroundAdjustBright(int percent, int? duration = null);

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task<bool> BackgroundAdjustColor(int percent, int? duration = null);

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Task<bool> BackgroundAdjustColorTemperature(int percent, int? duration = null);

        /// <summary>
        /// Initiate a new Color Flow
        /// </summary>
        /// <returns></returns>
        FluentFlow BackgroundFlow();

        /// <summary>
        /// Adjust settings
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetAdjust(AdjustAction action, AdjustProperty property);

        /// <summary>
        /// Set the brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetBrightness(int value, int? smooth = null);

        /// <summary>
        /// Set the color temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetColorTemperature(int temperature, int? smooth = null);

        /// <summary>
        /// Save the current state as default
        /// </summary>
        /// <returns></returns>
        Task<bool> BackgroundSetDefault();

        /// <summary>
        /// Set the HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetHSVColor(int hue, int sat, int? smooth = null);

        /// <summary>
        /// Set Power
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetPower(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal);

        /// <summary>
        /// Set RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetRGBColor(int r, int g, int b, int? smooth = null);

        /// <summary>
        /// Set a scene
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetScene(Scene scene);

        /// <summary>
        /// Start a color flow
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        Task<bool> BackgroundStartColorFlow(ColorFlow flow);

        /// <summary>
        /// Stop the current color flow
        /// </summary>
        /// <returns></returns>
        Task<bool> BackgroundStopColorFlow();

        /// <summary>
        /// Toggle the device power state
        /// </summary>
        /// <returns></returns>
        Task<bool> BackgroundToggle();

        /// <summary>
        /// Turn-Off the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <returns></returns>
        Task<bool> BackgroundTurnOff(int? smooth = null);

        /// <summary>
        /// Turn-On the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        Task<bool> BackgroundTurnOn(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal);

        /// <summary>
        /// Toggle both Background and classical power state
        /// </summary>
        /// <returns></returns>
        Task<bool> DevToggle();

        #endregion Public Methods
    }
}