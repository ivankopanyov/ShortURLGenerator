namespace ShortURLGenerator.QRCreator.API.IntegrationEventHandling;

/// <summary>Class that describes the URI sent event.</summary>
public class UriSentIntegrationEventHandler : IntegrationEventHandlerBase<UriSentIntegrationEvent>
{
    private readonly IQRCodeCreationService _qRCodeCreationService;

    /// <summary>The sender of integration events.</summary>
    private readonly IEventBus _eventBus;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    private int _qRCodeSize;

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
        _qRCodeSize = configuration.GetSection("QRCode").GetValue<int>("Size");
    }

    /// <summary>Overriding the event handling method.</summary>
    /// <param name="event">URI sent event.</param>
    protected override Task HandleAsync(UriSentIntegrationEvent? @event)
    {
        _logger.LogInformation("Handle URI sent event: start.");

        if (@event is null)
        {
            _logger.LogError("Handle URI sent event: failed.\n\tError: Event is null.");
            return Task.CompletedTask;
        }

        _logger.LogInformation($"Handle URI sent event.\n\t{@event}");

        var qrcode = _qRCodeCreationService.GenerateJpeg(@event.Uri, _qRCodeSize);

        var qrCodeCreatedEvent = new QRCodeCreatedIntegrationEvent(@event.ChatId, @event.MessageId, qrcode);

        _logger.LogInformation($"Handle URI sent event: connection to broker.\n\t{@event}\nQR code created event: \n\t{qrCodeCreatedEvent}");

        try
        {
            _eventBus.Publish(qrCodeCreatedEvent);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Handle URI sent event: failed.\n\t{@event}\nQR code created event: \n\t{qrCodeCreatedEvent}\n\tError: Failed to connect to the broker.");
        }

        _logger.LogInformation($"Handle URI sent event: The event was sent successfully.\n\t{@event}\nQR code created event: \n\t{qrCodeCreatedEvent}");
        _logger.LogInformation($"Handle URI sent event: succesful.\n\t{@event}");
        return Task.CompletedTask;
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

