using System.Net;

namespace Commander;

/// <summary>Default <see cref="IQueryResult{T}"/> implementation.</summary>
public sealed class QueryResult<T> : IQueryResult<T>
{
    private QueryResult(T? data, string? message, bool success, HttpStatusCode statusCode)
    {
        Data = data;
        Message = message;
        IsSuccess = success;
        StatusCode = statusCode;
    }

    public string? Message { get; }
    public bool IsSuccess { get; }
    public T? Data { get; }
    public HttpStatusCode StatusCode { get; }

    public static IQueryResult<T> Success(T data) => new QueryResult<T>(data, null, true, HttpStatusCode.OK);
    public static IQueryResult<T> Success(T data, string message) => new QueryResult<T>(data, message, true, HttpStatusCode.OK);
    public static IQueryResult<T> Success(T data, string message, HttpStatusCode statusCode) => new QueryResult<T>(data, message, true, statusCode);
    public static ValueTask<IQueryResult<T>> SuccessAsync(T data) => ValueTask.FromResult(Success(data));
    public static ValueTask<IQueryResult<T>> SuccessAsync(T data, string message) => ValueTask.FromResult(Success(data, message));

    public static IQueryResult<T> Failed(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        new QueryResult<T>(default, message, false, statusCode);
    public static ValueTask<IQueryResult<T>> FailedAsync(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) =>
        ValueTask.FromResult(Failed(message, statusCode));
}
