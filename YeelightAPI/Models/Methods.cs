using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Available Methodss
    /// </summary>
    public enum METHODS
    {
        /// <summary>
        /// Retrieve current property of smart LED
        /// </summary>
        [RealName("get_prop")]
        GetProp,

        /// <summary>
        /// Change the color temperature of a smart LED
        /// </summary>
        [RealName("set_ct_abx")]
        SetColorTemperature,

        /// <summary>
        /// Change the color of a smart LED
        /// </summary>
        [RealName("set_rgb")]
        SetRGBColor,

        /// <summary>
        /// Change the color of a smart LED
        /// </summary>
        [RealName("set_hsv")]
        SetHSVColor,

        /// <summary>
        /// Change the brightness of a smart LED
        /// </summary>
        [RealName("set_bright")]
        SetBrightness,

        /// <summary>
        /// Switch on or off the smart LED (software managed on/off)
        /// </summary>
        [RealName("set_power")]
        SetPower,

        /// <summary>
        /// Toggle the smart LED
        /// </summary>
        [RealName("toggle")]
        Toggle,

        /// <summary>
        /// Save current state of smart LED in persistent
        /// </summary>
        [RealName("set_default")]
        SetDefault,

        /// <summary>
        /// Start a color flow
        /// </summary>
        [RealName("start_cf")]
        StartColorFlow,

        /// <summary>
        /// Stop a running color flow
        /// </summary>
        [RealName("stop_cf")]
        StopColorFlow,

        /// <summary>
        /// Set the smart LED directly to specified state
        /// </summary>
        [RealName("set_scene")]
        SetScene,

        /// <summary>
        /// Start a timer job on the smart LED
        /// </summary>
        [RealName("cron_add")]
        AddCron,

        /// <summary>
        /// Retrieve the setting of the current cron job of the specified type
        /// </summary>
        [RealName("cron_get")]
        GetCron,

        /// <summary>
        /// Stop the specified cron job
        /// </summary>
        [RealName("cron_del")]
        DeleteCron,

        /// <summary>
        /// Change brightness, CT or color of a smart LED without knowing the current value, it's main used by controllers
        /// </summary>
        [RealName("set_adjust")]
        SetAdjust,

        /// <summary>
        /// Start or stop music mode on a device. Under music mode, no property will be reported and no message quota is checked
        /// </summary>
        [RealName("set_music")]
        SetMusicMode,

        /// <summary>
        /// Name the device. The name will be stored on the device and reported in discovering response. User can also read the name through "get_prop"
        /// </summary>
        [RealName("set_name")]
        SetName,

        /// <summary>
        /// Change the background color of a smart LED
        /// </summary>
        [RealName("bg_set_rgb")]
        SetBackgroundLightRGBColor,

        /// <summary>
        /// Change the background color of a smart LED
        /// </summary>
        [RealName("bg_set_hsv")]
        SetBackgroundLightHSVColor,

        /// <summary>
        /// Change the background color temperature of a smart LED
        /// </summary>
        [RealName("bg_set_ct_abx")]
        SetBackgroundColorTemperature,

        /// <summary>
        /// Start a background color flow
        /// </summary>
        [RealName("bg_start_cf")]
        StartBackgroundLightColorFlow,

        /// <summary>
        /// Stop a running background color flow
        /// </summary>
        [RealName("bg_stop_cf")]
        StopBackgroundLightColorFlow,

        /// <summary>
        /// Set the background smart LED directly to specified state
        /// </summary>
        [RealName("bg_set_scene")]
        SetBackgroundLightScene,

        /// <summary>
        /// Save current state of background smart LED in persistent
        /// </summary>
        [RealName("bg_set_default")]
        SetBackgroundLightDefault,

        /// <summary>
        /// Switch on or off the background smart LED (software managed on/off)
        /// </summary>
        [RealName("bg_set_power")]
        SetBackgroundLightPower,

        /// <summary>
        /// Change the brightness of a background smart LED
        /// </summary>
        [RealName("bg_set_bright")]
        SetBackgroundLightBrightness,

        /// <summary>
        /// Change brightness, CT or color of a background smart LED without knowing the current value, it's main used by controllers
        /// </summary>
        [RealName("bg_set_adjust")]
        SetBackgroundLightAdjust,

        /// <summary>
        /// Toggle the background smart LED
        /// </summary>
        [RealName("bg_toggle")]
        ToggleBackgroundLight,

        /// <summary>
        /// Toggle the main light and background light at the same time
        /// </summary>
        [RealName("dev_toggle")]
        ToggleDev
    }
}
