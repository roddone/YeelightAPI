using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Ways to check the connection of the device
    /// </summary>
    public enum ConnectionCheckMode
    {
        /// <summary>
        /// try to get the current state by performing a "TCP Poll"
        /// </summary>
        TryGetCurrent = 0,

        /// <summary>
        /// use the last TCP State
        /// </summary>
        LastKnownState = 1,

        /// <summary>
        /// Ping the device to check the connection
        /// </summary>
        Ping = 2
    }
}
