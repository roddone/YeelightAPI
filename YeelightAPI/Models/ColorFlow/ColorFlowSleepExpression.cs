using YeelightAPI.Core;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow to sleep
    /// </summary>
    public class ColorFlowSleepExpression : ColorFlowExpression
    {
        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="duration"></param>
        public ColorFlowSleepExpression(int duration = Constants.MinimumFlowExpressionDuration)
        {
            Duration = duration;
            Mode = ColorFlowMode.Sleep;
            Value = 0;
            Brightness = 0;
        }

        #endregion Public Constructors
    }
}