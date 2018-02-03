using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    public interface IDeviceReader
    {
        object GetProp(PROPERTIES prop);

        Dictionary<PROPERTIES, object> GetProps(PROPERTIES props);

        Dictionary<PROPERTIES, object> GetAllProps();

        Task<object> GetPropAsync(PROPERTIES prop);

        Task<Dictionary<PROPERTIES, object>> GetPropsAsync(PROPERTIES props);

        Task<Dictionary<PROPERTIES, object>> GetAllPropsAsync();
    }
}
