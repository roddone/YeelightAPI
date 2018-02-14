using System;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Notification event Argument
    /// </summary>
    public class NotificationReceivedEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Notification Result
        /// </summary>
        public NotificationResult Result { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationReceivedEventArgs() { }

        /// <summary>
        /// Constructor with notification result
        /// </summary>
        /// <param name="result"></param>
        public NotificationReceivedEventArgs(NotificationResult result)
        {
            Result = result;
        }

        #endregion Public Constructors
    }
}