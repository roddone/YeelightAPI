using System.Collections.Generic;

namespace YeelightAPI.Models
{
    /// <summary>
    /// Extensions for CommandResult
    /// </summary>
    public static class CommandResultExtensions
    {
        #region Public Methods

        /// <summary>
        /// Determine if the result is a classical OK result ({"id":1, "result":["ok"]})
        /// </summary>
        /// <remarks>
        /// returns true by default if music mode is enabled
        /// </remarks>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsOk(this CommandResult<List<string>> @this)
        {
            return @this?.IsMusicResponse == true || (@this?.Error == null && @this?.Result?[0] == "ok");
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Default command result
    /// </summary>
    public class CommandResult
    {
        #region Public Properties

        /// <summary>
        /// Error, null if command is successful
        /// </summary>
        public CommandErrorResult Error { get; set; }

        /// <summary>
        /// Request Id (mirrored from the sent request)
        /// </summary>
        public int Id { get; set; }

        #endregion Public Properties

        #region Internal Properties

        internal bool IsMusicResponse { get; set; }

        #endregion Internal Properties

        #region Public Classes

        /// <summary>
        /// Error model
        /// </summary>
        public class CommandErrorResult
        {
            #region Public Properties

            /// <summary>
            /// Error code
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// Error message
            /// </summary>
            public string Message { get; set; }

            #endregion Public Properties

            #region Public Methods

            /// <summary>
            /// ToString override
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Code} - {Message}";
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }

    /// <summary>
    /// Result received after a Command has been sent
    /// </summary>
    public class CommandResult<T> : CommandResult
    {
        #region Public Properties

        /// <summary>
        /// Result
        /// </summary>
        public T Result { get; set; }

        #endregion Public Properties
    }
}