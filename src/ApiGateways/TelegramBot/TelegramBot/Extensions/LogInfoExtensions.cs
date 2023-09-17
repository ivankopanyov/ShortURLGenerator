namespace ShortURLGenerator.TelegramBot.Extensions;

/// <summary>Static class of extensions for logging information about objects.</summary>
public static class LogInfoExtensions
{
    /// <summary>Method for obtaining information about an object of the User class.</summary>
    /// <param name="user">Object of the User class.</param>
    /// <returns>Information about an object of the User class.</returns>
    public static string LogInfo(this User user) =>
        "User: { ID: " + user.Id +
        ", Is bot: " + user.IsBot +
        ", First name: " + user.FirstName + " }";

    /// <summary>Method for obtaining information about an object of the Chat class.</summary>
    /// <param name="chat">Object of the Chat class.</param>
    /// <returns>Information about an object of the Chat class.</returns>
    public static string LogInfo(this Chat chat) =>
        "Chat: { ID: " + chat.Id + " }";

    /// <summary>Method for obtaining information about an object of the CallbackQuery class.</summary>
    /// <param name="callbackQuery">Object of the CallbackQuery class.</param>
    /// <returns>Information about an object of the CallbackQuery class.</returns>
    public static string LogInfo(this CallbackQuery callbackQuery) =>
        "Callback query { ID: " + callbackQuery.Id +
        ", Data: " + callbackQuery.Data +
        ", " + callbackQuery.From.LogInfo() +
        ", " + callbackQuery.Message?.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the Message class.</summary>
    /// <param name="message">Object of the Message class.</param>
    /// <returns>Information about an object of the Message class.</returns>
    public static string LogInfo(this Message message) =>
        "Message: { Message ID: " + message.MessageId +
        ", Text: " + message.Text +
        ", Reply to message: " + message.ReplyToMessage +
        ", " + message.Chat.LogInfo() +
        ", " + message.From?.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the Update class.</summary>
    /// <param name="update">Object of the Update class.</param>
    /// <returns>Information about an object of the Update class.</returns>
    public static string LogInfo(this Update update) =>
        "Uodate { ID: " + update.Id +
        ", " + update.CallbackQuery?.LogInfo() +
        ", " + update.Message?.LogInfo() + " }";
}
