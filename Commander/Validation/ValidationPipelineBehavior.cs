using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Commander.Validation;

/// <summary>
/// Pipeline behavior that runs every registered <see cref="ICommandValidator{T}"/> for the
/// request type before the handler executes. Short-circuits the pipeline with a failed result
/// if any validator returns <c>IsSuccess == false</c>.
/// </summary>
/// <remarks>
/// <para>
/// Register via <c>cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior&lt;,&gt;));</c>. Works for both
/// <see cref="ICommandResult"/> and <see cref="ICommandResult{T}"/> response shapes; for any other
/// <typeparamref name="TResponse"/> the behavior is a transparent pass-through.
/// </para>
/// <para>
/// Constructing a failed <see cref="ICommandResult{T}"/> for an arbitrary <typeparamref name="TResponse"/>
/// requires reflection — that path is annotated with <see cref="RequiresUnreferencedCodeAttribute"/> and
/// <see cref="RequiresDynamicCodeAttribute"/>. AOT / trimmed apps should validate inside handlers or
/// implement a closed-generic validation behavior per command instead.
/// </para>
/// </remarks>
public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Command
{
    private const string FailureFactoryAotMessage =
        "Constructing a failed ICommandResult<T> for an arbitrary TResponse requires runtime reflection. " +
        "Disable trimming/AOT for this code path or write a closed-generic validation behavior.";

    private static readonly ConcurrentDictionary<Type, Func<string, object>> FailureFactoryCache = new();

    private readonly IEnumerable<ICommandValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<ICommandValidator<TRequest>> validators)
        => _validators = validators;

    [RequiresUnreferencedCode(FailureFactoryAotMessage)]
    [RequiresDynamicCode(FailureFactoryAotMessage)]
    [UnconditionalSuppressMessage("Trimming", "IL2046",
        Justification = "This implementation requires reflection by design; consumers are warned via the attributes.")]
    [UnconditionalSuppressMessage("AOT", "IL3051",
        Justification = "This implementation requires dynamic code by design; consumers are warned via the attributes.")]
    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (result is { IsSuccess: false })
            {
                return BuildFailure(result.Message ?? "Validation failed.");
            }
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }

    [RequiresUnreferencedCode(FailureFactoryAotMessage)]
    [RequiresDynamicCode(FailureFactoryAotMessage)]
    private static TResponse BuildFailure(string message)
    {
        if (typeof(TResponse) == typeof(ICommandResult))
        {
            return (TResponse)CommandResult.IsFailed(message);
        }

        if (typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(ICommandResult<>))
        {
            var factory = FailureFactoryCache.GetOrAdd(typeof(TResponse), CreateFailureFactory);
            return (TResponse)factory(message);
        }

        throw new InvalidOperationException(
            $"ValidationPipelineBehavior cannot construct a failed result for response type '{typeof(TResponse).FullName}'. " +
            "Use ICommandResult or ICommandResult<T>.");
    }

    [RequiresUnreferencedCode(FailureFactoryAotMessage)]
    [RequiresDynamicCode(FailureFactoryAotMessage)]
    private static Func<string, object> CreateFailureFactory(Type responseType)
    {
        var inner = responseType.GetGenericArguments()[0];
        var method = typeof(CommandResult<>)
            .MakeGenericType(inner)
            .GetMethod(nameof(CommandResult<object>.IsFailed), [typeof(string)])
            ?? throw new MissingMethodException(typeof(CommandResult<>).FullName, nameof(CommandResult<object>.IsFailed));

        return message => method.Invoke(null, [message])!;
    }
}
