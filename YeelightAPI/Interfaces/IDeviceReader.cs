using System.Collections.Generic;
using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Cron;

namespace YeelightAPI
{
    public interface IDeviceReader
    {
        #region Public Methods

        Task<CronResult> CronGet(CronType type = CronType.PowerOff);

        Task<Dictionary<PROPERTIES, object>> GetAllProps();

        Task<object> GetProp(PROPERTIES prop);

        Task<Dictionary<PROPERTIES, object>> GetProps(PROPERTIES props);

        Task<bool> SetName(string name);

        #endregion Public Methods
    }
}