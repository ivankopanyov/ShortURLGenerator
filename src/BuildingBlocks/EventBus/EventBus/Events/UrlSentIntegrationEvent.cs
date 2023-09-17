namespace ShortURLGenerator.EventBus.Events;

/// <summary>Class that describes the event of sending the generated URI to the user.</summary>
public sealed class UriSentIntegrationEvent : IntegrationEventBase
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
    public UriSentIntegrationEvent(long chatId, int messageId, string uri)
    {
        ChatId = chatId;
        MessageId = messageId;
        Uri = uri;
    }

    /// <summary>Overriding the method of casting an object to a string type.</summary>
    /// <returns>Object cast to a string type.</returns>
    public override string ToString() => "QR code created integration event: { " +
        base.ToString() + ", Chat ID: " + ChatId + ", Message ID: " + MessageId +
        ", URI" + Uri + " }";
}
