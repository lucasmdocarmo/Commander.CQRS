namespace Commander.Playground.Tests;

public sealed class ProductQuery : Query
{
    public ProductQuery(Guid id) => Id = id;

    public Guid Id { get; }
}
