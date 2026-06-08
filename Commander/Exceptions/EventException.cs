using System.Net;

namespace Commander;

[Serializable]
public class EventException : Exception
{
    public string OriginalMessage { get; } = string.Empty;
    public HttpStatusCode ResponseCode { get; init; }

    public EventException() { }

    public EventException(string originalMessage) : base(originalMessage)
        => OriginalMessage = originalMessage;

    public EventException(string originalMessage, Exception? originalException)
        : base(originalMessage, originalException)
        => OriginalMessage = originalMessage;

    public EventException(string originalMessage, Exception? originalException, HttpStatusCode responseCode)
        : base(originalMessage, originalException)
    {
        OriginalMessage = originalMessage;
        ResponseCode = responseCode;
    }
}
