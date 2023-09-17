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

    /// <summary>User connection service.</summary>
    protected readonly IConnectionService _connectionService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    protected readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    protected readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="connectionService">User connection service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public CloseConnectionCommand(IConnectionService connectionService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _connectionService = connectionService;
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

        _logger.LogInformation($"Execute close connection command: Start. {update.LogInfo()}");

        int messageId = message.MessageId;
        var connectionId = text.Split('_', StringSplitOptions.RemoveEmptyEntries)[1];

        try
        {
            await _connectionService.CloseConnectionAsync(chatId, connectionId);
            await _telegramBot.SendCloseConnectionAsync(chatId, messageId);
            _logger.LogInformation($"Execute close connection command: Successfully. {update.LogInfo()}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute close connection command: {ex.Message}. {update.LogInfo()}");
        }

        return true;
    }
}



