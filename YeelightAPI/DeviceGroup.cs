using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    /// <summary>
    /// Group of Yeelight Devices
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
            Name = name;
        }

        /// <summary>
        /// Constructor with one device
        /// </summary>
        /// <param name="device"></param>
        public DeviceGroup(Device device, string name = null)
        {
            Add(device);
            Name = name;
        }

        /// <summary>
        /// Constructor with devices as params
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(params Device[] devices)
        {
            AddRange(devices);
        }

        /// <summary>
        /// Constructor with a list (IEnumerable) of devices
        /// </summary>
        /// <param name="devices"></param>
        public DeviceGroup(IEnumerable<Device> devices, string name = null)
        {
            AddRange(devices);
            Name = name;
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

        /// <summary>
        /// Execute code for all the devices
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        protected async Task<bool> Process(Func<Device, Task<bool>> f)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();

            foreach (Device device in this)
            {
                tasks.Add(f(device));
            }

            await Task.WhenAll(tasks);

            foreach (Task<bool> t in tasks)
            {
                result &= t.Result;
            }

            return result;
        }

    }
}
