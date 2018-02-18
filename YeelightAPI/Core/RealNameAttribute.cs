using System;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Attribute to set the real name of a Yeelight Enum
    /// </summary>
    internal class RealNameAttribute : Attribute
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