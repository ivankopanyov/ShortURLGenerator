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
        if (@event is null)
        {
            _logger.LogError("Handle URL generated event", "Event is null");
            return;
        }

        string eventId = @event.Id.ToString();

        _logger.LogStart("Handle URL generated event", eventId);
        _logger.LogObject("Handle URL generated event", @event);

        var messageId = await _telegramBot.SendUriAsync(@event.ChatId, @event.Uri, @event.SourceUri);
        var uriSentEvent = new UriSentIntegrationEvent(@event.ChatId, messageId, @event.Uri);
        var uriSentEventId = uriSentEvent.Id.ToString();


        _logger.LogStart("Send URL generated event", uriSentEventId);
        _logger.LogObject("Send URL generated event", uriSentEvent);

        try
        {
            _eventBus.Publish(uriSentEvent);
            _logger.LogSuccessfully("Send URL generated event", uriSentEventId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Send URL generated event", "Failed to connect to the broker", uriSentEventId);
        }

        _logger.LogSuccessfully("Handle URL generated event", eventId);
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfiguringConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

