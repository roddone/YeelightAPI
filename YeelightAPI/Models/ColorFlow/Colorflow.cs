using System.Collections.Generic;
using System.Linq;

namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Color flow
    /// </summary>
    public class ColorFlow : List<ColorFlowExpression>
    {
        #region Public Properties

        /// <summary>
        /// Action taken when the flow stops
        /// </summary>
        public ColorFlowEndAction EndAction { get; set; }

        /// <summary>
        /// Number of repetitions
        /// </summary>
        public int RepetitionCount { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repetitionCount"></param>
        /// <param name="endAction"></param>
        /// <param name="expressions"></param>
        public ColorFlow(int repetitionCount, ColorFlowEndAction endAction, params ColorFlowExpression[] expressions) : this(repetitionCount, endAction, expressions?.ToList())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repetitionCount"></param>
        /// <param name="endAction"></param>
        /// <param name="expressions"></param>
        public ColorFlow(int repetitionCount, ColorFlowEndAction endAction, IEnumerable<ColorFlowExpression> expressions)
        {
            RepetitionCount = repetitionCount;
            EndAction = endAction;
            AddRange(expressions);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Returns the flow expression
        /// </summary>
        /// <returns></returns>
        public string GetColorFlowExpression()
        {
            List<int> flow = new List<int>();

            foreach (ColorFlowExpression expression in this)
            {
                flow.AddRange(expression.GetFlow());
            }

            return string.Join(",", flow);
        }

        #endregion Public Methods
    }
}