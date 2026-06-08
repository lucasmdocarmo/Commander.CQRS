namespace Commander;

public interface ICommandResult
{
    bool IsSuccess { get; }
    string? Message { get; }
}

public interface ICommandResult<out T> : ICommandResult
{
    T? Data { get; }
}
