using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Commander.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Commander;

/// <summary>
/// DI registration entry points.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly Type[] HandlerOpenGenerics =
    [
        typeof(ICommandHandler<,>),
        typeof(ICommandHandler<>),
        typeof(IQueryHandler<,>),
        typeof(IStreamQueryHandler<,>),
        typeof(IEventHandler<>),
        typeof(ICommandValidator<>),
    ];

    private const string AssemblyScanAotMessage =
        "AddCommander uses Assembly.GetTypes() and reflection to discover handlers and validators. " +
        "For trim/AOT-friendly registration use the source-generated AddCommander(cfg => cfg.RegisterHandlersFrom<Assembly>()) " +
        "extension produced by Commander.SourceGenerator.";

    /// <summary>
    /// Backwards-compatible single-marker registration. Equivalent to:
    /// <c>services.AddCommander(cfg => cfg.RegisterServicesFromAssemblyContaining&lt;T&gt;());</c>
    /// </summary>
    [RequiresUnreferencedCode(AssemblyScanAotMessage)]
    [RequiresDynamicCode(AssemblyScanAotMessage)]
    public static IServiceCollection AddCommander<T>(this IServiceCollection services)
        where T : Message
        => services.AddCommander(cfg => cfg.RegisterServicesFromAssemblyContaining<T>());

    /// <summary>
    /// Modern entry point. Configure assemblies, behaviors, lifetime, and publisher strategy.
    /// </summary>
    [RequiresUnreferencedCode(AssemblyScanAotMessage)]
    [RequiresDynamicCode(AssemblyScanAotMessage)]
    public static IServiceCollection AddCommander(
        this IServiceCollection services,
        Action<CommanderServiceConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new CommanderServiceConfiguration();
        configure(configuration);

        if (configuration.AssembliesToRegister.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one assembly must be registered. Call RegisterServicesFromAssembly(...) inside AddCommander.");
        }

        services.TryAdd(new ServiceDescriptor(typeof(ICommander), typeof(CommanderMediator), configuration.Lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<ICommander>(), configuration.Lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<ICommander>(), configuration.Lifetime));
        services.TryAddSingleton(typeof(INotificationPublisher), configuration.NotificationPublisherType);

        RegisterAssemblies(services, configuration);
        RegisterOpenGenericBehaviors(services, configuration);

        return services;
    }

    [RequiresUnreferencedCode(AssemblyScanAotMessage)]
    [RequiresDynamicCode(AssemblyScanAotMessage)]
    [UnconditionalSuppressMessage("Trimming", "IL2075",
        Justification = "Caller is already warned via RequiresUnreferencedCode; AOT-friendly path goes through the source generator.")]
    [UnconditionalSuppressMessage("Trimming", "IL2072",
        Justification = "Caller is already warned via RequiresUnreferencedCode; AOT-friendly path goes through the source generator.")]
    private static void RegisterAssemblies(IServiceCollection services, CommanderServiceConfiguration configuration)
    {
        foreach (var assembly in configuration.AssembliesToRegister)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(static t => t is not null).ToArray()!;
            }

            foreach (var type in types)
            {
                if (type is null || !type.IsClass || type.IsAbstract || type.ContainsGenericParameters)
                {
                    continue;
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType)
                    {
                        continue;
                    }

                    var def = iface.GetGenericTypeDefinition();
                    if (Array.IndexOf(HandlerOpenGenerics, def) < 0)
                    {
                        continue;
                    }

                    services.TryAddEnumerable(new ServiceDescriptor(iface, type, configuration.Lifetime));
                }
            }
        }
    }

    [RequiresUnreferencedCode(AssemblyScanAotMessage)]
    [RequiresDynamicCode(AssemblyScanAotMessage)]
    [UnconditionalSuppressMessage("Trimming", "IL2075",
        Justification = "Caller is already warned via RequiresUnreferencedCode.")]
    [UnconditionalSuppressMessage("Trimming", "IL2072",
        Justification = "Caller is already warned via RequiresUnreferencedCode.")]
    private static void RegisterOpenGenericBehaviors(
        IServiceCollection services,
        CommanderServiceConfiguration configuration)
    {
        foreach (var openBehavior in configuration.BehaviorsToRegister)
        {
            var serviceType = openBehavior
                .GetInterfaces()
                .Where(i => i.IsGenericType)
                .Select(i => i.GetGenericTypeDefinition())
                .FirstOrDefault(d => d == typeof(IPipelineBehavior<,>) || d == typeof(IStreamPipelineBehavior<,>))
                ?? throw new InvalidOperationException(
                    $"{openBehavior.FullName} does not implement IPipelineBehavior<,> or IStreamPipelineBehavior<,>.");

            services.Add(new ServiceDescriptor(serviceType, openBehavior, configuration.Lifetime));
        }
    }
}
