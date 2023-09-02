namespace ShortURLGenerator.EventBus.Events;

/// <summary>Class that describes the QR code creation event.</summary>
public sealed class QRCodeCreatedIntegrationEvent : IntegrationEventBase
{
    /// <summary>Chat ID.</summary>
    public long ChatId { get; set; }

    /// <summary>Source message ID.</summary>
    public int MessageId { get; set; }

    /// <summary>QR code data.</summary>
    public byte[] Data { get; set; }

    /// <summary>Integration event initialization.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="messageId">Source message ID.</param>
    /// <param name="data">QR code data.</param>
    public QRCodeCreatedIntegrationEvent(long chatId, int messageId, byte[] data)
    {
        ChatId = chatId;
        MessageId = messageId;
        Data = data;
    }

    /// <summary>Overriding the method of casting an object to a string type.</summary>
    /// <returns>Object cast to a string type.</returns>
    public override string ToString() => $"{base.ToString()}\n\tChat ID: {ChatId}\n\tMessage ID: {MessageId}";
}
