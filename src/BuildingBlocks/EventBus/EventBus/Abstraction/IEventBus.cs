namespace ShortURLGenerator.EventBus.Abstraction;

/// <summary>The sender of integration events to the message broker.</summary>
public interface IEventBus
{
    /// <summary>Method for publishing an RabbitMQ.</summary>
    /// <typeparam name="T">Integration event type.</typeparam>
    /// <param name="event">Integration event.</param>
    void Publish<T>(T @event) where T : IntegrationEventBase;
}

