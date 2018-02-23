using YeelightAPI.Core;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow to change RGB color
    /// </summary>
    public class ColorFlowRGBExpression : ColorFlowExpression
    {
        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="duration"></param>
        /// <param name="brightness"></param>
        public ColorFlowRGBExpression(int r, int g, int b, int brightness, int duration = Constantes.MinimumFlowExpressionDuration)
        {
            Duration = duration;
            Value = ((r) << 16) | ((g) << 8) | (b);
            Mode = ColorFlowMode.Color;
            Brightness = brightness;
        }

        #endregion Public Constructors
    }
}