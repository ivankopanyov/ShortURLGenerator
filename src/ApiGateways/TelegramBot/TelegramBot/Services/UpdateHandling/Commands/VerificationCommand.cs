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

    /// <summary>User identification service.</summary>
    private readonly IIdentityService _identityService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="identityService">User identification service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public VerificationCommand(IIdentityService identityService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _identityService = identityService;
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

        _logger.LogInformation($"Execute get verification code command: start.\n\tUpdate Id: {update.Id}\n\tUser ID: {chatId}");

        try
        {
            var response = await _identityService.GetVerificationCodeAsync(chatId);
            await _telegramBot.SendVerificationCodeAsync(chatId, response.Code, response.LifeTimeMinutes);
            _logger.LogInformation($"Execute get verification code: succesful.\n\tUpdate Id: {update.Id}\n\tUser ID: {chatId}\n\tVerification code: {response.Code}\n\tLifetime Minutes: {response.LifeTimeMinutes}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogInformation($"Execute get verification code command: failed.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tError: {ex.Message}");
        }

        return true;
    }
}

