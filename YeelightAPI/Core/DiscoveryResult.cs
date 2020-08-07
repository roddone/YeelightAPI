using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Results of a discovery
    /// </summary>
    internal class DiscoveryResult
    {
        /// <summary>
        /// Devices found during the discovery
        /// </summary>
        internal IEnumerable<Device> Devices { get; set; }

        /// <summary>
        /// Exceptions thrown during discovery
        /// </summary>
        internal IEnumerable<DeviceDiscoveryException> Exceptions { get; set; }
    }
}
