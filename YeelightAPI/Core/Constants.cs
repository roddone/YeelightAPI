using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Common
    /// </summary>
    internal static class Constants
    {
        #region Public Fields

        /// <summary>
        /// Default port value
        /// </summary>
        public const int DefaultPort = 55443;

        /// <summary>
        /// Default Timeout when waiting for a response from the device
        /// </summary>
        public const int DefaultTimeout = 5000;

        /// <summary>
        /// Line separator
        /// </summary>
        public const string LineSeparator = "\r\n";

        /// <summary>
        /// Minimum Flow duration
        /// </summary>
        public const int MinimumFlowExpressionDuration = 50;

        /// <summary>
        /// Minimum Smooth value
        /// </summary>
        public const int MinimumSmoothDuration = 30;

        /// <summary>
        /// "Off" parameter to change power state
        /// </summary>
        public const string Off = "off";

        /// <summary>
        /// "On" parameter to change power state
        /// </summary>
        public const string On = "on";

        /// <summary>
        /// Serializer settings
        /// </summary>
        public static readonly JsonSerializerSettings DeviceSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        #endregion Public Fields
    }
}