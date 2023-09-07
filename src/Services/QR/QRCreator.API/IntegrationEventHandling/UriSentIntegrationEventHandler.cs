namespace ShortURLGenerator.QRCreator.API.IntegrationEventHandling;

/// <summary>Class that describes the URI sent event.</summary>
public class UriSentIntegrationEventHandler : IntegrationEventHandlerBase<UriSentIntegrationEvent>
{
    /// <summary>QR code creation service</summary>
    private readonly IQRCodeCreationService _qRCodeCreationService;

    /// <summary>The sender of integration events.</summary>
    private readonly IEventBus _eventBus;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Handler initialization.</summary>
    /// <param name="qRCodeCreationService"></param>
    /// <param name="eventBus">The sender of integration events.</param>
    /// <param name="logger">Log service.</param>
    public UriSentIntegrationEventHandler(IQRCodeCreationService qRCodeCreationService,
        IEventBus eventBus,
        ILogger<UriSentIntegrationEventHandler> logger)
    {
        _qRCodeCreationService = qRCodeCreationService;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>Overriding the event handling method.</summary>
    /// <param name="event">URI sent event.</param>
    protected override Task HandleAsync(UriSentIntegrationEvent? @event)
    {
        if (@event is null)
        {
            _logger.LogError("Handle URI sent event", "Event is null.");
            return Task.CompletedTask;
        }

        var eventId = @event.Id.ToString();

        _logger.LogStart("Handle URI sent event", eventId);
        _logger.LogObject("Handle URI sent event", @event);

        var qrcode = _qRCodeCreationService.GenerateJpeg(@event.Uri);
        var qrCodeCreatedEvent = new QRCodeCreatedIntegrationEvent(@event.ChatId, @event.MessageId, qrcode);
        var qrCodeCreatedEventId = qrCodeCreatedEvent.Id.ToString();

        _logger.LogStart("Send QR code created event", qrCodeCreatedEventId);
        _logger.LogObject("Send QR code created event", qrCodeCreatedEvent);

        try
        {
            _eventBus.Publish(qrCodeCreatedEvent);
            _logger.LogSuccessfully("Send QR code created event", qrCodeCreatedEventId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Send QR code created event", ex.Message, qrCodeCreatedEventId);
        }

        _logger.LogSuccessfully("Handle URI sent event", eventId);
        return Task.CompletedTask;
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfiguringConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

