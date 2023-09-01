namespace ShortURLGenerator.TelegramBot.Services.Telegram;

public class TelegramBot : TelegramBotClient, ITelegramBot
{
    private static readonly string _frontend = Environment.GetEnvironmentVariable("FRONTEND")!;

    private readonly ILogger _logger;

    public TelegramBot(ILogger<TelegramBot> logger) : base(Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")!)
    {
        _logger = logger;
    }

    public async Task SendUriAsync(long chatId, string url)
    {
        string message = $"https://{_frontend}/{url}";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }

    public async Task SendErrorMessageAsync(long chatId, string errorMessage)
    {
        string message = $"Ошибка: {errorMessage ?? "Неизвестная ошибка."} Попробуйте снова.";
        await this.SendTextMessageAsync(chatId: chatId, text: message);
        _logger.LogInformation($"Send message to chat.\n\tChat ID: {chatId}\n\tMessage: {message}");
    }
}

