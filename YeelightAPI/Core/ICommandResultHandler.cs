using System;
using YeelightAPI.Models;

namespace YeelightAPI.Core
{
    /// <summary>
    /// Handler interface for CommandResult
    /// </summary>
    internal interface ICommandResultHandler
    {
        #region Public Properties

        /// <summary>
        /// Type of the result
        /// </summary>
        Type ResultType { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sets the error
        /// </summary>
        /// <param name="commandResultError"></param>
        void SetError(CommandResult.CommandErrorResult commandResultError);

        /// <summary>
        /// Sets the result
        /// </summary>
        /// <param name="commandResult"></param>
        void SetResult(CommandResult commandResult);

        /// <summary>
        /// Try to cancel
        /// </summary>
        void TrySetCanceled();

        #endregion Public Methods
    }
}