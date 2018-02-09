using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow to change RGB color
    /// </summary>
    public class ColorFlowRGBExpression : ColorFlowExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="duration"></param>
        /// <param name="brightness"></param>
        public ColorFlowRGBExpression(int r, int g, int b, int brightness, int? duration = null)
        {
            this.Duration = duration ?? 50;
            this.Duration = Math.Max(50, this.Duration);

            this.Value = ((r) << 16) | ((g) << 8) | (b);
            this.Mode = ColorFlowMode.Color;
            this.Brightness = brightness;
        }
    }
}
