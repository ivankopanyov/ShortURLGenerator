namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Base;

/// <summary>Abstract class that describes the basic handler for telegram bot updates.</summary>
public abstract class UpdateHandlerBase : IUpdateHandler, ICommandSetBuilder
{
    /// <summary>User identification service.</summary>
    protected IIdentityService IdentityService { get; private init; }

    /// <summary>Service for generating short URLs.</summary>
    protected IUrlService UrlService { get; private init; }

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    protected ITelegramBot TelegramBot { get; private init; }

    /// <summary>Log service.</summary>
    protected ILogger<IUpdateCommand> Logger { get; private init; }

    /// <summary>Application configuration.</summary>
    protected IConfiguration Configuration { get; private init; }

    /// <summary>The set of commands to be executed by the handler.</summary>
    private readonly HashSet<IUpdateCommand> commands;

    /// <summary>Update handler initialization.</summary>
    /// <param name="identityService">User identification service.</param>
    /// <param name="urlService">Service for generating short URLs.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public UpdateHandlerBase(IIdentityService identityService,
        IUrlService urlService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration)
    {
        IdentityService = identityService;
        UrlService = urlService;
        TelegramBot = telegramBot;
        Logger = logger;
        Configuration = configuration;

        commands = new HashSet<IUpdateCommand>();

        CommandSetConfiguration(this);
    }

    /// <summary>Telegram bot update handling method.</summary>
    /// <param name="update">Telegram bot update.</param>
    public async Task HandleAsync(Update update)
    {
        foreach (var command in commands)
            if (await command.ExecuteIfValidAsync(update))
                return;

        await NotFoundCommandHandleAsync(update);
    }

    /// <summary>Method for adding command to a handler.</summary>
    /// <param name="command">The command to add.</param>
    /// <returns>Command set builder.</returns>
    /// <exception cref="ArgumentNullException">Command is null.</exception>
    public ICommandSetBuilder AddCommand(IUpdateCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        commands.Add(command);
        return this;
    }

    /// <summary>Abstract method for adding commands to the handler using the command set builder.</summary>
    /// <param name="commandSetBuilder">Command set builder.</param>
    protected abstract void CommandSetConfiguration(ICommandSetBuilder commandSetBuilder);

    /// <summary>Abstract method is called if no suitable update command is found.</summary>
    /// <param name="update">Telegram bot update.</param>
    protected abstract Task NotFoundCommandHandleAsync(Update update);
}

