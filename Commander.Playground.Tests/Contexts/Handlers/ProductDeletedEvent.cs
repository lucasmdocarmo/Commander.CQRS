namespace Commander.Playground.Tests.Contexts.Handlers;

public sealed class ProductDeletedEvent : Event
{
    public ProductDeletedEvent(Guid id) => Id = id;

    public Guid Id { get; }
}
