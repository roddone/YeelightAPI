using System;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Notification event Argument
    /// </summary>
    public class CommandErrorEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Notification Result
        /// </summary>
        public CommandResult.CommandErrorResult Error { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandErrorEventArgs() { }

        /// <summary>
        /// Constructor with notification result
        /// </summary>
        /// <param name="result"></param>
        public CommandErrorEventArgs(CommandResult.CommandErrorResult result)
        {
            Error = result;
        }

        #endregion Public Constructors
    }
}