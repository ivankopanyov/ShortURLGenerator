namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling.Commands.Abstract;

/// <summary>Command executed by the update handler.</summary>
public interface IUpdateCommand
{
    /// <summary>The method that executes the command if the update is valid.</summary>
    /// <param name="update">Telegram bot update.</param>
    Task<bool> ExecuteIfValidAsync(Update update);
}

