namespace ShortURLGenerator.QRCreator.API.IntegrationEventHandling;

/// <summary>Class that describes the URI sent event.</summary>
public class UriSentIntegrationEventHandler : IntegrationEventHandlerBase<UriSentIntegrationEvent>
{
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
        ILogger<UriSentIntegrationEventHandler> logger,
        IConfiguration configuration)
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

        _logger.LogStart("Handle URI sent event", @event);

        var qrcode = _qRCodeCreationService.GenerateJpeg(@event.Uri);
        var qrCodeCreatedEvent = new QRCodeCreatedIntegrationEvent(@event.ChatId, @event.MessageId, qrcode);

        _logger.LogInformation("Handle URI sent event", "connection to broker.", @event, qrCodeCreatedEvent);

        try
        {
            _eventBus.Publish(qrCodeCreatedEvent);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Handle URI sent event", "Failed to connect to the broker.", @event, qrCodeCreatedEvent);
        }

        _logger.LogSuccessfully("Handle URI sent event", @event, qrCodeCreatedEvent);
        return Task.CompletedTask;
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

