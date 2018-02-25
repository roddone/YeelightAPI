using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Extensions for RealNameAttribute
    /// </summary>
    internal static class RealNameAttributeExtension
    {
        #region Private Fields

        private static readonly ConcurrentDictionary<Enum, string> _realNames = new ConcurrentDictionary<Enum, string>();

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

        /// <summary>
        /// Gets the enum value with the given <see cref="RealNameAttribute"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="realName">The name of the <see cref="RealNameAttribute"/>.</param>
        /// <param name="result">The enum value.</param>
        /// <returns><see langword="true"/> if a matching enum value was found; otherwise <see langword="false"/>.</returns>
        public static bool TryParseByRealName<TEnum>(string realName, out TEnum result) where TEnum : struct
        {
            // we don't cache anything here, because
            // a) it would require for each enum type a dictionary of realName to value
            // b) the method is only used by device locator and therefore seldom called.

            foreach (FieldInfo fieldInfo in typeof(TEnum).GetFields())
            {
                RealNameAttribute attribute = fieldInfo.GetCustomAttribute<RealNameAttribute>();
                if (attribute?.PropertyName == realName)
                {
                    result = (TEnum)fieldInfo.GetValue(null);
                    return true;
                }
            }

            result = default(TEnum);
            return false;
        }

        #endregion Public Methods
    }
}