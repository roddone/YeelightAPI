using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundAdjustBrightAsync(int percent, int? duration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundAdjustColorAsync(int percent, int? duration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundAdjustColorTemperatureAsync(int percent, int? duration = null, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetAdjustAsync(AdjustAction action, AdjustProperty property, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetBrightnessAsync(int value, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the color temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetColorTemperatureAsync(int temperature, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save the current state as default
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetHSVColorAsync(int hue, int sat, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set Power
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetPowerAsync(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetRGBColorAsync(int r, int g, int b, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set a scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundSetSceneAsync(Scene scene, CancellationToken cancellationToken = default);

        /// <summary>
        /// Start a color flow
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundStartColorFlowAsync(ColorFlow flow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the current color flow
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundStopColorFlowAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Toggle the device power state
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundToggleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Turn-Off the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundTurnOffAsync(int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Turn-On the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> BackgroundTurnOnAsync(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default);

        /// <summary>
        /// Toggle both Background and classical power state
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> DevToggleAsync(CancellationToken cancellationToken = default);

        #endregion Public Methods
    }
}
