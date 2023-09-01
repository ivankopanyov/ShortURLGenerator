namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

public class ChangePageConnectionsCommand : ConnectionsCommandBase
{
    private const string CHANGE_PAGE_CONNECTIONS_PATTERN = @"^connections_[0-9]{1}$";

    protected override string CommandName => "change page connections";

    public ChangePageConnectionsCommand(IIdentityService identityService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration) : base(identityService, telegramBot, logger, configuration) { }

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

