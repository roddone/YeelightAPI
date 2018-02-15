using System;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    internal interface ICommandResultHandler
    {
        Type ResultType { get; }

        void SetResult(CommandResult commandResult);

        void SetError(CommandResult.CommandErrorResult commandResultError);
        void TrySetCanceled();
    }

    internal class CommandResultHandler<T> : ICommandResultHandler
    {
        private readonly TaskCompletionSource<CommandResult<T>> _tcs = new TaskCompletionSource<CommandResult<T>>();

        public Type ResultType => typeof(CommandResult<T>);

        public void SetResult(CommandResult commandResult)
        {
            _tcs.SetResult((CommandResult<T>)commandResult);
        }

        public void SetError(CommandResult.CommandErrorResult commandResultError)
        {
            _tcs.SetException(new Exception(commandResultError.ToString()));
        }

        public void TrySetCanceled()
        {
            _tcs.TrySetCanceled();
        }

        public Task<CommandResult<T>> Task => _tcs.Task;
    }
}