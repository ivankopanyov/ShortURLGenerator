﻿namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Base;

/// <summary>Abstract class that describes the basic handler for telegram bot updates.</summary>
public abstract class UpdateHandlerBase : IUpdateHandler, ICommandSetBuilder
{
    /// <summary>Service for sending integration events.</summary>
    protected IEventBus EventBus { get; private init; }

    /// <summary>User connection service.</summary>
    protected IConnectionService ConnectionService { get; private init; }

    /// <summary>Service for generating short URLs.</summary>
    protected IUrlGenerator UrlGenerator { get; private init; }

    /// <summary>Service for sending Telegram messages to a bot.</summary>
    protected ITelegramBot TelegramBot { get; private init; }

    /// <summary>Log service.</summary>
    protected ILogger<IUpdateCommand> Logger { get; private init; }

    /// <summary>Application configuration.</summary>
    protected IConfiguration Configuration { get; private init; }

    /// <summary>The set of commands to be executed by the handler.</summary>
    private readonly HashSet<IUpdateCommand> commands;

    /// <summary>Update handler initialization.</summary>
    /// <param name="eventBus">Service for sending integration events.</param>
    /// <param name="connectionService">User connection service.</param>
    /// <param name="urlGenerator">Service for generating short URLs.</param>
    /// <param name="telegramBot">Service for sending Telegram messages to a bot.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public UpdateHandlerBase(IEventBus eventBus,
        IConnectionService connectionService,
        IUrlGenerator urlGenerator,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger,
        IConfiguration configuration)
    {
        EventBus = eventBus;
        ConnectionService = connectionService;
        UrlGenerator = urlGenerator;
        TelegramBot = telegramBot;
        Logger = logger;
        Configuration = configuration;

        commands = new HashSet<IUpdateCommand>();

        OnCommandSetConfiguring(this);
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
    protected abstract void OnCommandSetConfiguring(ICommandSetBuilder commandSetBuilder);

    /// <summary>Abstract method is called if no suitable update command is found.</summary>
    /// <param name="update">Telegram bot update.</param>
    protected abstract Task NotFoundCommandHandleAsync(Update update);
}

