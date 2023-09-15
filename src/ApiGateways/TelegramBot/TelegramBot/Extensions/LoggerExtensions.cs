namespace ShortURLGenerator.TelegramBot.Extensions;

/// <summary>Static class of extensions for logging information about objects.</summary>
public static class LoggerExtensions
{
    /// <summary>Method for obtaining information about an object of the User class.</summary>
    /// <param name="user">Object of the User class.</param>
    /// <returns>Information about an object of the User class.</returns>
    public static string LogInfo(this User user) =>
        $"User ID: {user.Id}, " +
        $"Is bot: {user.IsBot}, " +
        $"First name: {user.FirstName}";

    /// <summary>Method for obtaining information about an object of the Chat class.</summary>
    /// <param name="chat">Object of the Chat class.</param>
    /// <returns>Information about an object of the Chat class.</returns>
    public static string LogInfo(this Chat chat) =>
        $"Chat ID: {chat.Id}";

    /// <summary>Method for obtaining information about an object of the CallbackQuery class.</summary>
    /// <param name="callbackQuery">Object of the CallbackQuery class.</param>
    /// <returns>Information about an object of the CallbackQuery class.</returns>
    public static string LogInfo(this CallbackQuery callbackQuery) =>
        $"Callback query ID: {callbackQuery.Id}, " +
        $"From: {callbackQuery.From}, " +
        $"Data: {callbackQuery.Data}, " +
        $"Message: {callbackQuery.Message?.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the Message class.</summary>
    /// <param name="message">Object of the Message class.</param>
    /// <returns>Information about an object of the Message class.</returns>
    public static string LogInfo(this Message message) =>
        $"Message ID: {message.MessageId}, " +
        $"Chat: {message.Chat.LogInfo()}" +
        $"From: {message.From?.LogInfo()}, " +
        $"Text: {message.Text}, " +
        $"Reply to message: {message.ReplyToMessage}";

    /// <summary>Method for obtaining information about an object of the Update class.</summary>
    /// <param name="update">Object of the Update class.</param>
    /// <returns>Information about an object of the Update class.</returns>
    public static string LogInfo(this Update update) =>
        $"Uodate ID: {update.Id}, " +
        $"Callback query: {update.CallbackQuery?.LogInfo()}, " +
        $"Message: {update.Message?.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the PageInfoDto class.</summary>
    /// <param name="pageInfo">Object of the PageInfoDto class.</param>
    /// <returns>Information about an object of the PageInfoDto class.</returns>
    public static string LogInfo(this PageInfoDto pageInfo) =>
        $"Index: {pageInfo.Index}, " +
        $"Count: {pageInfo.Count}";

    /// <summary>Method for obtaining information about an object of the ConnectionsPageDto class.</summary>
    /// <param name="connectionsPage">Object of the ConnectionsPageDto class.</param>
    /// <returns>Information about an object of the ConnectionsPageDto class.</returns>
    public static string LogInfo(this ConnectionsPageDto connectionsPage) =>
        $"Page info: {connectionsPage.PageInfo.LogInfo()}, " +
        $"Connections count: {connectionsPage.Connections.Count}";

    /// <summary>Method for obtaining information about an object of the ConnectionInfoDto class.</summary>
    /// <param name="connectionInfo">Object of the ConnectionInfoDto class.</param>
    /// <returns>Information about an object of the ConnectionInfoDto class.</returns>
    public static string LogInfo(this ConnectionInfoDto connectionInfo) =>
        $"OS: {connectionInfo.Os}, " +
        $"Browser: {connectionInfo.Browser}, " +
        $"Location: {connectionInfo.Location}, " +
        $"IP: {connectionInfo.Ip}";

