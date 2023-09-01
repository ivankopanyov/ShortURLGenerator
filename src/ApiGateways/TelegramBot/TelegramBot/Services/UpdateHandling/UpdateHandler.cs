namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

public class UpdateHandler : UpdateHandlerBase
{
    public UpdateHandler(IUrlService urlService,
        ITelegramBot telegramBot,
        ILogger<IUpdateCommand> logger) : base(urlService, telegramBot, logger) { }

    protected override void CommandSetConfiguration(ICommandSetBuilder commandSetBuilder) => commandSetBuilder
        .AddCommand(new GenerationUrlCommand(UrlService, TelegramBot, Logger));

    protected override async Task NotFoundCommandHandleAsync(Update update)
    {
        if (update is not null && update.Message is { } message)
        {
            long chatId = message.Chat.Id;
            Logger.LogInformation($"Handle update: failed.\n\tUpdate ID: {update.Id}\n\tChat ID: {chatId}\n\tMessage: {message.Text}\n\tError: Command not found.");
            await TelegramBot.SendErrorMessageAsync(chatId, "Ссылка некорректна.");
        }
        else
            Logger.LogError($"Handle update: failed.\n\tError: Command not found.");
    }
}

