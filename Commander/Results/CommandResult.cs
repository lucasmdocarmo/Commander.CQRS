namespace Commander;

/// <summary>Default <see cref="ICommandResult"/> implementation. Cached success/failure helpers minimize allocations.</summary>
public class CommandResult : ICommandResult
{
    private static readonly CommandResult SuccessSingleton = new(null, true);

    public CommandResult() : this(null, true) { }

    protected CommandResult(string? message, bool success)
    {
        Message = message;
        IsSuccess = success;
    }

    public string? Message { get; protected set; }
    public bool IsSuccess { get; protected set; }

    public static ICommandResult Success() => SuccessSingleton;
    public static ICommandResult Success(string message) => new CommandResult(message, true);
    public static ValueTask<ICommandResult> SuccessAsync() => ValueTask.FromResult<ICommandResult>(SuccessSingleton);
    public static ValueTask<ICommandResult> SuccessAsync(string message) => ValueTask.FromResult(Success(message));
    public static ICommandResult IsFailed(string message) => new CommandResult(message, false);
    public static ValueTask<ICommandResult> FailAsync(string message) => ValueTask.FromResult(IsFailed(message));
}

/// <summary>Default <see cref="ICommandResult{T}"/> implementation.</summary>
public sealed class CommandResult<T> : CommandResult, ICommandResult<T>
{
    private CommandResult(T? data, string? message, bool success) : base(message, success) => Data = data;

    public T? Data { get; }

    public static ICommandResult<T> Success(T data) => new CommandResult<T>(data, null, true);
    public static ICommandResult<T> Success(T data, string message) => new CommandResult<T>(data, message, true);
    public static ValueTask<ICommandResult<T>> SuccessAsync(T data) => ValueTask.FromResult(Success(data));
    public static ValueTask<ICommandResult<T>> SuccessAsync(T data, string message) => ValueTask.FromResult(Success(data, message));
    public static new ICommandResult<T> IsFailed(string message) => new CommandResult<T>(default, message, false);
    public static new ValueTask<ICommandResult<T>> FailAsync(string message) => ValueTask.FromResult(IsFailed(message));
}
