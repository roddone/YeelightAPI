using System;
using System.Threading;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Handler interface for CommandResult
    /// </summary>
    internal interface ICommandResultHandler
    {
        /// <summary>
        /// Type of the result
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        /// Sets the result
        /// </summary>
        /// <param name="commandResult"></param>
        void SetResult(CommandResult commandResult);

        /// <summary>
        /// Sets the error
        /// </summary>
        /// <param name="commandResultError"></param>
        void SetError(CommandResult.CommandErrorResult commandResultError);

        /// <summary>
        /// Try to cancel
        /// </summary>
        void TrySetCanceled();
    }

    /// <summary>
    /// Handler for CommandResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CommandResultHandler<T> : ICommandResultHandler
    {
        /// <summary>
        /// Task Completion Source
        /// </summary>
        private readonly TaskCompletionSource<CommandResult<T>> _tcs = new TaskCompletionSource<CommandResult<T>>();

        /// <summary>
        /// Cancellation Token Source
        /// </summary>
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(5000);

        /// <summary>
        /// Type of the result
        /// </summary>
        public Type ResultType => typeof(CommandResult<T>);

        /// <summary>
        /// Task object
        /// </summary>
        public Task<CommandResult<T>> Task => _tcs.Task;

        /// <summary>
        /// Constructor
        /// </summary>
        internal CommandResultHandler()
        {
            //automatic cancel after 5 seconds
            _cts.Token.Register(() =>
            {
                Console.WriteLine("Task cancelled (timeout)", ConsoleColor.DarkYellow);
                TrySetCanceled();
            });
        }

        /// <summary>
        /// Sets the result
        /// </summary>
        /// <param name="commandResult"></param>
        public void SetResult(CommandResult commandResult)
        {
            _tcs.SetResult((CommandResult<T>)commandResult);
            _cts?.Dispose();
        }

        /// <summary>
        /// Sets the error
        /// </summary>
        /// <param name="commandResultError"></param>
        public void SetError(CommandResult.CommandErrorResult commandResultError)
        {
            _tcs.SetException(new Exception(commandResultError.ToString()));
            _cts?.Dispose();
        }

        /// <summary>
        /// Try to cancel
        /// </summary>
        public void TrySetCanceled()
        {
            _tcs.TrySetCanceled();
            _cts?.Dispose();
        }
    }
}