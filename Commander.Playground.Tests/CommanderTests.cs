using Commander.FluentValidation;
using Commander.Internal;
using Commander.Playground.Tests.Contexts.Behaviors;
using Commander.Playground.Tests.Contexts.Category;
using Commander.Playground.Tests.Contexts.Product.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Commander.Playground.Tests;

public sealed class CommanderTests
{
    private static IServiceProvider BuildProvider(Action<CommanderServiceConfiguration>? extra = null)
    {
        var services = new ServiceCollection();
        services.AddCommander(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AddProductCommand>();
            cfg.AddFluentValidation();
            extra?.Invoke(cfg);
        });
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Command_ReturnsSuccess_WhenValidationPasses()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();

        var result = await commander.Execute<AddProductCommand, Product>(new AddProductCommand { Name = "notebook" });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("notebook", result.Data!.Name);
    }

    [Fact]
    public async Task Command_ReturnsFailure_WhenValidationFails()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();

        var result = await commander.Execute<AddProductCommand, Product>(new AddProductCommand { Name = "" });

        Assert.False(result.IsSuccess);
        Assert.False(string.IsNullOrEmpty(result.Message));
    }

    [Fact]
    public async Task Query_ReturnsExpectedPayload()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();
        var id = Guid.NewGuid();

        var result = await commander.ExecuteQuery<ProductQuery, ProductOutput>(new ProductQuery(id));

        Assert.True(result.IsSuccess);
        Assert.Equal($"product-{id}", result.Data!.Name);
    }

    [Fact]
    public async Task StreamingQuery_YieldsAllItems()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();

        var items = new List<ProductOutput>();
        await foreach (var item in commander.Stream<ProductQuery, ProductOutput>(new ProductQuery(Guid.NewGuid())))
        {
            items.Add(item);
        }

        Assert.Equal(3, items.Count);
    }

    [Fact]
    public async Task StreamPipelineBehavior_WrapsTheStream()
    {
        StreamLoggingBehavior<ProductQuery, ProductOutput>.InvocationCount = 0;
        StreamLoggingBehavior<ProductQuery, ProductOutput>.ItemsObserved = 0;

        var commander = BuildProvider(cfg => cfg.AddOpenBehavior(typeof(StreamLoggingBehavior<,>)))
            .GetRequiredService<ICommander>();

        var items = new List<ProductOutput>();
        await foreach (var item in commander.Stream<ProductQuery, ProductOutput>(new ProductQuery(Guid.NewGuid())))
        {
            items.Add(item);
        }

        Assert.Equal(3, items.Count);
        Assert.Equal(1, StreamLoggingBehavior<ProductQuery, ProductOutput>.InvocationCount);
        Assert.Equal(3, StreamLoggingBehavior<ProductQuery, ProductOutput>.ItemsObserved);
    }

    [Fact]
    public async Task Publish_InvokesAllHandlers_WithDefaultForeachAwaitPublisher()
    {
        ProductEventHandler.AddedCount = 0;
        SecondaryProductAddedHandler.InvocationCount = 0;
        var commander = BuildProvider().GetRequiredService<ICommander>();

        var result = await commander.Publish(new ProductAddedEvent("test"));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, ProductEventHandler.AddedCount);
        Assert.Equal(1, SecondaryProductAddedHandler.InvocationCount);
    }

    [Fact]
    public async Task Publish_WithParallelStrategy_StillInvokesAllHandlers()
    {
        ProductEventHandler.AddedCount = 0;
        SecondaryProductAddedHandler.InvocationCount = 0;
        var commander = BuildProvider(cfg => cfg.UseNotificationPublisher<TaskWhenAllPublisher>())
            .GetRequiredService<ICommander>();

        var result = await commander.Publish(new ProductAddedEvent("test"));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, ProductEventHandler.AddedCount);
        Assert.Equal(1, SecondaryProductAddedHandler.InvocationCount);
    }

    [Fact]
    public async Task Publish_WithoutSubscribers_StillSucceeds()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();

        var result = await commander.Publish(new UnhandledEvent());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task PipelineBehavior_RunsAroundHandler()
    {
        LoggingBehavior<AddProductCommand, ICommandResult<Product>>.InvocationCount = 0;
        LoggingBehavior<AddProductCommand, ICommandResult<Product>>.Log.Clear();

        var commander = BuildProvider(cfg => cfg.AddOpenBehavior(typeof(LoggingBehavior<,>)))
            .GetRequiredService<ICommander>();

        var result = await commander.Execute<AddProductCommand, Product>(new AddProductCommand { Name = "lap" });

        Assert.True(result.IsSuccess);
        Assert.Equal(1, LoggingBehavior<AddProductCommand, ICommandResult<Product>>.InvocationCount);
        Assert.Equal(["BEFORE AddProductCommand", "AFTER AddProductCommand"],
            LoggingBehavior<AddProductCommand, ICommandResult<Product>>.Log);
    }

    [Fact]
    public async Task Cancellation_TokenIsHonored_OnStream()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var token = cts.Token;
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in commander.Stream<ProductQuery, ProductOutput>(new ProductQuery(Guid.NewGuid()), token))
            {
            }
        });
    }

    [Fact]
    public void MissingHandler_ThrowsCommandException()
    {
        var commander = BuildProvider().GetRequiredService<ICommander>();

        Assert.Throws<CommandException>(() =>
        {
            commander.Execute<UnregisteredCommand, Product>(new UnregisteredCommand()).GetAwaiter().GetResult();
        });
    }

    [Fact]
    public async Task GeneratedRegistration_RegistersHandlersWithoutReflection()
    {
        var services = new ServiceCollection();
        services.AddCommander(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AddProductCommand>();
            cfg.AddFluentValidation();
        });
        services.AddCommanderHandlers();

        var commander = services.BuildServiceProvider().GetRequiredService<ICommander>();

        var result = await commander.Execute<AddProductCommand, Product>(new AddProductCommand { Name = "via-srcgen" });

        Assert.True(result.IsSuccess);
    }

    public sealed class UnregisteredCommand : Command { }

    public sealed class UnhandledEvent : Event { }
}
