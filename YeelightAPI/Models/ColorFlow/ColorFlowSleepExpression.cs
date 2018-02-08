using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow to sleep
    /// </summary>
    public class ColorFlowSleepExpression : ColorFlowExpression
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="duration"></param>
        public ColorFlowSleepExpression(int duration)
        {
            this.Mode = ColorFlowMode.Sleep;
            this.Value = 0;
            this.Duration = duration;
            this.Brightness = 0;
        }
    }
}
