namespace ShortURLGenerator.EventBus.Abstraction;

/// <summary>The sender of integration events to the message broker.</summary>
public interface IEventBus
{
    /// <summary>Method for publishing an integration event.</summary>
    /// <param name="event">Integration event.</param>
    void Publish(IntegrationEventBase @event);
}

