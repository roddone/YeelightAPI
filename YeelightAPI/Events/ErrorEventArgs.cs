using System;
using System.Collections.Generic;
using System.Text;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Notification event Argument
    /// </summary>
    public class CommandErrorEventArgs : EventArgs
    {
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

        /// <summary>
        /// Notification Result
        /// </summary>
        public CommandResult.CommandErrorResult Error { get; set; }

    }
}
