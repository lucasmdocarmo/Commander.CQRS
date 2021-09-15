using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    public class CommandResult : ICommandResult
    {
        protected CommandResult() => IsSuccess = true;

        protected CommandResult(string message, bool success)
        {
            IsSuccess = success;
            Message = message;
        }

        public string Message { get; protected set; }
        public bool IsSuccess { get; protected set; }

        public static ICommandResult Success() => new CommandResult();

        public static ICommandResult Success(string message) => new CommandResult(message, true);

        public static ValueTask<ICommandResult> SuccessAsync() => ValueTask.FromResult(Success());

        public static ValueTask<ICommandResult> SuccessAsync(string message) => ValueTask.FromResult(Success(message));

        public static ICommandResult IsFailed(string message) => new CommandResult(message, false);

        public static ValueTask<ICommandResult> FailAsync(string message) => ValueTask.FromResult(IsFailed(message));
    }

    public sealed class CommandResult<T> : CommandResult, ICommandResult<T>
    {
        private CommandResult() => IsSuccess = true;

        private CommandResult(T data, bool success)
        {
            IsSuccess = success;
            Data = data;
        }
        protected CommandResult(string message, bool success)
        {
            IsSuccess = success;
            Message = message;
        }
        protected CommandResult(T data, string message, bool success)
        {
            IsSuccess = success;
            Message = message;
            Data = data;
        }

        public T Data { get; }

        public new static ICommandResult<T> Success() => new CommandResult<T>();

        public static ICommandResult<T> Success(T data) => new CommandResult<T>(data, true);

        public static ValueTask<ICommandResult<T>> SuccessAsync(T data) => ValueTask.FromResult(Success(data));

        public new static ICommandResult<T> IsFailed(string message) => new CommandResult<T>(message, false);

        public new static ValueTask<ICommandResult<T>> FailAsync(string message) => ValueTask.FromResult(IsFailed(message));
    }
}
