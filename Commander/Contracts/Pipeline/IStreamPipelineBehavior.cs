namespace Commander;

/// <summary>Delegate that produces the underlying stream when invoked by the next stage of the streaming pipeline.</summary>
public delegate IAsyncEnumerable<TResponse> StreamHandlerDelegate<out TResponse>(CancellationToken cancellationToken);

/// <summary>
/// Wraps a <see cref="IStreamQueryHandler{TRequest, TResponse}"/> so cross-cutting concerns
/// (logging, metrics, timeouts, transforms) can be composed around the streamed result.
/// </summary>
public interface IStreamPipelineBehavior<in TRequest, TResponse>
    where TRequest : Query
    where TResponse : IQueryOutput
{
    IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
