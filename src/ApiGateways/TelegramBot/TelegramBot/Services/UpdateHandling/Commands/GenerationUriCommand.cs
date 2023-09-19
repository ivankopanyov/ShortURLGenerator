namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Сlass that describes the command to generate a short URL and send it with the code to the chat.
/// The message must contain a valid http or https address.
/// Does not support bots. Supports any chats.
/// </summary>
public class GenerationUrlCommand : IUpdateCommand
{
    /// <summary>Service for sending integration events.</summary>
    private readonly IEventBus _eventBus;

    /// <summary>Service for generating short URLs.</summary>
    private readonly IUrlGenerator _urlGenerator;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Service for fixing URL.</summary>
    private readonly IFixUrlService _fixUrlService;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="urlGenerator">Service for generating short URLs.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="fixUrlService">Service for fixing URL.</param>
    /// <param name="logger">Log service.</param>
    public GenerationUrlCommand(IEventBus eventBus,
        IUrlGenerator urlGenerator,
        ITelegramBot telegramBot,
        IFixUrlService fixUrlService,
        ILogger<IUpdateCommand> logger)
    {
        _eventBus = eventBus;
        _urlGenerator = urlGenerator;
        _telegramBot = telegramBot;
        _fixUrlService = fixUrlService;
        _logger = logger;
    }

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (update is null || update.Message is not { } message || message.From is not { } user ||
            user.IsBot || message.Text is not { } text ||
            !Uri.TryCreate(text, UriKind.Absolute, out Uri? uri) || uri is null ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return false;

        _logger.LogInformation($"Execute generation uri command: Start. {update.LogInfo()}.");

        long chatId = message.Chat.Id;

        try
        {
            var url = await _urlGenerator.GenerateUrlAsync(text);
            var result = _fixUrlService.FixUrl(url);
            var messageId = await _telegramBot.SendUriAsync(chatId, result, message.MessageId);

            var @event = new UriSentIntegrationEvent(chatId, messageId, result);

            try
            {
                _eventBus.Publish(@event);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Execute generation uri command: Send URI sent event failed. {@event}");
            }

            _logger.LogInformation($"Execute generation uri command: Successfully. {update.LogInfo()}.");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute generation uri command: {ex.Message}. {update.LogInfo()}.");
        }

        return true;
    }
}

