using Microsoft.Extensions.DependencyInjection;

namespace Commander.Internal;

/// <summary>
/// Default <see cref="ICommander"/> implementation.
/// <para>
/// Dispatch is fully generic so the JIT specializes each call site and avoids reflection on the hot path.
/// Pipeline behaviors are composed into a delegate chain per call; an event handler list is materialized
/// once per <see cref="Publish"/> call and forwarded to the configured <see cref="INotificationPublisher"/>.
/// </para>
/// </summary>
internal sealed class CommanderMediator : ICommander
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationPublisher _notificationPublisher;

    public CommanderMediator(IServiceProvider serviceProvider, INotificationPublisher notificationPublisher)
    {
        _serviceProvider = serviceProvider;
        _notificationPublisher = notificationPublisher;
    }

    public ValueTask<ICommandResult<TResponse>> Execute<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : Command
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = _serviceProvider.GetService<ICommandHandler<TRequest, TResponse>>()
            ?? throw new CommandException(
                $"No ICommandHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}> registered.");

        var behaviors = ResolveBehaviors<TRequest, ICommandResult<TResponse>>();

        RequestHandlerDelegate<ICommandResult<TResponse>> pipeline = ct => handler.Execute(request, ct);
        pipeline = BuildPipeline(behaviors, request, pipeline);

        return ExecuteWithExceptionShield(pipeline, request, handler, cancellationToken);

        static async ValueTask<ICommandResult<TResponse>> ExecuteWithExceptionShield(
            RequestHandlerDelegate<ICommandResult<TResponse>> pipeline,
            TRequest request,
            ICommandHandler<TRequest, TResponse> handler,
            CancellationToken ct)
        {
            try
            {
                return await pipeline(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (CommandException) { throw; }
            catch (Exception ex)
            {
                throw new CommandException(
                    $"An error occurred while executing command {typeof(TRequest).Name} in handler {handler.GetType().Name}.",
                    ex);
            }
        }
    }

    public ValueTask<ICommandResult> Execute<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : Command
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = _serviceProvider.GetService<ICommandHandler<TRequest>>()
            ?? throw new CommandException($"No ICommandHandler<{typeof(TRequest).Name}> registered.");

        var behaviors = ResolveBehaviors<TRequest, ICommandResult>();

        RequestHandlerDelegate<ICommandResult> pipeline = ct => handler.Execute(request, ct);
        pipeline = BuildPipeline(behaviors, request, pipeline);

        return ExecuteWithExceptionShield(pipeline, request, handler, cancellationToken);

        static async ValueTask<ICommandResult> ExecuteWithExceptionShield(
            RequestHandlerDelegate<ICommandResult> pipeline,
            TRequest request,
            ICommandHandler<TRequest> handler,
            CancellationToken ct)
        {
            try
            {
                return await pipeline(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (CommandException) { throw; }
            catch (Exception ex)
            {
                throw new CommandException(
                    $"An error occurred while executing command {typeof(TRequest).Name} in handler {handler.GetType().Name}.",
                    ex);
            }
        }
    }

    public ValueTask<IQueryResult<TResponse>> ExecuteQuery<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : Query
        where TResponse : IQueryOutput
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = _serviceProvider.GetService<IQueryHandler<TRequest, TResponse>>()
            ?? throw new CommandException(
                $"No IQueryHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}> registered.");

        var behaviors = ResolveBehaviors<TRequest, IQueryResult<TResponse>>();

        RequestHandlerDelegate<IQueryResult<TResponse>> pipeline = ct => handler.ExecuteQuery(request, ct);
        pipeline = BuildPipeline(behaviors, request, pipeline);

        return ExecuteWithExceptionShield(pipeline, request, handler, cancellationToken);

        static async ValueTask<IQueryResult<TResponse>> ExecuteWithExceptionShield(
            RequestHandlerDelegate<IQueryResult<TResponse>> pipeline,
            TRequest request,
            IQueryHandler<TRequest, TResponse> handler,
            CancellationToken ct)
        {
            try
            {
                return await pipeline(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (CommandException) { throw; }
            catch (Exception ex)
            {
                throw new CommandException(
                    $"An error occurred while executing query {typeof(TRequest).Name} in handler {handler.GetType().Name}.",
                    ex);
            }
        }
    }

    public IAsyncEnumerable<TResponse> Stream<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : Query
        where TResponse : IQueryOutput
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = _serviceProvider.GetService<IStreamQueryHandler<TRequest, TResponse>>()
            ?? throw new CommandException(
                $"No IStreamQueryHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}> registered.");

        var behaviors = ResolveStreamBehaviors<TRequest, TResponse>();
        if (behaviors.Length == 0)
        {
            return handler.Stream(request, cancellationToken);
        }

        StreamHandlerDelegate<TResponse> pipeline = ct => handler.Stream(request, ct);
        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var current = pipeline;
            pipeline = ct => behavior.Handle(request, current, ct);
        }
        return pipeline(cancellationToken);
    }

    private IStreamPipelineBehavior<TRequest, TResponse>[] ResolveStreamBehaviors<TRequest, TResponse>()
        where TRequest : Query
        where TResponse : IQueryOutput
    {
        var enumerable = _serviceProvider.GetServices<IStreamPipelineBehavior<TRequest, TResponse>>();
        return enumerable as IStreamPipelineBehavior<TRequest, TResponse>[]
            ?? [.. enumerable];
    }

    public ValueTask<IEventResult> Publish<TRequest>(
        TRequest notification,
        CancellationToken cancellationToken = default)
        where TRequest : Event
    {
        ArgumentNullException.ThrowIfNull(notification);

        var rawHandlers = _serviceProvider.GetServices<IEventHandler<TRequest>>();
        var executors = MaterializeExecutors(rawHandlers);

        if (executors.Count == 0)
        {
            return ValueTask.FromResult(EventResult.Success());
        }

        return PublishCore(executors, notification, cancellationToken);

        async ValueTask<IEventResult> PublishCore(
            IReadOnlyList<NotificationHandlerExecutor> handlers,
            TRequest evt,
            CancellationToken ct)
        {
            try
            {
                await _notificationPublisher.Publish(handlers, evt, ct).ConfigureAwait(false);
                return EventResult.Success();
            }
            catch (OperationCanceledException) { throw; }
            catch (EventException) { throw; }
            catch (Exception ex)
            {
                throw new EventException(
                    $"An error occurred while publishing event {typeof(TRequest).Name}.", ex);
            }
        }
    }

    private IPipelineBehavior<TRequest, TResponse>[] ResolveBehaviors<TRequest, TResponse>()
        where TRequest : Message
    {
        var enumerable = _serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>();
        return enumerable as IPipelineBehavior<TRequest, TResponse>[]
            ?? [.. enumerable];
    }

    private static RequestHandlerDelegate<TResponse> BuildPipeline<TRequest, TResponse>(
        IPipelineBehavior<TRequest, TResponse>[] behaviors,
        TRequest request,
        RequestHandlerDelegate<TResponse> terminal)
        where TRequest : Message
    {
        if (behaviors.Length == 0)
        {
            return terminal;
        }

        var next = terminal;
        for (var i = behaviors.Length - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var current = next;
            next = ct => behavior.Handle(request, current, ct);
        }
        return next;
    }

    private static IReadOnlyList<NotificationHandlerExecutor> MaterializeExecutors<TRequest>(
        IEnumerable<IEventHandler<TRequest>> handlers)
        where TRequest : Event
    {
        if (handlers is IEventHandler<TRequest>[] array)
        {
            if (array.Length == 0)
            {
                return Array.Empty<NotificationHandlerExecutor>();
            }

            var result = new NotificationHandlerExecutor[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                var handler = array[i];
                result[i] = new NotificationHandlerExecutor(
                    handler,
                    (evt, ct) => DispatchAsync(handler, (TRequest)evt, ct));
            }
            return result;
        }

        var list = new List<NotificationHandlerExecutor>();
        foreach (var handler in handlers)
        {
            var captured = handler;
            list.Add(new NotificationHandlerExecutor(
                captured,
                (evt, ct) => DispatchAsync(captured, (TRequest)evt, ct)));
        }
        return list;
    }

    private static async ValueTask DispatchAsync<TRequest>(
        IEventHandler<TRequest> handler,
        TRequest notification,
        CancellationToken cancellationToken)
        where TRequest : Event
    {
        await handler.Publish(notification, cancellationToken).ConfigureAwait(false);
    }
}
