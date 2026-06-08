namespace Commander;

/// <summary>Strategy for invoking a set of event handlers.</summary>
public interface INotificationPublisher
{
    /// <summary>Invoke each handler. Implementations decide on ordering / parallelism / fault propagation.</summary>
    ValueTask Publish(
        IReadOnlyList<NotificationHandlerExecutor> handlers,
        Event notification,
        CancellationToken cancellationToken);
}

/// <summary>Pre-bound execution unit consumed by an <see cref="INotificationPublisher"/>.</summary>
/// <param name="HandlerInstance">The resolved handler instance.</param>
/// <param name="HandlerCallback">Async callback that invokes the handler with the notification.</param>
public readonly record struct NotificationHandlerExecutor(
    object HandlerInstance,
    Func<Event, CancellationToken, ValueTask> HandlerCallback);
