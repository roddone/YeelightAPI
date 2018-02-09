using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    public partial class Device : IDeviceReader
    {

   
        /// <summary>
        /// Get a single property value asynchronously
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public async Task<object> GetProp(PROPERTIES prop)
        {
            CommandResult result = await ExecuteCommandWithResponse(
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
            List<object> names = GetPropertiesRealNames(props);

            CommandResult commandResult = await ExecuteCommandWithResponse(
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
        /// Get all the properties asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<PROPERTIES, object>> GetAllProps()
        {
            Dictionary<PROPERTIES, object> result = await GetProps(PROPERTIES.ALL);

            return result;
        }
        
    }
}
