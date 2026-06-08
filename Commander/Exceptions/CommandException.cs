using System.Net;

namespace Commander;

[Serializable]
public class CommandException : Exception
{
    public string OriginalMessage { get; } = string.Empty;
    public HttpStatusCode ResponseCode { get; init; }

    public CommandException() { }

    public CommandException(string originalMessage) : base(originalMessage)
        => OriginalMessage = originalMessage;

    public CommandException(string originalMessage, Exception? originalException)
        : base(originalMessage, originalException)
        => OriginalMessage = originalMessage;

    public CommandException(string originalMessage, Exception? originalException, HttpStatusCode responseCode)
        : base(originalMessage, originalException)
    {
        OriginalMessage = originalMessage;
        ResponseCode = responseCode;
    }
}
