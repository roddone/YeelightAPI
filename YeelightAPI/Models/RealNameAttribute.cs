using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Attribute to set the real name of a Yeelight Enum
    /// </summary>
    public class RealNameAttribute : Attribute
    {
        public RealNameAttribute(string name)
        {
            PropertyName = name;
        }

        public string PropertyName { get; set; }

    }

    /// <summary>
    /// Real-name handling Extensions
    /// </summary>
    public static class RealNameAttributeExtension
    {
        private static ConcurrentDictionary<Enum, string> _realNames = new ConcurrentDictionary<Enum, string>();

        /// <summary>
        /// Retreive the RealNameAttribute of an enum value
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetRealName(this Enum enumValue)
        {
            if (_realNames.ContainsKey(enumValue))
            {
                // get from the cache
                return _realNames[enumValue];
            }

            //read the attribute
            RealNameAttribute attribute;
            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (RealNameAttribute)memberInfo.GetCustomAttributes(typeof(RealNameAttribute), false).FirstOrDefault();

                //adding to cache
                _realNames.TryAdd(enumValue, attribute.PropertyName);

                return attribute.PropertyName;
            }

            return null;
        }
    }
}
