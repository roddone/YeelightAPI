using System.Collections.Generic;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Notification, resulting of a command which has changed the state of the bulb
    /// </summary>
    public class NotificationResult
    {
        #region Public Properties

        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public Dictionary<PROPERTIES, object> Params { get; set; }

        #endregion Public Properties
    }
}