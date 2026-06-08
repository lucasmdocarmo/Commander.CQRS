using System.Runtime.CompilerServices;

namespace Commander.Playground.Tests.Contexts.Behaviors;

public sealed class StreamLoggingBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : Query
    where TResponse : IQueryOutput
{
    public static int InvocationCount;
    public static int ItemsObserved;

    public async IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref InvocationCount);
        await foreach (var item in next(cancellationToken).WithCancellation(cancellationToken))
        {
            Interlocked.Increment(ref ItemsObserved);
            yield return item;
        }
    }
}
