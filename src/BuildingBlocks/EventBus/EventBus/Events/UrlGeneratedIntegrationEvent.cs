namespace ShortURLGenerator.EventBus.Events;

/// <summary>Class that describes the short URL generation event.</summary>
public sealed class UriGeneratedIntegrationEvent : IntegrationEventBase
{
    /// <summary>Chat ID.</summary>
    public long ChatId { get; set; }

    /// <summary>Generated URI.</summary>
    public string Uri { get; set; }

    /// <summary>The original URI submitted by the user.</summary>
    public string SourceUri { get; set; }

    /// <summary>Integration event initialization.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="url">Generated URL.</param>
    /// <param name="sourceUri">he original URI submitted by the user.</param>
    public UriGeneratedIntegrationEvent(long chatId, string url, string sourceUri)
    {
        ChatId = chatId;
        Uri = url;
        SourceUri = sourceUri;
    }

    /// <summary>Overriding the method of casting an object to a string type.</summary>
    /// <returns>Object cast to a string type.</returns>
    public override string ToString() => $"{base.ToString()}\n\tChat ID: {ChatId}\n\tURI: {Uri}\n\tSource URI: {SourceUri}";
}

