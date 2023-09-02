namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

/// <summary>Telegram bot update handler.</summary>
public interface IUpdateHandler
{
    /// <summary>Telegram bot update handling method.</summary>
    /// <param name="update">Telegram bot update.</param>
    Task HandleAsync(Update update);
}
