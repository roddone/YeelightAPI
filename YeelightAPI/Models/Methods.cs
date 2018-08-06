using YeelightAPI.Core;

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
        GetProp = 1,

        /// <summary>
        /// Change the color temperature of a smart LED
        /// </summary>
        [RealName("set_ct_abx")]
        SetColorTemperature = 2,

        /// <summary>
        /// Change the color of a smart LED
        /// </summary>
        [RealName("set_rgb")]
        SetRGBColor = 3,

        /// <summary>
        /// Change the color of a smart LED
        /// </summary>
        [RealName("set_hsv")]
        SetHSVColor = 4,

        /// <summary>
        /// Change the brightness of a smart LED
        /// </summary>
        [RealName("set_bright")]
        SetBrightness = 5,

        /// <summary>
        /// Switch on or off the smart LED (software managed on/off)
        /// </summary>
        [RealName("set_power")]
        SetPower = 6,

        /// <summary>
        /// Toggle the smart LED
        /// </summary>
        [RealName("toggle")]
        Toggle = 7,

        /// <summary>
        /// Save current state of smart LED in persistent
        /// </summary>
        [RealName("set_default")]
        SetDefault = 8,

        /// <summary>
        /// Start a color flow
        /// </summary>
        [RealName("start_cf")]
        StartColorFlow = 9,

        /// <summary>
        /// Stop a running color flow
        /// </summary>
        [RealName("stop_cf")]
        StopColorFlow = 10,

        /// <summary>
        /// Set the smart LED directly to specified state
        /// </summary>
        [RealName("set_scene")]
        SetScene = 11,

        /// <summary>
        /// Start a timer job on the smart LED
        /// </summary>
        [RealName("cron_add")]
        AddCron = 12,

        /// <summary>
        /// Retrieve the setting of the current cron job of the specified type
        /// </summary>
        [RealName("cron_get")]
        GetCron = 13,

        /// <summary>
        /// Stop the specified cron job
        /// </summary>
        [RealName("cron_del")]
        DeleteCron = 14,

        /// <summary>
        /// Change brightness, CT or color of a smart LED without knowing the current value, it's main used by controllers
        /// </summary>
        [RealName("set_adjust")]
        SetAdjust = 15,

        /// <summary>
        /// Start or stop music mode on a device. Under music mode, no property will be reported and no message quota is checked
        /// </summary>
        [RealName("set_music")]
        SetMusicMode = 16,

        /// <summary>
        /// Name the device. The name will be stored on the device and reported in discovering response. User can also read the name through "get_prop"
        /// </summary>
        [RealName("set_name")]
        SetName = 17,

        /// <summary>
        /// Change the background color of a smart LED
        /// </summary>
        [RealName("bg_set_rgb")]
        SetBackgroundLightRGBColor = 18,

        /// <summary>
        /// Change the background color of a smart LED
        /// </summary>
        [RealName("bg_set_hsv")]
        SetBackgroundLightHSVColor = 19,

        /// <summary>
        /// Change the background color temperature of a smart LED
        /// </summary>
        [RealName("bg_set_ct_abx")]
        SetBackgroundColorTemperature = 20,

        /// <summary>
        /// Start a background color flow
        /// </summary>
        [RealName("bg_start_cf")]
        StartBackgroundLightColorFlow = 21,

        /// <summary>
        /// Stop a running background color flow
        /// </summary>
        [RealName("bg_stop_cf")]
        StopBackgroundLightColorFlow = 22,

        /// <summary>
        /// Set the background smart LED directly to specified state
        /// </summary>
        [RealName("bg_set_scene")]
        SetBackgroundLightScene = 23,

        /// <summary>
        /// Save current state of background smart LED in persistent
        /// </summary>
        [RealName("bg_set_default")]
        SetBackgroundLightDefault = 24,

        /// <summary>
        /// Switch on or off the background smart LED (software managed on/off)
        /// </summary>
        [RealName("bg_set_power")]
        SetBackgroundLightPower = 25,

        /// <summary>
        /// Change the brightness of a background smart LED
        /// </summary>
        [RealName("bg_set_bright")]
        SetBackgroundLightBrightness = 26,

        /// <summary>
        /// Change brightness, CT or color of a background smart LED without knowing the current value, it's main used by controllers
        /// </summary>
        [RealName("bg_set_adjust")]
        SetBackgroundLightAdjust = 27,

        /// <summary>
        /// Toggle the background smart LED
        /// </summary>
        [RealName("bg_toggle")]
        ToggleBackgroundLight = 28,

        /// <summary>
        /// Toggle the main light and background light at the same time
        /// </summary>
        [RealName("dev_toggle")]
        ToggleDev = 29,

        /// <summary>
        /// Adjusts the brightness
        /// </summary>
        [RealName("adjust_bright")]
        AdjustBright = 30,

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        [RealName("adjust_ct")]
        AdjustColorTemperature = 31,

        /// <summary>
        /// Adjusts the color
        /// </summary>
        [RealName("adjust_color")]
        AdjustColor = 32,

        /// <summary>
        /// Adjusts the brightness
        /// </summary>
        [RealName("bg_adjust_bright")]
        BackgroundAdjustBright = 33,

        /// <summary>
        /// Adjusts the color temperature
        /// </summary>
        [RealName("bg_adjust_ct")]
        BackgroundAdjustColorTemperature = 34,

        /// <summary>
        /// Adjusts the color
        /// </summary>
        [RealName("bg_adjust_color")]
        BackgroundAdjustColor = 35,
    }
}