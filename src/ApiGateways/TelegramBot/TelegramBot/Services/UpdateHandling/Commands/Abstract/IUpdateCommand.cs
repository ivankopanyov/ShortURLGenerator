namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Abstract;

public interface IUpdateCommand
{
    Task<bool> ExecuteIfValidAsync(Update update);
}

