namespace ShortURLGenerator.TelegramBot.IntegrationEventHandling;

/// <summary>URI generation event handler.</summary>
public class UriGeneratedIntegrationEventHandler : IntegrationEventHandlerBase<UriGeneratedIntegrationEvent>
{
    /// <summary>The sender of integration events.</summary>
    private readonly IEventBus _eventBus;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Handler initialization.</summary>
    /// <param name="eventBus">The sender of integration events.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public UriGeneratedIntegrationEventHandler(IEventBus eventBus,
        ITelegramBot telegramBot,
        ILogger<UriGeneratedIntegrationEventHandler> logger)
    {
        _eventBus = eventBus;
        _telegramBot = telegramBot;
        _logger = logger;
    }

    /// <summary>Overriding the event handling method.</summary>
    /// <param name="event">URI generated event.</param>
    protected override async Task HandleAsync(UriGeneratedIntegrationEvent? @event)
    {
        _logger.LogInformation("Handle URL generated event: start.");

        if (@event is null)
        {
            _logger.LogError("Handle URL generated event: failed.\n\tError: Event is null.");
            return;
        }

        _logger.LogInformation($"Handle URL generated event.\n\t{@event}");

        var messageId = await _telegramBot.SendUriAsync(@event.ChatId, @event.Uri, @event.SourceUri);

        var uriSentEvent = new UriSentIntegrationEvent(@event.ChatId, messageId, @event.Uri);

        _logger.LogInformation($"Handle URL generated event: connection to broker.\n\t{@event}\nURI sent event:\n\t{uriSentEvent}");

        try
        {
            _eventBus.Publish(uriSentEvent);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Handle URL generated event: failed.\n\t{@event}\nURI sent event:\n\t{uriSentEvent}\n\tError: Failed to connect to the broker.");
        }

        _logger.LogInformation($"Handle URL generated event: The event was sent successfully.\n\t{@event}\nURI sent event:\n\t{uriSentEvent}");
        _logger.LogInformation($"Handle URL generated event: succesful.\n\t{@event}");
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfigureConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

