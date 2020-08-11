using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Results of a discovery
    /// </summary>
    internal class DiscoveryResult
    {
      public DiscoveryResult()
      {
        this.Devices = new List<Device>();
        this.Exceptions = new List<DeviceDiscoveryException>();
      }
      public DiscoveryResult(IEnumerable<Device> devices) : this()
      {
        this.Devices = devices;
      }
      public DiscoveryResult(IEnumerable<DeviceDiscoveryException> deviceDiscoveryExceptions) : this()
      {
        this.Exceptions = deviceDiscoveryExceptions;
      }
        /// <summary>
        /// Devices found during the discovery
        /// </summary>
        internal IEnumerable<Device> Devices { get; set; }

        /// <summary>
        /// Exceptions thrown during discovery
        /// </summary>
        internal IEnumerable<DeviceDiscoveryException> Exceptions { get; set; }

        /// <summary>
        /// Returns whether the result object has an error.
        /// </summary>
        internal bool HasError => this.Exceptions.Any();

        /// <summary>
        /// Returns whether the result object has any discovered <see cref="Device"/>.
        /// </summary>
        internal bool HasResult => this.Devices.Any();
    }
}
