namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Class that describes the command to request a verification code and send a message with the code to the chat.
/// The update message must be equals to the string "/verification".
/// Does not support bots. Supports only private chats.
/// </summary>
public class VerificationCommand : IUpdateCommand
{
    /// <summary>Message text.</summary>
    private const string VERIFICATION_PATTERN = "/verification";

    /// <summary>User connection service.</summary>
    private readonly IConnectionService _connectionService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="connectionService">User connection service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public VerificationCommand(IConnectionService connectionService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _connectionService = connectionService;
        _telegramBot = telegramBot;
        _logger = logger;
    }

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (update is null || update.Message is not { } message ||
            message.Text != VERIFICATION_PATTERN || message.From is not { } user || user.IsBot)
            return false;

        long chatId = message.Chat.Id;

        if (chatId != user.Id)
            return false;

        _logger.LogInformation($"Execute get verification code command: Start. {update.LogInfo()}");

        try
        {
            var response = await _connectionService.GetVerificationCodeAsync(chatId);
            await _telegramBot.SendVerificationCodeAsync(chatId, response.Id, response.LifeTimeMinutes);
            _logger.LogInformation($"Execute get verification code command: Successfully. {update.LogInfo()}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError($"Execute get verification code command: {ex.Message}. {update.LogInfo()}");
        }

        return true;
    }
}

