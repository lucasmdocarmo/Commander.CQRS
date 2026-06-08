namespace Commander;

/// <summary>Handles a command and returns a typed payload.</summary>
public interface ICommandHandler<in TRequest, TResponse> where TRequest : Command
{
    ValueTask<ICommandResult<TResponse>> Execute(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>Handles a command without producing a payload.</summary>
public interface ICommandHandler<in TRequest> where TRequest : Command
{
    ValueTask<ICommandResult> Execute(TRequest request, CancellationToken cancellationToken = default);
}
