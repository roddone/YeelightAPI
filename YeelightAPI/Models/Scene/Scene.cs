using System;
using System.Collections.Generic;
using YeelightAPI.Core;

namespace YeelightAPI.Models.Scene
{
    /// <summary>
    /// Scene
    /// </summary>
    public class Scene : List<object>
    {
        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Scene(List<object> parameters)
        {
            this.AddRange(parameters);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Get a Scene from an auto delay off timing
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static Scene FromAutoDelayOff(int delay, int brightness)
        {
            List<object> parameters = new List<object>()
            {
                SceneClass.auto_delay_off.ToString(),
                brightness,
                delay
            };

            return new Scene(parameters);
        }

        /// <summary>
        /// Get a Scene from a color flow
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public static Scene FromColorFlow(ColorFlow.ColorFlow flow)
        {
            if (flow == null)
            {
                throw new ArgumentNullException(nameof(flow));
            }

            List<object> parameters = new List<object>()
            {
                SceneClass.cf.ToString(),
                flow.RepetitionCount,
                (int)flow.EndAction,
                flow.GetColorFlowExpression()
            };

            return new Scene(parameters);
        }

        /// <summary>
        /// Get a Scene from a color temperature
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static Scene FromColorTemperature(int temperature, int brightness)
        {
            List<object> parameters = new List<object>()
            {
                SceneClass.ct.ToString(),
                temperature,
                brightness
            };

            return new Scene(parameters);
        }

        /// <summary>
        /// Get a Scene from a HSV color
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="sat"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static Scene FromHSVColor(int hue, int sat, int brightness)
        {
            List<object> parameters = new List<object>()
            {
                SceneClass.hsv.ToString(),
                hue,
                sat,
                brightness
            };

            return new Scene(parameters);
        }

        /// <summary>
        /// Get a Scene from a RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static Scene FromRGBColor(int r, int g, int b, int brightness)
        {
            List<object> parameters = new List<object>()
            {
                SceneClass.color.ToString(),
                ColorHelper.ComputeRGBColor(r, g, b),
                brightness
            };

            return new Scene(parameters);
        }

        #endregion Public Methods
    }
}