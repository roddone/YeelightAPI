using System;
using System.Collections.Generic;
using System.Linq;
using YeelightAPI.Models;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Extensions for PROPERTIES enum
    /// </summary>
    internal static class PropertiesExtensions
    {
        #region Public Methods

        /// <summary>
        /// Get the real name of the properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<object> GetRealNames(this PROPERTIES properties)
        {
            Array vals = Enum.GetValues(typeof(PROPERTIES));
            return vals
                .Cast<PROPERTIES>()
                .Where(m => properties.HasFlag(m) && m != PROPERTIES.ALL && m != PROPERTIES.NONE)
                .Select(x => x.ToString())
                .ToList<object>();
        }

        #endregion Public Methods
    }
}