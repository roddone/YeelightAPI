using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightClient.Models
{
    /// <summary>
    /// Available Properties
    /// </summary>
    [Flags]
    public enum PROPERTIES
    {
        /// <summary>
        /// on: smart LED is turned on / off: smart LED is turned off
        /// </summary>
        [RealName("power")]
        Power = 1,

        /// <summary>
        /// Brightness percentage. Range 1 ~ 100
        /// </summary>
        [RealName("bright")]
        BrightnessPercentage = 1 << 1,

        /// <summary>
        /// Color temperature. Range 1700 ~ 6500(k)
        /// </summary>
        [RealName("ct")]
        ColorTemperature = 1 << 2,

        /// <summary>
        /// Color. Range 1 ~ 16777215
        /// </summary>
        [RealName("rgb")]
        RGBColor = 1 << 3,

        /// <summary>
        /// Hue. Range 0 ~ 359
        /// </summary>
        [RealName("hue")]
        Hue = 1 << 4,

        /// <summary>
        /// Saturation. Range 0 ~ 100
        /// </summary>
        [RealName("sat")]
        Saturation = 1 << 5,

        /// <summary>
        /// 1: rgb mode / 2: color temperature mode / 3: hsv mode
        /// </summary>
        [RealName("color_mode")]
        ColorMode = 1 << 6,

        /// <summary>
        /// 0: no flow is running / 1:color flow is running
        /// </summary>
        [RealName("flowing")]
        Flowing = 1 << 7,

        /// <summary>
        /// The remaining time of a sleep timer. Range 1 ~ 60 (minutes)
        /// </summary>
        [RealName("delayoff")]
        SleepTimer = 1 << 8,

        /// <summary>
        /// Current flow parameters (only meaningful when 'flowing' is 1)
        /// </summary>
        [RealName("flow_params")]
        FlowParameters = 1 << 9,

        /// <summary>
        /// 1: Music mode is on / 0: Music mode is off
        /// </summary>
        [RealName("music_on")]
        MusicOn = 1 << 10,

        /// <summary>
        /// The name of the device set by “set_name” command
        /// </summary>
        [RealName("name")]
        DeviceName = 1 << 11,

        /// <summary>
        /// Background light power status
        /// </summary>
        [RealName("bg_power")]
        BackgroundLightPower = 1 << 12,

        /// <summary>
        /// Background light is flowing
        /// </summary>
        [RealName("bg_flowing")]
        BagroundLightFlowing = 1 << 13,

        /// <summary>
        /// Current flow parameters of background light
        /// </summary>
        [RealName("bg_flow_params")]
        BackgroundLightFLowParameters = 1 << 14,

        /// <summary>
        /// Color temperature of background light
        /// </summary>
        [RealName("bg_ct")]
        BackgroundLightColorTemperature = 1 << 15,

        /// <summary>
        /// 1: rgb mode / 2: color temperature mode / 3: hsv mode
        /// </summary>
        [RealName("bg_lmode")]
        BackgroundLightMode= 1<<16,

        /// <summary>
        /// Brightness percentage of background light
        /// </summary>
        [RealName("bg_bright")]
        BackgroundLightBrightnessPercentage=1<<17,

        /// <summary>
        /// Color of background light
        /// </summary>
        [RealName("bg_rgb")]
        BackgroundLightRGBColor=1<<18,

        /// <summary>
        /// Hue of background light
        /// </summary>
        [RealName("bg_hue")]
        BackgroundLightHue=1<<19,

        /// <summary>
        /// Saturation of background light    
        /// </summary>
        [RealName("bg_sat")]
        BackgroundLightSaturation=1<<20,

        /// <summary>
        /// Brightness of night mode light
        /// </summary>
        [RealName("nl_br")]
        NightModeLightBrightness=1<<21,

        /// <summary>
        /// All Properties
        /// </summary>
        All = ~0
    }
}
