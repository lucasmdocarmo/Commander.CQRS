using System.Reflection;
using Commander.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Commander;

/// <summary>
/// Options used by <see cref="ServiceCollectionExtensions.AddCommander"/> to drive registration.
/// </summary>
public sealed class CommanderServiceConfiguration
{
    /// <summary>Assemblies that will be scanned for handlers, validators and pipeline behaviors.</summary>
    public List<Assembly> AssembliesToRegister { get; } = new();

    /// <summary>
    /// DI lifetime applied to handlers, validators and pipeline behaviors.
    /// Default is <see cref="ServiceLifetime.Scoped"/> (the most common case for app-level handlers).
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

    /// <summary>Strategy used to dispatch events to multiple handlers. Default: <see cref="ForeachAwaitPublisher"/>.</summary>
    public Type NotificationPublisherType { get; set; } = typeof(ForeachAwaitPublisher);

    /// <summary>Open-generic pipeline behaviors registered in declaration order.</summary>
    public List<Type> BehaviorsToRegister { get; } = new();

    /// <summary>Add an assembly to scan for handlers, validators and pipeline behaviors.</summary>
    public CommanderServiceConfiguration RegisterServicesFromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        if (!AssembliesToRegister.Contains(assembly))
        {
            AssembliesToRegister.Add(assembly);
        }
        return this;
    }

    /// <summary>Add the assembly that contains <typeparamref name="TMarker"/> to the scan list.</summary>
    public CommanderServiceConfiguration RegisterServicesFromAssemblyContaining<TMarker>()
        => RegisterServicesFromAssembly(typeof(TMarker).Assembly);

    /// <summary>Add the assembly that contains <paramref name="markerType"/> to the scan list.</summary>
    public CommanderServiceConfiguration RegisterServicesFromAssemblyContaining(Type markerType)
        => RegisterServicesFromAssembly(markerType.Assembly);

    /// <summary>Bulk-add multiple assemblies to scan.</summary>
    public CommanderServiceConfiguration RegisterServicesFromAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            RegisterServicesFromAssembly(assembly);
        }
        return this;
    }

    /// <summary>
    /// Register an open-generic pipeline behavior, e.g.
    /// <c>cfg.AddOpenBehavior(typeof(LoggingBehavior&lt;,&gt;))</c>.
    /// </summary>
    public CommanderServiceConfiguration AddOpenBehavior(Type openGenericBehavior)
    {
        ArgumentNullException.ThrowIfNull(openGenericBehavior);
        if (!openGenericBehavior.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                $"Behavior {openGenericBehavior.FullName} must be an open generic type definition (e.g. typeof(MyBehavior<,>))",
                nameof(openGenericBehavior));
        }
        BehaviorsToRegister.Add(openGenericBehavior);
        return this;
    }

    /// <summary>Use a custom <see cref="INotificationPublisher"/> implementation.</summary>
    public CommanderServiceConfiguration UseNotificationPublisher<TPublisher>()
        where TPublisher : class, INotificationPublisher
    {
        NotificationPublisherType = typeof(TPublisher);
        return this;
    }
}
