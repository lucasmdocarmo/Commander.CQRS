namespace Commander.Playground.Tests.Contexts.Product.Events;

public sealed class ProductAddedEvent : Event
{
    public ProductAddedEvent(string productName) => ProductName = productName;

    public string ProductName { get; }
}
