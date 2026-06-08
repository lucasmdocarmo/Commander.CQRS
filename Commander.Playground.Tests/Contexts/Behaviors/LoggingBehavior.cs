namespace Commander.Playground.Tests.Contexts.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : Message
{
    public static int InvocationCount;
    public static readonly List<string> Log = new();

    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref InvocationCount);
        Log.Add($"BEFORE {typeof(TRequest).Name}");
        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            Log.Add($"AFTER {typeof(TRequest).Name}");
        }
    }
}
