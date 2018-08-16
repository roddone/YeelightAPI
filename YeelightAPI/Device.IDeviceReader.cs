using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YeelightAPI.Core;
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
                            parameters: parameters);

            return result?.Result?.FirstOrDefault();
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
                parameters: new List<object>() { prop.ToString() } );

            return result?.Result?.Count == 1 ? result.Result[0] : null;
        }

        /// <summary>
        /// Get multiple properties asynchronously
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public async Task<Dictionary<PROPERTIES, object>> GetProps(PROPERTIES props)
        {
            List<object> names = props.GetRealNames();
            List<string> results = new List<string>();
            if (names.Count <= 20)
            {
                CommandResult<List<string>> commandResult = await ExecuteCommandWithResponse<List<string>>(
                    method: METHODS.GetProp,
                    parameters: names
                    );

                results.AddRange(commandResult?.Result);
            }
            else
            {

                CommandResult<List<string>> commandResult1 = await ExecuteCommandWithResponse<List<string>>(
                    method: METHODS.GetProp,
                    parameters: names.Take(20).ToList() );
                CommandResult<List<string>> commandResult2 = await ExecuteCommandWithResponse<List<string>>(
                    method: METHODS.GetProp,
                    parameters: names.Skip(20).ToList());

                results.AddRange(commandResult1?.Result);
                results.AddRange(commandResult2?.Result);
            }

            if (results.Count > 0)
            {
                Dictionary<PROPERTIES, object> result = new Dictionary<PROPERTIES, object>();

                for (int n = 0; n < names.Count; n++)
                {
                    string name = names[n].ToString();

                    if (Enum.TryParse<PROPERTIES>(name, out PROPERTIES p))
                    {
                        result.Add(p, results[n]);
                    }
                }

                return result;
            }
            return null;
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
                            parameters: parameters);

            if (result.IsOk())
            {
                Name = name;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Public Methods
    }
}