namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Base;

public abstract class UpdateHandlerBase : IUpdateHandler, ICommandSetBuilder
{
    protected IUrlService UrlService { get; private init; }

    protected ITelegramBot TelegramBot { get; private init; }

    protected ILogger<IUpdateCommand> Logger { get; private init; }

    private readonly HashSet<IUpdateCommand> commands;

    public UpdateHandlerBase(IUrlService urlService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger)
    {
        UrlService = urlService;
        TelegramBot = telegramBot;
        Logger = logger;

        commands = new HashSet<IUpdateCommand>();

        CommandSetConfiguration(this);
    }

    public async Task HandleAsync(Update update)
    {
        foreach (var command in commands)
            if (await command.ExecuteIfValidAsync(update))
                return;

        await NotFoundCommandHandleAsync(update);
    }

    public ICommandSetBuilder AddCommand(IUpdateCommand command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        commands.Add(command);
        return this;
    }

    protected abstract void CommandSetConfiguration(ICommandSetBuilder commandSetBuilder);

    protected abstract Task NotFoundCommandHandleAsync(Update update);
}

