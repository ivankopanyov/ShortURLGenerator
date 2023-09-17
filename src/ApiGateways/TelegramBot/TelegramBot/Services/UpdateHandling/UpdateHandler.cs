namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

/// <summary>Inherited class that describes the basic handler for telegram bot updates.</summary>
public class UpdateHandler : UpdateHandlerBase
{
    /// <summary>Update handler initialization.</summary>
    /// <param name="eventBus">Service for sending integration events.</param>
    /// <param name="connectionService">User connection service.</param>
    /// <param name="urlGenerator">Service for generating short URLs.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public UpdateHandler(IEventBus eventBus,
        IConnectionService connectionService,
        IUrlGenerator urlGenerator,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration) : base(eventBus, connectionService, urlGenerator, telegramBot, logger, configuration) { }

    /// <summary>Overriding the method for adding commands to the handler.</summary>
    /// <param name="commandSetBuilder">Command set builder.</param>
    protected override void OnCommandSetConfiguring(ICommandSetBuilder commandSetBuilder) => commandSetBuilder
        .AddCommand(new StartCommand(TelegramBot, Logger))
        .AddCommand(new GenerationUrlCommand(EventBus, UrlGenerator, TelegramBot, Logger, Environment.GetEnvironmentVariable("FRONTEND")!))
        .AddCommand(new VerificationCommand(ConnectionService, TelegramBot, Logger))
        .AddCommand(new FirstPageConnectionsCommand(ConnectionService, TelegramBot, Logger, Configuration))
        .AddCommand(new ChangePageConnectionsCommand(ConnectionService, TelegramBot, Logger, Configuration))
        .AddCommand(new CloseConnectionCommand(ConnectionService, TelegramBot, Logger));

    /// <summary>Override the method to be called if no matching command is found.</summary>
    /// <param name="update">Telegram bot update.</param>
    protected override async Task NotFoundCommandHandleAsync(Update update)
    {
        Logger.LogInformation($"Handle update not found command: Start. {update?.LogInfo()}");

        if (update is null)
        {
            Logger.LogError($"Handle update not found command: Update is null.");
            return;
        }

        if (update.Message is { } message && message.From is { } user && !user.IsBot)
        {
            long chatId = message.Chat.Id;
            await TelegramBot.SendErrorMessageAsync(chatId, "Ссылка некорректна.");
            Logger.LogInformation($"Handle update not found command: Successfully. {update.LogInfo()}");
        }
        else
            Logger.LogError($"Handle update not found command: Invalid update type. {update.LogInfo()}");
    }
}

