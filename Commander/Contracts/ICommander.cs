namespace Commander;

/// <summary>
/// Sends commands and queries to a single handler.
/// </summary>
public interface ISender
{
    /// <summary>Execute a command that returns a typed payload.</summary>
    ValueTask<ICommandResult<TResponse>> Execute<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : Command;

    /// <summary>Execute a command that does not return a payload.</summary>
    ValueTask<ICommandResult> Execute<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : Command;

    /// <summary>Execute a query and return the result.</summary>
    ValueTask<IQueryResult<TResponse>> ExecuteQuery<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : Query
        where TResponse : IQueryOutput;

    /// <summary>Execute a streaming query.</summary>
    IAsyncEnumerable<TResponse> Stream<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : Query
        where TResponse : IQueryOutput;
}

/// <summary>
/// Publishes events / notifications to zero or more handlers.
/// </summary>
public interface IPublisher
{
    /// <summary>Publish an event to all registered handlers using the configured <see cref="INotificationPublisher"/>.</summary>
    ValueTask<IEventResult> Publish<TRequest>(TRequest notification, CancellationToken cancellationToken = default)
        where TRequest : Event;
}

/// <summary>
/// Umbrella interface combining <see cref="ISender"/> and <see cref="IPublisher"/> in a single dependency
/// for callers that want one handle to the mediator (MediatR-style).
/// </summary>
public interface ICommander : ISender, IPublisher
{
}
