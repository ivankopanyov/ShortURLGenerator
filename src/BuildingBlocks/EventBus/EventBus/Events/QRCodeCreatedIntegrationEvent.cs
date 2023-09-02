namespace EventBus.Events;

public sealed class QRCodeCreatedIntegrationEvent : IntegrationEventBase
{
    public long ChatId { get; set; }

    public int MessageId { get; set; }

    public byte[] Data { get; set; }

    public QRCodeCreatedIntegrationEvent(long chatId, int messageId, byte[] data)
    {
        ChatId = chatId;
        MessageId = messageId;
        Data = data;
    }
}
