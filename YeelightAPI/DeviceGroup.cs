using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    /// <summary>
    /// Group of devices
    /// </summary>
    public partial class DeviceGroup : List<Device>, IDisposable
    {

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; set; }

        #endregion PUBLIC PROPERTIES

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="device"></param>
        public DeviceGroup(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="device"></param>
        public DeviceGroup(Device device, string name = null)
        {
            this.Add(device);
            this.Name = name;
        }

        /// <summary>
        /// Constructor with devices as params
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(params Device[] devices)
        {
            this.AddRange(devices);
        }

        /// <summary>
        /// Constructor with a list (IEnumerable) of devices
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(IEnumerable<Device> devices, string name = null)
        {
            this.AddRange(devices);
            this.Name = name;
        }

        #endregion CONSTRUCTOR

        #region IDisposable

        /// <summary>
        /// Dispose the devices
        /// </summary>
        public void Dispose()
        {
            foreach(Device device in this)
            {
                device.Dispose();
            }
        }

        #endregion

    }
}
