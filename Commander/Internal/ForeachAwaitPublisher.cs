namespace Commander.Internal;

/// <summary>
/// Awaits each event handler in registration order. The first throwing handler aborts the publish call.
/// Default and recommended publisher: deterministic, easy to reason about, no extra task/scheduler overhead.
/// </summary>
public sealed class ForeachAwaitPublisher : INotificationPublisher
{
    public async ValueTask Publish(
        IReadOnlyList<NotificationHandlerExecutor> handlers,
        Event notification,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < handlers.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await handlers[i].HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}
