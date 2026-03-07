using System.Collections.Generic;
using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CronResult> CronGetAsync(CronType type = CronType.PowerOff, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all properties values
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<PROPERTIES, object>> GetAllPropsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a single property value
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object> GetPropAsync(PROPERTIES prop, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get multiple properties values
        /// </summary>
        /// <param name="props"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<PROPERTIES, object>> GetPropsAsync(PROPERTIES props, CancellationToken cancellationToken = default);

        /// <summary>
        /// Set the name of the device
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SetNameAsync(string name, CancellationToken cancellationToken = default);

        #endregion Public Methods
    }
}
