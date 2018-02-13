using System;
using System.Collections.Generic;
using System.Linq;

namespace YeelightAPI.Models
{
    internal static class PropertiesExtensions
    {
        /// <summary>
        /// Get the real name of the properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<object> GetRealNames(this PROPERTIES properties)
        {
            var vals = Enum.GetValues(typeof(PROPERTIES));
            return vals
                .Cast<PROPERTIES>()
                .Where(m => properties.HasFlag(m) && m != PROPERTIES.ALL && m != PROPERTIES.NONE)
                .Select(x => x.ToString())
                .ToList<object>();
        }
    }
}