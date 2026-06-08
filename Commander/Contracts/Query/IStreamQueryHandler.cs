namespace Commander;

/// <summary>
/// Handles a query and produces an asynchronous stream of results.
/// </summary>
public interface IStreamQueryHandler<in TRequest, out TResponse>
    where TRequest : Query
    where TResponse : IQueryOutput
{
    IAsyncEnumerable<TResponse> Stream(TRequest request, CancellationToken cancellationToken = default);
}
