using System;
using System.Collections.Generic;
using System.Text;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Notification event Argument
    /// </summary>
    public class NotificationReceivedEventArgs : EventArgs
    {
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

        /// <summary>
        /// Notification Result
        /// </summary>
        public NotificationResult Result { get; set; }
    }
}
