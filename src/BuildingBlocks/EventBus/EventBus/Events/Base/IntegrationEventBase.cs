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
        CreationDate = DateTime.Now;
    }

    /// <summary>Overriding the method of casting an object to a string type.</summary>
    /// <returns>Object cast to a string type.</returns>
    public override string ToString() => "ID: " + Id + ", Created " + CreationDate.ToString("dd.MM.yyyy HH:mm:ss.ffff");
}

