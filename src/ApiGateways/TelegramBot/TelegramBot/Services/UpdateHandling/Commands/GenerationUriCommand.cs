namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Сlass that describes the command to generate a short URL and send it with the code to the chat.
/// The message must contain a valid http or https address.
/// Does not support bots. Supports any chats.
/// </summary>
public class GenerationUrlCommand : IUpdateCommand
{
    /// <summary>Service for generating short URLs.</summary>
    private readonly IUrlService _urlService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="urlService">Service for generating short URLs.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public GenerationUrlCommand(IUrlService urlService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _urlService = urlService;
        _telegramBot = telegramBot;
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

        long chatId = message.Chat.Id;

        _logger.LogInformation($"Execute generation uri command: start.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tSource URI: {text}");

        try
        {
            var url = await _urlService.GenerateUrlAsync(text);
            var messageId = await _telegramBot.SendUriAsync(chatId, url, message.MessageId);
            _logger.LogInformation($"Execute generation uri command: succesful.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tSource URI: {text}\n\tMessage ID: {messageId}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute generation uri command: failed.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tSource URI: {text}\n\tError: {ex.Message}");
        }

        return true;
    }
}

