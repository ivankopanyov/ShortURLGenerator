namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Base;

/// <summary>Abstract class that describes a command that displays a list of active connections.</summary>
public abstract class ConnectionsCommandBase : IUpdateCommand
{
    /// <summary>User identification service.</summary>
    private readonly IIdentityService _identityService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>The number of active connections on the page.</summary>
    protected readonly int _pageSize;

    /// <summary>Abstract property that holds the command name.</summary>
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

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
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

    /// <summary>Abstract method for checking the validity of an update.</summary>
    /// <param name="update">Telegram bot ID.</param>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="index">Page index.</param>
    /// <returns>Is the update valid.</returns>
    protected abstract bool IsValid(Update update, out long chatId, out int index);
}

