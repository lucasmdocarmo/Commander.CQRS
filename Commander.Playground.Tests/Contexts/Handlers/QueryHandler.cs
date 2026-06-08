namespace Commander.Playground.Tests.Contexts.Handlers;

public sealed class QueryHandler : IQueryHandler<ProductQuery, ProductOutput>
{
    public ValueTask<IQueryResult<ProductOutput>> ExecuteQuery(ProductQuery request, CancellationToken cancellationToken = default)
        => QueryResult<ProductOutput>.SuccessAsync(new ProductOutput { Name = $"product-{request.Id}" });
}

public sealed class StreamingProductQueryHandler : IStreamQueryHandler<ProductQuery, ProductOutput>
{
    public async IAsyncEnumerable<ProductOutput> Stream(
        ProductQuery request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < 3; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return new ProductOutput { Name = $"item-{i}" };
            await Task.Yield();
        }
    }
}
