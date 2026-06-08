using Commander.Playground.Tests.Contexts.Handlers;
using Commander.Playground.Tests.Contexts.Product.Events;

namespace Commander.Playground.Tests;

public sealed class ProductEventHandler : IEventHandler<ProductAddedEvent>, IEventHandler<ProductDeletedEvent>
{
    public static int AddedCount;
    public static int DeletedCount;

    public ValueTask<IEventResult> Publish(ProductAddedEvent request, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref AddedCount);
        return EventResult.SuccessAsync();
    }

    public ValueTask<IEventResult> Publish(ProductDeletedEvent request, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref DeletedCount);
        return EventResult.SuccessAsync("Event Published!");
    }
}

public sealed class SecondaryProductAddedHandler : IEventHandler<ProductAddedEvent>
{
    public static int InvocationCount;

    public ValueTask<IEventResult> Publish(ProductAddedEvent request, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref InvocationCount);
        return EventResult.SuccessAsync();
    }
}
