using BenchmarkDotNet.Attributes;
using Commander;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Commander.Benchmarks;

[MemoryDiagnoser]
[CategoriesColumn]
public class SendBenchmarks
{
    private ICommander _commander = null!;
    private IMediator _mediator = null!;
    private static readonly BenchCommand SharedCommand = new() { Value = 7 };
    private static readonly MediatrCommand SharedMediatrCommand = new() { Value = 7 };

    [GlobalSetup]
    public void Setup()
    {
        var commanderServices = new ServiceCollection();
        commanderServices.AddCommander(cfg => cfg.RegisterServicesFromAssemblyContaining<BenchCommand>());
        _commander = commanderServices.BuildServiceProvider().GetRequiredService<ICommander>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<MediatrCommand>());
        _mediator = mediatrServices.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Send")]
    public async ValueTask<int> Commander_Execute()
    {
        var result = await _commander.Execute<BenchCommand, int>(SharedCommand);
        return result.Data;
    }

    [Benchmark, BenchmarkCategory("Send")]
    public async Task<int> MediatR_Send()
    {
        return await _mediator.Send(SharedMediatrCommand);
    }
}

public sealed class BenchCommand : Command
{
    public int Value { get; init; }
}

public sealed class BenchCommandHandler : ICommandHandler<BenchCommand, int>
{
    public ValueTask<ICommandResult<int>> Execute(BenchCommand request, CancellationToken cancellationToken = default)
        => CommandResult<int>.SuccessAsync(request.Value * 2);
}

public sealed class MediatrCommand : IRequest<int>
{
    public int Value { get; init; }
}

public sealed class MediatrCommandHandler : IRequestHandler<MediatrCommand, int>
{
    public Task<int> Handle(MediatrCommand request, CancellationToken cancellationToken)
        => Task.FromResult(request.Value * 2);
}
