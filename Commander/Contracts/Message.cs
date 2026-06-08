namespace Commander;

/// <summary>
/// Base type for every message that flows through the mediator.
/// </summary>
public abstract class Message
{
    /// <summary>UTC timestamp captured when the message was constructed.</summary>
    public DateTime Timestamp { get; protected init; } = DateTime.UtcNow;
}
