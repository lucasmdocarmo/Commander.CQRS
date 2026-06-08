using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Commander.Benchmarks;

[MemoryDiagnoser]
[CategoriesColumn]
public class PublishBenchmarks
{
    private ICommander _commander = null!;
    private IMediator _mediator = null!;
    private static readonly BenchEvent SharedEvent = new();
    private static readonly MediatrNotification SharedNotification = new();

    [GlobalSetup]
    public void Setup()
    {
        var commanderServices = new ServiceCollection();
        commanderServices.AddCommander(cfg => cfg.RegisterServicesFromAssemblyContaining<BenchEvent>());
        _commander = commanderServices.BuildServiceProvider().GetRequiredService<ICommander>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<MediatrNotification>());
        _mediator = mediatrServices.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Publish-3-handlers")]
    public ValueTask Commander_Publish() => Discard(_commander.Publish(SharedEvent));

    [Benchmark, BenchmarkCategory("Publish-3-handlers")]
    public Task MediatR_Publish() => _mediator.Publish(SharedNotification);

    private static async ValueTask Discard(ValueTask<IEventResult> task) => _ = await task;
}

public sealed class BenchEvent : Event { }
public sealed class BenchEventHandler1 : IEventHandler<BenchEvent>
{
    public ValueTask<IEventResult> Publish(BenchEvent request, CancellationToken cancellationToken = default)
        => EventResult.SuccessAsync();
}
public sealed class BenchEventHandler2 : IEventHandler<BenchEvent>
{
    public ValueTask<IEventResult> Publish(BenchEvent request, CancellationToken cancellationToken = default)
        => EventResult.SuccessAsync();
}
public sealed class BenchEventHandler3 : IEventHandler<BenchEvent>
{
    public ValueTask<IEventResult> Publish(BenchEvent request, CancellationToken cancellationToken = default)
        => EventResult.SuccessAsync();
}

public sealed class MediatrNotification : INotification { }
public sealed class MediatrNotificationHandler1 : INotificationHandler<MediatrNotification>
{
    public Task Handle(MediatrNotification notification, CancellationToken cancellationToken) => Task.CompletedTask;
}
public sealed class MediatrNotificationHandler2 : INotificationHandler<MediatrNotification>
{
    public Task Handle(MediatrNotification notification, CancellationToken cancellationToken) => Task.CompletedTask;
}
public sealed class MediatrNotificationHandler3 : INotificationHandler<MediatrNotification>
{
    public Task Handle(MediatrNotification notification, CancellationToken cancellationToken) => Task.CompletedTask;
}
