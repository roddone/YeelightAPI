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
    }
}
