namespace Commander;

/// <summary>Subscribes to an event. A single event may be handled by 0..N handlers.</summary>
public interface IEventHandler<in TRequest> where TRequest : Event
{
    ValueTask<IEventResult> Publish(TRequest request, CancellationToken cancellationToken = default);
}
