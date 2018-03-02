using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YeelightAPI.Core;
using YeelightAPI.Models.ColorFlow;

namespace YeelightAPI.Models
{
    /*/// <summary>
    /// Fluent-syntax Color FLow
    /// </summary>
    public class BackgroundFluentFlow : FluentFlow
    {
        #region Private Fields

        private IBackgroundDeviceController _device = null;

        #endregion Private Fields

        #region Internal Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device"></param>
        internal BackgroundFluentFlow(IBackgroundDeviceController device)
        {
            this._device = device;
        }

        #endregion Internal Constructors

        #region Public Methods

        /// <summary>
        /// Play the color flow
        /// </summary>
        /// <param name="endAction"></param>
        /// <param name="repetition"></param>
        /// <returns></returns>
        public async override Task<FluentFlow> Play(ColorFlowEndAction endAction, int repetition = 0)
        {
            ColorFlow.ColorFlow flow = new ColorFlow.ColorFlow(0, endAction, _expressions);

            await _device.BackgroundStartColorFlow(flow);

            return this;
        }



        /// <summary>
        /// Stop the color flow
        /// </summary>
        /// <returns></returns>
        public async override Task<FluentFlow> Stop()
        {
            await _device.BackgroundStopColorFlow();

            return this;
        }


        #endregion Public Methods
    }*/
}