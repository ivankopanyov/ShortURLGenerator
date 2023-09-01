namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

public class UpdateHandler : UpdateHandlerBase
{
    public UpdateHandler(IIdentityService identityService,
        IUrlService urlService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration) : base(identityService, urlService, telegramBot, logger, configuration) { }

    protected override void CommandSetConfiguration(ICommandSetBuilder commandSetBuilder) => commandSetBuilder
        .AddCommand(new StartCommand(TelegramBot, Logger))
        .AddCommand(new GenerationUrlCommand(UrlService, TelegramBot, Logger))
        .AddCommand(new VerificationCommand(IdentityService, TelegramBot, Logger))
        .AddCommand(new FirstPageConnectionsCommand(IdentityService, TelegramBot, Logger, Configuration))
        .AddCommand(new ChangePageConnectionsCommand(IdentityService, TelegramBot, Logger, Configuration))
        .AddCommand(new CloseConnectionCommand(IdentityService, TelegramBot, Logger));

    protected override async Task NotFoundCommandHandleAsync(Update update)
    {
        if (update is not null && update.Message is { } message && message.From is { } user && !user.IsBot)
        {
            long chatId = message.Chat.Id;
            Logger.LogInformation($"Handle update: failed.\n\tUpdate ID: {update.Id}\n\tChat ID: {chatId}\n\tMessage: {message.Text}\n\tError: Command not found.");
            await TelegramBot.SendErrorMessageAsync(chatId, "Ссылка некорректна.");
        }
        else
            Logger.LogError($"Handle update: failed.\n\tError: Command not found.");
    }
}

