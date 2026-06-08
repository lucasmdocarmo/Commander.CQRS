namespace Commander;

/// <summary>Handles a query and returns a typed payload.</summary>
public interface IQueryHandler<in TRequest, TResponse>
    where TRequest : Query
    where TResponse : IQueryOutput
{
    ValueTask<IQueryResult<TResponse>> ExecuteQuery(TRequest request, CancellationToken cancellationToken = default);
}
