using System.Net;

namespace Commander;

public interface IQueryResult
{
    HttpStatusCode StatusCode { get; }
    string? Message { get; }
    bool IsSuccess { get; }
}

public interface IQueryResult<out T> : IQueryResult
{
    T? Data { get; }
}
