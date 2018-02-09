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
        public ColorFlowSleepExpression(int? duration = null)
        {
            this.Duration = duration ?? 50;
            this.Duration = Math.Max(50, this.Duration);

            this.Mode = ColorFlowMode.Sleep;
            this.Value = 0;
            this.Brightness = 0;
        }
    }
}
