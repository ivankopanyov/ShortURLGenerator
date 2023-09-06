namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Сlass that describes a command to close an active connection and send a close notification to chat.
/// The callback query message must match the "^close_[0-9A-Za-z]{20}$" pattern.
/// Does not support bots. Supports only private chats.
/// </summary>
public class CloseConnectionCommand : IUpdateCommand
{
    /// <summary>Callback query pattern.</summary>
    private const string CLOSE_CONNECTION_PATTERN = @"^close_[0-9A-Za-z]{20}$";

    /// <summary>User identification service.</summary>
    protected readonly IIdentityService _identityService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    protected readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    protected readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="identityService">User identification service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public CloseConnectionCommand(IIdentityService identityService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _identityService = identityService;
        _telegramBot = telegramBot;
        _logger = logger;
    }

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (update is null || update.CallbackQuery is not { } callbackQuery ||
            callbackQuery.Message is not { } message || callbackQuery.From.IsBot ||
            callbackQuery.Data is not { } text || !Regex.IsMatch(text, CLOSE_CONNECTION_PATTERN))
            return false;

        long chatId = message.Chat.Id;

        if (chatId != callbackQuery.From.Id)
            return false;

        int messageId = message.MessageId;
        var connectionId = text.Split('_', StringSplitOptions.RemoveEmptyEntries)[1];

        _logger.LogStart("Execute close connection command", update);

        try
        {
            await _identityService.CloseConnectionAsync(chatId, connectionId);
            await _telegramBot.SendCloseConnectionAsync(chatId, messageId);
            _logger.LogSuccessfully("Execute close connection command", update);
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, "Execute close connection command", ex.Message, update);
        }

        return true;
    }
}



