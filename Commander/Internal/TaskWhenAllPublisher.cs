namespace Commander.Internal;

/// <summary>
/// Fans out notifications to every handler in parallel and waits for them all to complete.
/// Aggregates exceptions instead of swallowing them like the previous <c>Task.WhenAll</c>-based approach did.
/// </summary>
public sealed class TaskWhenAllPublisher : INotificationPublisher
{
    public async ValueTask Publish(
        IReadOnlyList<NotificationHandlerExecutor> handlers,
        Event notification,
        CancellationToken cancellationToken)
    {
        if (handlers.Count == 0)
        {
            return;
        }

        if (handlers.Count == 1)
        {
            await handlers[0].HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
            return;
        }

        var tasks = new Task[handlers.Count];
        for (var i = 0; i < handlers.Count; i++)
        {
            tasks[i] = handlers[i].HandlerCallback(notification, cancellationToken).AsTask();
        }

        var aggregator = Task.WhenAll(tasks);
        try
        {
            await aggregator.ConfigureAwait(false);
        }
        catch
        {
            if (aggregator.Exception is { InnerExceptions.Count: > 1 } aggregate)
            {
                throw aggregate;
            }
            throw;
        }
    }
}
