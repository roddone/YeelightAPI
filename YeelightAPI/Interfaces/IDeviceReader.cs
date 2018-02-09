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

        Task<object> GetProp(PROPERTIES prop);

        Task<Dictionary<PROPERTIES, object>> GetProps(PROPERTIES props);

        Task<Dictionary<PROPERTIES, object>> GetAllProps();

        Task<bool> SetName(string name);
    }
}
