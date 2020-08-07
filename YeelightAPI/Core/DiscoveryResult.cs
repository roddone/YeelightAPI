using System;
using System.Collections.Generic;
using System.Text;

namespace YeelightAPI.Core
{
    internal class DiscoveryResult
    {
        internal IEnumerable<Device> Devices { get; set; }

        internal IEnumerable<DeviceDiscoveryException> Exceptions { get; set; }
    }
}
