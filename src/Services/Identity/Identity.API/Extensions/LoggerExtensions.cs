namespace ShortURLGenerator.Identity.API.Extensions;

/// <summary>Static class of extensions for logging information about objects.</summary>
public static class LoggerExtensions
{
    /// <summary>Method for obtaining information about an object of the ConnectionInfoDto class.</summary>
    /// <param name="connectionInfo">Object of the ConnectionInfoDto class.</param>
    /// <returns>Information about an object of the ConnectionInfoDto class.</returns>
	public static string LogInfo(this ConnectionInfoDto connectionInfo) =>
        $"OS: {connectionInfo.Os}, " +
        $"Browser: {connectionInfo.Browser}, " +
        $"Location: {connectionInfo.Location}, " +
        $"IP: {connectionInfo.Ip}";

    /// <summary>Method for obtaining information about an object of the Connection class.</summary>
    /// <param name="connection">Object of the Connection class.</param>
    /// <returns>Information about an object of the Connection class.</returns>
    public static string LogInfo(this Connection connection) =>
        $"ID: {connection.Id}, " +
        $"User ID: {connection.UserId}, " +
        $"Created: {connection.Created.ToString("dd.MM.yyyy HH:mm:ss.ffff")}, " +
        $"Info: {connection.ConnectionInfo?.LogInfo()}";

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
        $"Count: {connectionsPage.Connections.Count}, " +
        $"Page info: {connectionsPage.PageInfo.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the VerificationCode class.</summary>
    /// <param name="verificationCode">Object of the VerificationCode class.</param>
    /// <returns>Information about an object of the VerificationCode class.</returns>
    public static string LogInfo(this VerificationCode verificationCode) =>
        $"ID: {verificationCode.Id}, " +
        $"User ID: {verificationCode.UserId}, " +
        $"Life time minutes: {verificationCode.LifeTime.Minutes}";

    /// <summary>Method for obtaining information about an object of the VerificationCodeDto class.</summary>
    /// <param name="verificationCode">Object of the VerificationCodeDto class.</param>
    /// <returns>Information about an object of the VerificationCodeDto class.</returns>
    public static string LogInfo(this VerificationCodeDto verificationCode) =>
        $"Code: {verificationCode.Code}, " +
        $"Life time minutes: {verificationCode.LifeTimeMinutes}";

    /// <summary>Method for obtaining information about an object of the ResponseDto class.</summary>
    /// <param name="response">Object of the ResponseDto class.</param>
    /// <returns>Information about an object of the ResponseDto class.</returns>
    public static string LogInfo(this ResponseDto response) =>
        $"Status: {response.ResponseStatus}, " +
        $"Error message: {response.Error}";

    /// <summary>Method for obtaining information about an object of the VerificationCodeResponseDto class.</summary>
    /// <param name="verificationCodeResponse">Object of the VerificationCodeResponseDto class.</param>
    /// <returns>Information about an object of the VerificationCodeResponseDto class.</returns>
    public static string LogInfo(this VerificationCodeResponseDto verificationCodeResponse) =>
        $"Response: {verificationCodeResponse.Response.LogInfo()}, " +
        $"Verification code: {verificationCodeResponse.VerificationCode.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the ConnectionsPageResponseDto class.</summary>
    /// <param name="connectionsPageResponse">Object of the ConnectionsPageResponseDto class.</param>
    /// <returns>Information about an object of the ConnectionsPageResponseDto class.</returns>
    public static string LogInfo(this ConnectionsPageResponseDto connectionsPageResponse) =>
        $"Response: {connectionsPageResponse.Response.LogInfo()}, " +
        $"Connections page: {connectionsPageResponse.ConnectionsPage.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the UserIdDto class.</summary>
    /// <param name="userId">Object of the UserIdDto class.</param>
    /// <returns>Information about an object of the UserIdDto class.</returns>
    public static string LogInfo(this UserIdDto userId) => $"User ID: {userId.UserId}";

    /// <summary>Method for obtaining information about an object of the ConnectionsRequestDto class.</summary>
    /// <param name="connectionsRequest">Object of the ConnectionsRequestDto class.</param>
    /// <returns>Information about an object of the ConnectionsRequestDto class.</returns>
    public static string LogInfo(this ConnectionsRequestDto connectionsRequest) =>
        $"User ID: {connectionsRequest.UserId}, " +
        $"Page info: {connectionsRequest.PageInfo.LogInfo()}";

    /// <summary>Method for obtaining information about an object of the ConnectionRequestDto class.</summary>
    /// <param name="connectionRequest">Object of the ConnectionRequestDto class.</param>
    /// <returns>Information about an object of the ConnectionRequestDto class.</returns>
    public static string LogInfo(this ConnectionRequestDto connectionRequest) =>
        $"User ID: {connectionRequest.UserId}, " +
        $"Connection ID: {connectionRequest.ConnectionId}";
}

