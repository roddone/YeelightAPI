using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI
{
    public interface IDeviceReader
    {
        object GetProp(string prop);

        Dictionary<string, object> GetProps(List<object> props);

        Dictionary<string, object> GetAllProps();
    }
}
