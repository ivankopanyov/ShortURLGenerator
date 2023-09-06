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

    /// <summary>Application configuration.</summary>
    protected IConfiguration AppConfiguration { get; private init; }

    /// <summary>Command object initialization.</summary>
    /// <param name="identityService">User identification service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public ConnectionsCommandBase(IIdentityService identityService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration)
    {
        _identityService = identityService;
        _telegramBot = telegramBot;
        _logger = logger;
        AppConfiguration = configuration;

        var connectionsCommandConfiguration = new ConnectionsCommandConfiguration();
        OnConfiguring(connectionsCommandConfiguration);

        _pageSize = Math.Max(1, connectionsCommandConfiguration.PageSize);
    }

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (!IsValid(update, out long chatId, out int index))
            return false;

        _logger.LogStart($"Execute {CommandName} command", update);

        try
        {
            var connectionsPage = await _identityService.GetConnectionsAsync(chatId, index, _pageSize);
            await _telegramBot.SendConnectionsAsync(chatId, connectionsPage);
            _logger.LogSuccessfully($"Execute {CommandName} command", update, connectionsPage);
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute {CommandName} command", ex.Message, update);
        }

        return true;
    }

    /// <summary>Abstract method for checking the validity of an update.</summary>
    /// <param name="update">Telegram bot ID.</param>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="index">Page index.</param>
    /// <returns>Is the update valid.</returns>
    protected abstract bool IsValid(Update update, out long chatId, out int index);

    /// <summary>Abstract method for configuring a connection command.</summary>
    /// <param name="configuration">Connections command configuration.</param>
    protected abstract void OnConfiguring(ConnectionsCommandConfiguration configuration);
}

