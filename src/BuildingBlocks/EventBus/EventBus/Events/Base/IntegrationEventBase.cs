namespace ShortURLGenerator.EventBus.Events.Base;

/// <summary>The base class that describes the integration event.</summary>
public abstract class IntegrationEventBase
{
    /// <summary>Integration event ID.</summary>
    public Guid Id { get; private init; }

    /// <summary>Date and time when the integration event was created.</summary>
    public DateTime CreationDate { get; private init; }

    /// <summary>Integration event initialization.</summary>
    internal IntegrationEventBase()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}

