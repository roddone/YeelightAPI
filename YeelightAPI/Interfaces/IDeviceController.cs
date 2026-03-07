using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AdjustBrightAsync(int percent, int? duration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adjusts the color
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AdjustColorAsync(int percent, int? duration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="duration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AdjustColorTemperatureAsync(int percent, int? duration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Establish a connection to the device
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a cron task
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CronAddAsync(int value, CronType type = CronType.PowerOff, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a cron task
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> CronDeleteAsync(CronType type = CronType.PowerOff, CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetAdjustAsync(AdjustAction action, AdjustProperty property, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetBrightnessAsync(int value, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the color temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetColorTemperatureAsync(int temperature, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save the current state as default
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetHSVColorAsync(int hue, int sat, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set Power
        /// </summary>
        /// <param name="state"></param>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetPowerAsync(bool state = true, int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set a scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetSceneAsync(Scene scene, CancellationToken cancellationToken = default);

        /// <summary>
        /// Start a color flow
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> StartColorFlowAsync(ColorFlow flow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Start the music mode
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> StartMusicModeAsync(string hostName, int port, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the current color flow
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> StopColorFlowAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the music mode
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> StopMusicModeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Toggle the power state
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ToggleAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Turn-Off the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> TurnOffAsync(int? smooth = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Turn-On the device
        /// </summary>
        /// <param name="smooth"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> TurnOnAsync(int? smooth = null, PowerOnMode mode = PowerOnMode.Normal, CancellationToken cancellationToken = default);

        #endregion Public Methods
    }
}
