namespace ShortURLGenerator.TelegramBot.Services.Telegram;

public interface ITelegramBot : ITelegramBotClient
{
    Task SendHelloAsync(long chatId, string firstName);

    Task SendUriAsync(long chatId, string url);

    Task SendVerificationCodeAsync(long chatId, string code, int lifeTimeMinutes);

    Task SendConnectionsAsync(long userId, ConnectionsPageDto connectionsPage);

    Task SendCloseConnectionAsync(long userId, int messageId);

    Task SendErrorMessageAsync(long chatId, string? errorMessage = null);
}

