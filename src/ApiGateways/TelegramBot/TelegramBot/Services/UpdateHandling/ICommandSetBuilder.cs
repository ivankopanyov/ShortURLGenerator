namespace ShortURLGenerator.TelegramBot.Services.UpdateHandling;

/// <summary>Command set builder</summary>
public interface ICommandSetBuilder
{
    /// <summary>Method for adding command to a handler.</summary>
    /// <param name="command">The command to add.</param>
    /// <returns>Command set builder.</returns>
    ICommandSetBuilder AddCommand(IUpdateCommand command);
}
