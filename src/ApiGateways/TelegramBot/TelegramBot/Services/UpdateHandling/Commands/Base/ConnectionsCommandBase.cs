namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Base;

public abstract class ConnectionsCommandBase : IUpdateCommand
{
    protected readonly IIdentityService _identityService;

    protected readonly ITelegramBot _telegramBot;

    protected readonly ILogger _logger;

    protected readonly int _pageSize;

    protected abstract string CommandName { get; }

    public ConnectionsCommandBase(IIdentityService identityService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration)
    {
        _identityService = identityService;
        _telegramBot = telegramBot;
        _logger = logger;
        _pageSize = configuration.GetSection("Telegram").GetValue<int>("ConnectionsPageSize");
    }

    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (!IsValid(update, out long chatId, out int index))
            return false;

        _logger.LogInformation($"Execute {CommandName} command: start.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tIndex: {index}");

        try
        {
            var connectionsPage = await _identityService.GetConnectionsAsync(chatId, index, _pageSize);
            await _telegramBot.SendConnectionsAsync(chatId, connectionsPage);
            _logger.LogInformation($"Execute {CommandName} command: succesful.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tIndex: {index}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute {CommandName} command: failed.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tIndex: {index}\n\tError: {ex.Message}");
        }

        return true;
    }

    protected abstract bool IsValid(Update update, out long chatId, out int index);
}

