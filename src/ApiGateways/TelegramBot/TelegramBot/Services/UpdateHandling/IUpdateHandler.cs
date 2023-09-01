namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

public interface IUpdateHandler
{
    Task HandleAsync(Update update);
}
