namespace ShortURLGenerator.TelegramBot.IntegrationEventHandling;

/// <summary>QR code creation event handler.</summary>
public class QRCodeCreatedIntegrationEventHandler : IntegrationEventHandlerBase<QRCodeCreatedIntegrationEvent>
{
    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Handler initialization.</summary>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public QRCodeCreatedIntegrationEventHandler(ITelegramBot telegramBot,
        ILogger<QRCodeCreatedIntegrationEventHandler> logger)
    {
        _telegramBot = telegramBot;
        _logger = logger;
    }

    /// <summary>Overriding the event handling method.</summary>
    /// <param name="event">QR code creation event.</param>
    protected override async Task HandleAsync(QRCodeCreatedIntegrationEvent? @event)
    {
        _logger.LogInformation("Handle QR code created event: start.");

        if (@event is null)
        {
            _logger.LogError("Handle QR code created event: failed.\n\tError: Event is null.");
            return;
        }
        else if (@event.Data is null)
        {
            _logger.LogError($"Handle QR code created event: failed.\n\t{@event}\n\tError: Event data is null.");
            return;
        }

        _logger.LogInformation($"Handle QR code created event.\n\t{@event}");

        await _telegramBot.SendQRCodeAsync(@event.ChatId, @event.MessageId, @event.Data);
        _logger.LogInformation($"Handle QR code created event: succesful.\n\t{@event}");
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}
