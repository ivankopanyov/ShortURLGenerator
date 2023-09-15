namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Base;

/// <summary>Abstract class that describes a command that displays a list of active connections.</summary>
public abstract class ConnectionsCommandBase : IUpdateCommand
{
    /// <summary>The number of connections on the page by default.</summary>
    private const int DEFAULT_PAGE_SIZE = 1;

    /// <summary>User identification service.</summary>
    private readonly IIdentityService _identityService;

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    private readonly ITelegramBot _telegramBot;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>The number of active connections on the page.</summary>
    private readonly int _pageSize;

    /// <summary>Abstract property that holds the command name.</summary>
    protected abstract string CommandName { get; }

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

        var connectionsCommandConfiguration = new ConnectionsCommandConfiguration();
        OnConfiguring(connectionsCommandConfiguration, configuration);

        _pageSize = Math.Max(DEFAULT_PAGE_SIZE, connectionsCommandConfiguration.PageSize);
    }

    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (!IsValid(update, out long chatId, out int index))
            return false;

        _logger.LogInformation($"Execute {CommandName} command: Start. Update: {update.LogInfo()}");

        try
        {
            var connectionsPage = await _identityService.GetConnectionsAsync(chatId, index, _pageSize);
            await _telegramBot.SendConnectionsAsync(chatId, connectionsPage);
            _logger.LogInformation($"Execute {CommandName} command: Successfully. Update: {update.LogInfo()}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute {CommandName} command: {ex.Message}. Update: {update.LogInfo()}");
        }

        return true;
    }

    /// <summary>Abstract method for checking the validity of an update.</summary>
    /// <param name="update">Telegram bot ID.</param>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="index">Page index.</param>
    /// <returns>Is the update valid.</returns>
    protected abstract bool IsValid(Update update, out long chatId, out int index);

    /// <summary>Virtual method for configuring a connection command.</summary>
    /// <param name="commandConfiguration">Connections command configuration.</param>
    /// <param name="appConfiguration">Application configuration.</param>
    protected virtual void OnConfiguring(ConnectionsCommandConfiguration commandConfiguration, IConfiguration? appConfiguration)
    {
        if (appConfiguration is null)
            return;

        commandConfiguration.PageSize = appConfiguration
            .GetSection("Telegram")
            .GetValue<int>("ConnectionsPageSize");
    }
}

