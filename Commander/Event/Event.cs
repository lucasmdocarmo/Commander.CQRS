namespace Commander;

/// <summary>
/// Base type for domain events / notifications. A single event can be handled by 0..N handlers.
/// </summary>
public abstract class Event : Message
{
    /// <summary>Aggregate identifier the event relates to (optional).</summary>
    public Guid AggregateId { get; private set; }

    /// <summary>Set the aggregate identifier this event refers to.</summary>
    public void SetAggregateId(Guid id) => AggregateId = id;
}
