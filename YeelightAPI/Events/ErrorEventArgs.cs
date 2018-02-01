using System;
using System.Collections.Generic;
using System.Text;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Notification event Argument
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ErrorEventArgs() { }

        /// <summary>
        /// Constructor with notification result
        /// </summary>
        /// <param name="result"></param>
        public ErrorEventArgs(CommandResult.CommandErrorResult result)
        {
            this.Error = result;
        }

        /// <summary>
        /// Notification Result
        /// </summary>
        public CommandResult.CommandErrorResult Error { get; set; }
    }
}
