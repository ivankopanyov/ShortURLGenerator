namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

public class StartCommand : IUpdateCommand
{
    private const string START_PATTERN = "/start";

    private readonly ITelegramBot _telegramBot;

    private readonly ILogger _logger;

    public StartCommand(ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _telegramBot = telegramBot;
        _logger = logger;
    }

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

