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
        public ColorFlowRGBExpression(int r, int g, int b, int duration = 50, int brightness = -1)
        {
            this.Duration = duration;
            this.Value = ((r) << 16) | ((g) << 8) | (b);
            this.Mode = ColorFlowMode.Color;
            this.Brightness = brightness;
        }
    }
}
