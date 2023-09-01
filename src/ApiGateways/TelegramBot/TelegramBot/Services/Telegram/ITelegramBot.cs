namespace ShortURLGenerator.TelegramBot.Services.Telegram;

public interface ITelegramBot : ITelegramBotClient
{
	Task SendUriAsync(long chatId, string url);

    Task SendErrorMessageAsync(long chatId, string errorMessage);
}

