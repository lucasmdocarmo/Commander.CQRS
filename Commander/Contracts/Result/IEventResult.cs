using System.Net;

namespace Commander;

public interface IEventResult
{
    HttpStatusCode StatusCode { get; }
    string? Message { get; }
    bool IsSuccess { get; }
}
