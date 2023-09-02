namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Сlass that describes the command to send a welcome message to the chat.
/// The update message must be equals to the string "/start".
/// Does not support bots. Supports only private chats.
/// </summary>
public class StartCommand : IUpdateCommand
{
    /// <summary>Message text.</summary>
    private const string START_PATTERN = "/start";

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Command object initialization.</summary>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    public StartCommand(ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _telegramBot = telegramBot;
        _logger = logger;
    }

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (update is null || update.Message is not { } message ||
            message.Text != START_PATTERN || message.From is not { } user || user.IsBot)
            return false;

        long chatId = message.Chat.Id;

        if (chatId != user.Id)
            return false;

        string firstName = user.FirstName;

        _logger.LogInformation($"Execute start command: start.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tFirst name: {firstName}");

        try
        {
            await _telegramBot.SendHelloAsync(chatId, user.FirstName);
            _logger.LogInformation($"Execute start command: succesful.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tFirst name: {firstName}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute start command: failed.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tFirst name: {firstName}\n\tError: {ex.Message}");
        }

        return true;
    }
}

