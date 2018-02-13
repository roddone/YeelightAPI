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
        public ColorFlowTemperatureExpression(int temperature, int brightness, int? duration = null)
        {
            Duration = duration ?? 50;
            Duration = Math.Max(50, Duration);

            Mode = ColorFlowMode.ColorTemperature;
            Value = temperature;
            Brightness = brightness;
        }
    }
}
