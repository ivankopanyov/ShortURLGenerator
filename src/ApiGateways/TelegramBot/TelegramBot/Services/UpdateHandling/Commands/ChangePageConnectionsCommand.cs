namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

/// <summary>
/// Сlass that describes the command to change the page number of active connections.
/// The callback query message must match the "^connections_[0-9]{1}$" pattern.
/// Does not support bots. Supports only private chats.
/// </summary>
public class ChangePageConnectionsCommand : ConnectionsCommandBase
{
    /// <summary>Callback query pattern.</summary>
    private const string CHANGE_PAGE_CONNECTIONS_PATTERN = @"^connections_[0-9]{1}$";

    /// <summary>Command name.</summary>
    protected override sealed string CommandName => "change page connections";

    /// <summary>Command object initialization.</summary>
    /// <param name="identityService">User identification service.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public ChangePageConnectionsCommand(IIdentityService identityService,
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

        if (update is null || update.CallbackQuery is not { } callbackQuery || callbackQuery.Message is not { } message)
            return false;

        var user = callbackQuery.From;
        chatId = message.Chat.Id;

        if (user.IsBot || user.Id != chatId || callbackQuery.Data is not { } text || !Regex.IsMatch(text, CHANGE_PAGE_CONNECTIONS_PATTERN))
            return false;

        index = int.Parse(text.Split('_')[1]);
        return true;
    }
}

