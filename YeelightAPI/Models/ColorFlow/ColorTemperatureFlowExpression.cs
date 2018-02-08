using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow to change the color temperature
    /// </summary>
    public class ColorFlowTemperatureExpression : ColorFlowExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="duration"></param>
        /// <param name="brightness"></param>
        public ColorFlowTemperatureExpression(int temperature, int duration = 50, int brightness = -1)
        {
            this.Duration = duration;
            this.Mode = ColorFlowMode.ColorTemperature;
            this.Value = temperature;
            this.Brightness = brightness;
        }
    }
}
