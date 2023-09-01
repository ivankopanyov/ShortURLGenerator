namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

public class VerificationCommand : IUpdateCommand
{
    private const string VERIFICATION_PATTERN = "/verification";

    private readonly IIdentityService _identityService;

    private readonly ITelegramBot _telegramBot;

    private readonly ILogger _logger;

    public VerificationCommand(IIdentityService identityService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _identityService = identityService;
        _telegramBot = telegramBot;
        _logger = logger;
    }

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

