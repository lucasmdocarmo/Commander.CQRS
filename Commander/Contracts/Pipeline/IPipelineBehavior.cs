namespace Commander;

/// <summary>Delegate invoked by a pipeline behavior to continue the chain.</summary>
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

/// <summary>
/// Wraps a request handler so cross-cutting concerns (logging, validation, retries, caching, metrics, ...)
/// can be composed without polluting handler code.
/// </summary>
/// <typeparam name="TRequest">The request type (any <see cref="Command"/> or <see cref="Query"/>).</typeparam>
/// <typeparam name="TResponse">The result type returned by the next stage of the pipeline.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : Message
{
    ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
