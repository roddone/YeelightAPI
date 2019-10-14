using YeelightAPI.Core;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Available product models.
    /// </summary>
    public enum MODEL
    {
        /// <summary>
        /// Unknown model.
        /// </summary>
        Unknown,

        /// <summary>
        /// Mono device, supports brightness adjustment only.
        /// </summary>
        [RealName("mono")]
        Mono,

        /// <summary>
        /// Color device, support both color and color temperature adjustment.
        /// </summary>
        [RealName("color")]
        Color,

        /// <summary>
        /// Smart LED stripe.
        /// </summary>
        [RealName("stripe")]
        Stripe,

        /// <summary>
        ///  Ceiling Light.
        /// </summary>
        [RealName("ceiling")]
        Ceiling,

        /// <summary>
        /// Bedside lamp.
        /// </summary>
        [RealName("bslamp")]
        BedsideLamp,

        /// <summary>
        /// Desk Lamp.
        /// </summary>
        [RealName("desklamp")]
        DeskLamp,


        /// <summary>
        /// White bulb, supports color temperature adjustment.
        /// </summary>
        [RealName("ct_bulb")]
        TunableWhiteBulb,
    }
}