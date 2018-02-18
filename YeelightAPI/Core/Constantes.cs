using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Common
    /// </summary>
    internal class Constantes
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
        /// Serializer settings
        /// </summary>
        public static readonly JsonSerializerSettings DeviceSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        #endregion Public Fields
    }
}