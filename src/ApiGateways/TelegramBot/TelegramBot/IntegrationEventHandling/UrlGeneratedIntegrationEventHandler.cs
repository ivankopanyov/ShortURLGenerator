namespace ShortURLGenerator.TelegramBot.IntegrationEventHandling;

/// <summary>URI generation event handler.</summary>
public class UriGeneratedIntegrationEventHandler : IntegrationEventHandlerBase<UriGeneratedIntegrationEvent>
{
    /// <summary>The sender of integration events.</summary>
    private readonly IEventBus _eventBus;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Service for fixing URL.</summary>
    private readonly IFixUrlService _fixUrlService;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Handler initialization.</summary>
    /// <param name="eventBus">The sender of integration events.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="fixUrlService">Service for fixing URL.</param>
    /// <param name="logger">Log service.</param>
    public UriGeneratedIntegrationEventHandler(IEventBus eventBus,
        ITelegramBot telegramBot,
        IFixUrlService fixUrlService,
        ILogger<UriGeneratedIntegrationEventHandler> logger)
    {
        _eventBus = eventBus;
        _telegramBot = telegramBot;
        _fixUrlService = fixUrlService;
        _logger = logger;
    }

    /// <summary>Overriding the event handling method.</summary>
    /// <param name="event">URI generated event.</param>
    protected override async Task HandleAsync(UriGeneratedIntegrationEvent? @event)
    {
        _logger.LogInformation($"Handle URL generated event: Start. Event: {@event}.");

        if (@event is null)
        {
            _logger.LogError($"Handle URL generated event: Event is null.");
            return;
        }
        var uri = _fixUrlService.FixUrl(@event.Uri);
        var messageId = await _telegramBot.SendUriAsync(@event.ChatId, uri, @event.SourceUri);
        var uriSentEvent = new UriSentIntegrationEvent(@event.ChatId, messageId, uri);

        try
        {
            _eventBus.Publish(uriSentEvent);
            _logger.LogInformation($"Handle URL generated event: Successfully. Event: {@event}, URI sent event: {uriSentEvent}");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Handle URL generated event: Send URI sent event failed. Event: {@event}, URI sent event: {uriSentEvent}");
        }
    }

    /// <summary>Overriding the broker connection configuration method.</summary>
    /// <param name="connectionFactory">Connection factory.</param>
    protected override void OnConfiguringConnection(ConnectionFactory connectionFactory)
    {
        connectionFactory.HostName = Environment.GetEnvironmentVariable("EVENT_BUS_HOST_NAME");
    }
}

