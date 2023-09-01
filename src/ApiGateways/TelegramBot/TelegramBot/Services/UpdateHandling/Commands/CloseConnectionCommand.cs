namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

public class CloseConnectionCommand : IUpdateCommand
{
    private const string CLOSE_CONNECTION_PATTERN = @"^close_[0-9A-Za-z]{20}$";

    private readonly IIdentityService _identityService;

    private readonly ITelegramBot _telegramBot;

    private readonly ILogger _logger;

    public CloseConnectionCommand(IIdentityService identityService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _identityService = identityService;
        _telegramBot = telegramBot;
        _logger = logger;
    }

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

        _logger.LogInformation($"Execute close connection command: start.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tMessage ID: {messageId}\n\tConnection ID: {connectionId}");

        try
        {
            await _identityService.CloseConnectionAsync(chatId, connectionId);
            await _telegramBot.SendCloseConnectionAsync(chatId, messageId);
            _logger.LogInformation($"Execute close connection command: succesful.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tMessage ID: {messageId}\n\tConnection ID: {connectionId}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogInformation($"Execute close connection command: failed.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tMessage ID: {messageId}\n\tConnection ID: {connectionId}\n\tError: {ex.Message}");
        }

        return true;
    }
}



