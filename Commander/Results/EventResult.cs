using System.Net;

namespace Commander;

/// <summary>Default <see cref="IEventResult"/> implementation.</summary>
public sealed class EventResult : IEventResult
{
    private static readonly EventResult SuccessSingleton = new(null, true, HttpStatusCode.OK);

    public EventResult() : this(null, true, HttpStatusCode.OK) { }

    public EventResult(string? message, bool success)
        : this(message, success, success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError) { }

    public EventResult(string? message, bool success, HttpStatusCode statusCode)
    {
        Message = message;
        IsSuccess = success;
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
    public string? Message { get; }
    public bool IsSuccess { get; }

    public static IEventResult Success() => SuccessSingleton;
    public static IEventResult Success(string message) => new EventResult(message, true);
    public static ValueTask<IEventResult> SuccessAsync() => ValueTask.FromResult<IEventResult>(SuccessSingleton);
    public static ValueTask<IEventResult> SuccessAsync(string message) => ValueTask.FromResult(Success(message));
    public static IEventResult IsFailed(string message) => new EventResult(message, false);
    public static ValueTask<IEventResult> FailAsync(string message) => ValueTask.FromResult(IsFailed(message));
}
