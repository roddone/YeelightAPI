using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Real-name handling Extensions
    /// </summary>
    public static class RealNameAttributeExtension
    {
        #region Private Fields

        private static ConcurrentDictionary<Enum, string> _realNames = new ConcurrentDictionary<Enum, string>();

        #endregion Private Fields

        #region Public Methods

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

        #endregion Public Methods
    }

    /// <summary>
    /// Attribute to set the real name of a Yeelight Enum
    /// </summary>
    public class RealNameAttribute : Attribute
    {
        #region Public Properties

        public string PropertyName { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public RealNameAttribute(string name)
        {
            PropertyName = name;
        }

        #endregion Public Constructors
    }
}