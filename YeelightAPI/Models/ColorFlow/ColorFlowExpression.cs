using System.Collections.Generic;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow expression
    /// </summary>
    public class ColorFlowExpression
    {
        #region Public Properties

        /// <summary>
        /// Brightness (-1 or 1~100)
        /// </summary>
        public int Brightness { get; set; }

        /// <summary>
        /// Duration of the effect
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Mode
        /// </summary>
        public ColorFlowMode Mode { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public int Value { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// returns the flow expression
        /// </summary>
        /// <returns></returns>
        public List<int> GetFlow()
        {
            return new List<int>(4)
            {
                Duration,
                (int)Mode,
                Value,
                Brightness
            };
        }

        #endregion Public Methods
    }
}