using YeelightAPI.Core;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow to change the color temperature
    /// </summary>
    public class ColorFlowTemperatureExpression : ColorFlowExpression
    {
        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="duration"></param>
        /// <param name="brightness"></param>
        public ColorFlowTemperatureExpression(int temperature, int brightness, int duration = Constants.MinimumFlowExpressionDuration)
        {
            Duration = duration;
            Mode = ColorFlowMode.ColorTemperature;
            Value = temperature;
            Brightness = brightness;
        }

        #endregion Public Constructors
    }
}