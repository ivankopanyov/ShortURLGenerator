namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

/// <summary>Inherited class that describes the basic handler for telegram bot updates.</summary>
public class UpdateHandler : UpdateHandlerBase
{
    /// <summary>Update handler initialization.</summary>
    /// <param name="eventBus">Service for sending integration events.</param>
    /// <param name="identityService">User identification service.</param>
    /// <param name="urlService">Service for generating short URLs.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public UpdateHandler(IEventBus eventBus,
        IIdentityService identityService,
        IUrlService urlService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration) : base(eventBus, identityService, urlService, telegramBot, logger, configuration) { }

    /// <summary>Overriding the method for adding commands to the handler.</summary>
    /// <param name="commandSetBuilder">Command set builder.</param>
    protected override void OnCommandSetConfiguring(ICommandSetBuilder commandSetBuilder) => commandSetBuilder
        .AddCommand(new StartCommand(TelegramBot, Logger))
        .AddCommand(new GenerationUrlCommand(EventBus, UrlService, TelegramBot, Logger, Environment.GetEnvironmentVariable("FRONTEND")!))
        .AddCommand(new VerificationCommand(IdentityService, TelegramBot, Logger))
        .AddCommand(new FirstPageConnectionsCommand(IdentityService, TelegramBot, Logger, Configuration))
        .AddCommand(new ChangePageConnectionsCommand(IdentityService, TelegramBot, Logger, Configuration))
        .AddCommand(new CloseConnectionCommand(IdentityService, TelegramBot, Logger));

    /// <summary>Override the method to be called if no matching command is found.</summary>
    /// <param name="update">Telegram bot update.</param>
    protected override async Task NotFoundCommandHandleAsync(Update update)
    {
        Logger.LogStart("Handle update not found command");

        if (update is null)
        {
            Logger.LogError("Handle update not found command", "Update is null");
            return;
        }

        var updteId = update.Id.ToString();

        if (update.Message is { } message && message.From is { } user && !user.IsBot)
        {
            long chatId = message.Chat.Id;
            await TelegramBot.SendErrorMessageAsync(chatId, "Ссылка некорректна.");
            Logger.LogSuccessfully("Handle update not found command", updteId);
        }
        else
            Logger.LogWarning("Handle update not found command", "Invalid update type", updteId);
    }
}

