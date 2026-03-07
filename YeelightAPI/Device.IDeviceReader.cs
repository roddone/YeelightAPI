using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CronResult> CronGetAsync(CronType type = CronType.PowerOff, CancellationToken cancellationToken = default)
        {
            List<object> parameters = new List<object>() { (int)type };

            CommandResult<CronResult[]> result = await ExecuteCommandWithResponse<CronResult[]>(
                            method: METHODS.GetCron,
                            parameters: parameters,
                            cancellationToken: cancellationToken);

            return result?.Result?.FirstOrDefault();
        }

        /// <summary>
        /// Get all the properties asynchronously
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Dictionary<PROPERTIES, object>> GetAllPropsAsync(CancellationToken cancellationToken = default)
        {
            Dictionary<PROPERTIES, object> result = await GetPropsAsync(PROPERTIES.ALL, cancellationToken);

            return result;
        }

        /// <summary>
        /// Get a single property value asynchronously
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<object> GetPropAsync(PROPERTIES prop, CancellationToken cancellationToken = default)
        {
            CommandResult<List<string>> result = await ExecuteCommandWithResponse<List<string>>(
                method: METHODS.GetProp,
                parameters: new List<object>() { prop.ToString() },
                cancellationToken: cancellationToken);

            return result?.Result?.Count == 1 ? result.Result[0] : null;
        }

        /// <summary>
        /// Get multiple properties asynchronously
        /// </summary>
        /// <param name="props"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Dictionary<PROPERTIES, object>> GetPropsAsync(PROPERTIES props, CancellationToken cancellationToken = default)
        {
            List<object> names = props.GetRealNames();
            List<string> results = new List<string>();
            if (names.Count <= 20)
            {
                CommandResult<List<string>> commandResult = await ExecuteCommandWithResponse<List<string>>(
                    method: METHODS.GetProp,
                    parameters: names,
                    cancellationToken: cancellationToken
                    );

                results.AddRange(commandResult?.Result);
            }
            else
            {

                CommandResult<List<string>> commandResult1 = await ExecuteCommandWithResponse<List<string>>(
                    method: METHODS.GetProp,
                    parameters: names.Take(20).ToList(),
                    cancellationToken: cancellationToken);
                CommandResult<List<string>> commandResult2 = await ExecuteCommandWithResponse<List<string>>(
                    method: METHODS.GetProp,
                    parameters: names.Skip(20).ToList(),
                    cancellationToken: cancellationToken);

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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetNameAsync(string name, CancellationToken cancellationToken = default)
        {
            List<object> parameters = new List<object>() { name };

            CommandResult<List<string>> result = await ExecuteCommandWithResponse<List<string>>(
                            method: METHODS.SetName,
                            parameters: parameters,
                            cancellationToken: cancellationToken);

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
