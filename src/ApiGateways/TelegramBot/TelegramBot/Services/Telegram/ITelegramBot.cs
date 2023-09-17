namespace ShortURLGenerator.TelegramBot.Services.Telegram;

/// <summary>Service for sending Telegram messages to a bot.</summary>
public interface ITelegramBot : ITelegramBotClient
{
    /// <summary>Method for sending a welcome message to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="firstName">First name of user.</param>
    Task SendHelloAsync(long chatId, string firstName);

    /// <summary>Method for sending the generated link to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="uri">Generated URI.</param>
    /// <param name="sourceUri">Source URI.</param>
    /// <returns>Message ID.</returns>
    Task<int> SendUriAsync(long chatId, string uri, string sourceUri);

    /// <summary>Method for sending the generated link to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="uri">Generated URI.</param>
    /// <param name="sourceMessageId">Source message ID.</param>
    /// <returns>Message ID.</returns>
    Task<int> SendUriAsync(long chatId, string uri, int sourceMessageId);

    /// <summary>Method for sending verification code to chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="code">Verification code</param>
    /// <param name="lifeTimeMinutes">Verification code lifetime in minutes.</param>
    Task SendVerificationCodeAsync(long chatId, string code, int lifeTimeMinutes);

    /// <summary>Method for sending a list of active connections to a chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="connectionsPage">Page with a list of active connections.</param>
    Task SendConnectionsAsync(long chatId, ConnectionsPage connectionsPage);

    /// <summary>The method for sending a message to close the connection to a chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="messageId">The ID of the original message with connection information.</param>
    Task SendCloseConnectionAsync(long chatId, int messageId);

    /// <summary>Method of sending QR code to chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="messageId">Source message ID.</param>
    /// <param name="data">QR code data.</param>
    Task SendQRCodeAsync(long chatId, int messageId, byte[] data);

    /// <summary>Method for sending an error message to the chat.</summary>
    /// <param name="chatId">Chat ID.</param>
    /// <param name="errorMessage">Message describing the error.</param>
    Task SendErrorMessageAsync(long chatId, string? errorMessage = null);
}

