namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

public interface ICommandSetBuilder
{
    ICommandSetBuilder AddCommand(IUpdateCommand command);
}
