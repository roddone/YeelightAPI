using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Events
{
    /// <summary>
    /// Device found event argument
    /// </summary>
    public class DeviceFoundEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Notification Result
        /// </summary>
        public Device Device { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceFoundEventArgs() { }

        /// <summary>
        /// Constructor with notification result
        /// </summary>
        /// <param name="device"></param>
        public DeviceFoundEventArgs(Device device)
        {
            Device = device;
        }

        #endregion Public Constructors
    }
}
