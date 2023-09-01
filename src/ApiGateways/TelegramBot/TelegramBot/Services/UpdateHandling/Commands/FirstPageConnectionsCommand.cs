namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

public class FirstPageConnectionsCommand : ConnectionsCommandBase
{
    private const string FIRST_PAGE_CONNECTIONS_PATTERN = "/connections";

    protected override string CommandName => "first page connections";

    public FirstPageConnectionsCommand(IIdentityService identityService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration) : base(identityService, telegramBot, logger, configuration) { }

    protected override sealed bool IsValid(Update update, out long chatId, out int index)
    {
        chatId = 0;
        index = 0;

        if (update is null || update.Message is not { } message)
            return false;

        chatId = message.Chat.Id;

        return message.From is { } user && !user.IsBot && user.Id == chatId && message.Text == FIRST_PAGE_CONNECTIONS_PATTERN;
    }
}

