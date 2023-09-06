namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Class that describes a command to get and display the first page of active connections.
/// The update message must be equals to the string "/connections".
/// Does not support bots. Supports only private chats.
/// </summary>
public class FirstPageConnectionsCommand : ConnectionsCommandBase
{
    /// <summary>Message text.</summary>
    private const string FIRST_PAGE_CONNECTIONS_PATTERN = "/connections";

    /// <summary>Command name.</summary>
    protected override sealed string CommandName => "first page connections";

    /// <summary>Command object initialization.</summary>
    /// <param name="identityService">User identification service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public FirstPageConnectionsCommand(IIdentityService identityService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration) : base(identityService, telegramBot, logger, configuration) { }

    /// <summary>Method override for checking the validity of an update.</summary>
    /// <param name="update">Telegram bot ID.</param>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="index">Page index.</param>
    /// <returns>Is the update valid.</returns>
    protected override sealed bool IsValid(Update update, out long chatId, out int index)
    {
        chatId = 0;
        index = 0;

        if (update is null || update.Message is not { } message)
            return false;

        chatId = message.Chat.Id;

        return message.From is { } user && !user.IsBot && user.Id == chatId && message.Text == FIRST_PAGE_CONNECTIONS_PATTERN;
    }

    /// <summary>Method override for checking the validity of an update.</summary>
    /// <param name="configuration">Connections command configuration</param>
    protected override void OnConfiguring(ConnectionsCommandConfiguration configuration)
    {
        configuration.PageSize = AppConfiguration.GetSection("Telegram").GetValue<int>("ConnectionsPageSize");
    }
}

