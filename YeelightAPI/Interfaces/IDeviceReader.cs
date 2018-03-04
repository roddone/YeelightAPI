using System.Collections.Generic;
using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Cron;

namespace YeelightAPI
{
    /// <summary>
    /// Descriptor for Device Reading operations
    /// </summary>
    public interface IDeviceReader
    {
        #region Public Methods

        /// <summary>
        /// Get a cron task
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<CronResult> CronGet(CronType type = CronType.PowerOff);

        /// <summary>
        /// Get all properties values
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<PROPERTIES, object>> GetAllProps();

        /// <summary>
        /// Get a single property value
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        Task<object> GetProp(PROPERTIES prop);

        /// <summary>
        /// Get multiple properties values
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        Task<Dictionary<PROPERTIES, object>> GetProps(PROPERTIES props);

        /// <summary>
        /// Set the name of the device
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> SetName(string name);

        #endregion Public Methods
    }
}