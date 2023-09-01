namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands;

public class GenerationUrlCommand : IUpdateCommand
{
    private readonly IUrlService _urlService;

    private readonly ITelegramBot _telegramBot;

    private readonly ILogger _logger;

    public GenerationUrlCommand(IUrlService urlService, ITelegramBot telegramBot, ILogger<IUpdateCommand> logger)
    {
        _urlService = urlService;
        _telegramBot = telegramBot;
        _logger = logger;
    }

    public async Task<bool> ExecuteIfValidAsync(Update update)
    {
        if (update is null || update.Message is not { } message || message.Text is not { } text ||
            !Uri.TryCreate(text, UriKind.Absolute, out Uri? uri) || uri is null ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return false;

        long chatId = message.Chat.Id;

        _logger.LogInformation($"Execute generation uri command: start.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tSource URI: {text}");

        try
        {
            var url = await _urlService.GenerateUrlAsync(text);
            await _telegramBot.SendUriAsync(chatId, url);
            _logger.LogInformation($"Execute generation uri command: succesful.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tSource URI: {text}");
        }
        catch (InvalidOperationException ex)
        {
            await _telegramBot.SendErrorMessageAsync(chatId, ex.Message);
            _logger.LogError(ex, $"Execute generation uri command: failed.\n\tUpdate Id: {update.Id}\n\tChat ID: {chatId}\n\tSource URI: {text}\n\tError: {ex.Message}");
        }

        return true;
    }
}

