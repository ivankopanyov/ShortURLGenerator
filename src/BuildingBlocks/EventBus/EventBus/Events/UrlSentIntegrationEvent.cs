namespace ShortURLGenerator.EventBus.Events;

/// <summary>Class that describes the event of sending the generated URI to the user.</summary>
public sealed class UrlSentIntegrationEvent : IntegrationEventBase
{
    /// <summary>Chat ID.</summary>
    public long ChatId { get; set; }

    /// <summary>Source message ID.</summary>
    public int MessageId { get; set; }

    /// <summary>Generated URI.</summary>
    public string Uri { get; set; }

    /// <summary>Integration event initialization.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="messageId">Source message ID.</param>
    /// <param name="uri">Generated URI.</param>
    public UrlSentIntegrationEvent(long chatId, int messageId, string uri)
    {
        ChatId = chatId;
        MessageId = messageId;
        Uri = uri;
    }
}
