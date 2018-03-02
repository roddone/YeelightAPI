using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YeelightAPI.Core;
using YeelightAPI.Models.ColorFlow;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Fluent-syntax Color FLow
    /// </summary>
    public class FluentFlow
    {
        #region Private Fields

        private IDeviceController _device = null;

        #endregion Private Fields

        #region Protected Properties

        /// <summary>
        /// List of expressions
        /// </summary>
        protected List<ColorFlowExpression> _expressions => new List<ColorFlowExpression>();

        #endregion Protected Properties

        #region Internal Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device"></param>
        internal FluentFlow(IDeviceController device)
        {
            this._device = device;
        }

        #endregion Internal Constructors

        #region Public Methods

        /// <summary>
        /// Set the duration of the previous action
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public FluentFlow For(int duration)
        {
            _expressions.Last().Duration = duration;

            return this;
        }

        /// <summary>
        /// Play the color flow
        /// </summary>
        /// <param name="endAction"></param>
        /// <param name="repetition"></param>
        /// <returns></returns>
        public async virtual Task<FluentFlow> Play(ColorFlowEndAction endAction, int repetition = 0)
        {
            ColorFlow.ColorFlow flow = new ColorFlow.ColorFlow(0, endAction, _expressions);

            await _device.StartColorFlow(flow);

            return this;
        }

        /// <summary>
        /// Set a RGB Color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="brightness"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public FluentFlow RgbColor(int r, int g, int b, int brightness, int duration = Constants.MinimumFlowExpressionDuration)
        {
            _expressions.Add(new ColorFlowRGBExpression(r, g, b, brightness, duration));

            return this;
        }

        /// <summary>
        /// Sleep
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public FluentFlow Sleep(int duration = Constants.MinimumFlowExpressionDuration)
        {
            _expressions.Add(new ColorFlowSleepExpression(duration));

            return this;
        }

        /// <summary>
        /// Stop the color flow
        /// </summary>
        /// <returns></returns>
        public async virtual Task<FluentFlow> Stop()
        {
            await _device.StopColorFlow();

            return this;
        }

        /// <summary>
        /// Set the color temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="brightness"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public FluentFlow Temperature(int temperature, int brightness, int duration = Constants.MinimumFlowExpressionDuration)
        {
            _expressions.Add(new ColorFlowTemperatureExpression(temperature, brightness, duration));

            return this;
        }

        #endregion Public Methods
    }
}