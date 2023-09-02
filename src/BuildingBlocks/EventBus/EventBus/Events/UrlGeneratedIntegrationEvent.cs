namespace ShortURLGenerator.EventBus.Events;

/// <summary>Class that describes the short URL generation event.</summary>
public sealed class UrlGeneratedIntegrationEvent : IntegrationEventBase
{
    /// <summary>Chat ID.</summary>
    public long ChatId { get; set; }

    /// <summary>Generated URL.</summary>
    public string Url { get; set; }

    /// <summary>The original URI submitted by the user.</summary>
    public string SourceUri { get; set; }

    /// <summary>Integration event initialization.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="url">Generated URL.</param>
    /// <param name="sourceUri">he original URI submitted by the user.</param>
    public UrlGeneratedIntegrationEvent(long chatId, string url, string sourceUri)
    {
        ChatId = chatId;
        Url = url;
        SourceUri = sourceUri;
    }
}

