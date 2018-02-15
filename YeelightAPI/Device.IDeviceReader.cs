using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YeelightAPI.Models;
using YeelightAPI.Models.Cron;

namespace YeelightAPI
{
    /// <summary>
    /// Yeelight Device : IDeviceReader implementation
    /// </summary>
    public partial class Device : IDeviceReader
    {
        #region Public Methods

        /// <summary>
        /// Get a cron JOB
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<CronResult> CronGet(CronType type = CronType.PowerOff)
        {
            List<object> parameters = new List<object>() { (int)type };

            CommandResult<CronResult[]> result = await ExecuteCommandWithResponse<CronResult[]>(
                            method: METHODS.GetCron,
                            id: (int)METHODS.GetCron,
                            parameters: parameters);

            return result?.Result.FirstOrDefault();
        }

        /// <summary>
        /// Get all the properties asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<PROPERTIES, object>> GetAllProps()
        {
            Dictionary<PROPERTIES, object> result = await GetProps(PROPERTIES.ALL);

            return result;
        }

        /// <summary>
        /// Get a single property value asynchronously
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public async Task<object> GetProp(PROPERTIES prop)
        {
            CommandResult<List<string>> result = await ExecuteCommandWithResponse<List<string>>(
                method: METHODS.GetProp,
                id: (int)METHODS.GetProp,
                parameters: new List<object>() { prop.ToString() }
                );

            return result.Result != null && result.Result.Count == 1 ? result.Result[0] : null;
        }

        /// <summary>
        /// Get multiple properties asynchronously
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public async Task<Dictionary<PROPERTIES, object>> GetProps(PROPERTIES props)
        {
            List<object> names = props.GetRealNames();

            CommandResult<List<string>> commandResult = await ExecuteCommandWithResponse<List<string>>(
                method: METHODS.GetProp,
                id: ((int)METHODS.GetProp),// + 1000 + props.Count,
                parameters: names
                );

            Dictionary<PROPERTIES, object> result = new Dictionary<PROPERTIES, object>();

            for (int n = 0; n < names.Count; n++)
            {
                string name = names[n].ToString();

                if (Enum.TryParse<PROPERTIES>(name, out PROPERTIES p))
                {
                    result.Add(p, commandResult.Result[n]);
                }
            }

            return result;
        }

        /// <summary>
        /// Set the name of the device
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<bool> SetName(string name)
        {
            List<object> parameters = new List<object>() { name };

            CommandResult<List<string>> result = await ExecuteCommandWithResponse<List<string>>(
                            method: METHODS.SetName,
                            id: (int)METHODS.SetName,
                            parameters: parameters);

            if (result.IsOk())
            {
                Name = name;
                return true;
            }
            else
            {
                return result.IsOk();
            }
        }

        #endregion Public Methods
    }
}