    /// <summary>Method for obtaining information about an object of the ConnectionDto class.</summary>
    /// <param name="connection">Object of the ConnectionDto class.</param>
    /// <returns>Information about an object of the ConnectionDto class.</returns>
    public static string LogInfo(this ConnectionDto connection) =>
        $"Connection ID: {connection.ConnectionId}, " +
        $"Active seconds ago: {connection.ActiveSecondsAgo}, " +
        $"Connection info: {connection.ConnectionInfo.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the ResponseDto class.</summary>
    /// <param name="response">Object of the ResponseDto class.</param>
    /// <returns>Information about an object of the ResponseDto class.</returns>
    public static string LogInfo(this ResponseDto response) =>
        $"Status: {response.ResponseStatus}, " +
        $"Error message: {response.Error}";

    /// <summary>Method for obtaining information about an object of the UrlResponseDto class.</summary>
    /// <param name="urlResponse">Object of the UrlResponseDto class.</param>
    /// <returns>Information about an object of the UrlResponseDto class.</returns>
    public static string LogInfo(this UrlResponseDto urlResponse) =>
        $"Response: {urlResponse.Response.LogInfo()}, " +
        $"URL: {urlResponse.Url}";

    /// <summary>Method for obtaining information about an object of the SourceUriDto class.</summary>
    /// <param name="sourceUri">Object of the SourceUriDto class.</param>
    /// <returns>Information about an object of the SourceUriDto class.</returns>
    public static string LogInfo(this SourceUriDto sourceUri) =>
        $"Source URI: {sourceUri.Value}";

    /// <summary>Method for obtaining information about an object of the UserIdDto class.</summary>
    /// <param name="userId">Object of the UserIdDto class.</param>
    /// <returns>Information about an object of the UserIdDto class.</returns>
    public static string LogInfo(this UserIdDto userId) =>
        $"User ID: {userId.UserId}";

    /// <summary>Method for obtaining information about an object of the VerificationCodeDto class.</summary>
    /// <param name="verificationCode">Object of the VerificationCodeDto class.</param>
    /// <returns>Information about an object of the VerificationCodeDto class.</returns>
    public static string LogInfo(this VerificationCodeDto verificationCode) =>
        $"Verification code: {verificationCode.Code}, " +
        $"Life time minutes: {verificationCode.LifeTimeMinutes}";

    /// <summary>Method for obtaining information about an object of the VerificationCodeResponseDto class.</summary>
    /// <param name="verificationCodeResponse">Object of the VerificationCodeResponseDto class.</param>
    /// <returns>Information about an object of the VerificationCodeResponseDto class.</returns>
    public static string LogInfo(this VerificationCodeResponseDto verificationCodeResponse) =>
        $"Response: {verificationCodeResponse.Response.LogInfo()}, " +
        $"Verification code: {verificationCodeResponse.VerificationCode?.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the ConnectionsRequestDto class.</summary>
    /// <param name="connectionsRequest">Object of the ConnectionsRequestDto class.</param>
    /// <returns>Information about an object of the ConnectionsRequestDto class.</returns>
    public static string LogInfo(this ConnectionsRequestDto connectionsRequest) =>
        $"User ID: {connectionsRequest.UserId}, " +
        $"Page info: {connectionsRequest.PageInfo.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the ConnectionsPageResponseDto class.</summary>
    /// <param name="connectionsPageResponse">Object of the ConnectionsPageResponseDto class.</param>
    /// <returns>Information about an object of the ConnectionsPageResponseDto class.</returns>
    public static string LogInfo(this ConnectionsPageResponseDto connectionsPageResponse) =>
        $"Response: {connectionsPageResponse.Response.LogInfo()}, " +
        $"Connections page: {connectionsPageResponse.ConnectionsPage?.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the ConnectionRequestDto class.</summary>
    /// <param name="connectionRequest">Object of the ConnectionRequestDto class.</param>
    /// <returns>Information about an object of the ConnectionRequestDto class.</returns>
    public static string LogInfo(this ConnectionRequestDto connectionRequest) =>
        $"Connection ID: {connectionRequest.ConnectionId}, " +
        $"User ID: {connectionRequest.UserId}";
}

