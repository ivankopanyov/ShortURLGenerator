namespace ShortURLGenerator.GrpcHelper.Extensions;

/// <summary>Static class of extensions for logging information about objects.</summary>
public static class LogInfoExtensions
{
    /// <summary>Method for obtaining information about an object of the Response class.</summary>
    /// <param name="response">Object of the Response class.</param>
    /// <returns>Information about an object of the Response class.</returns>
    public static string LogInfo(this Response response) =>
        "Response: { Status: " + response.ResponseStatus +
        ", Error message: " + response.Error + " }";

    /// <summary>Method for obtaining information about an object of the UserId class.</summary>
    /// <param name="userId">Object of the UserId class.</param>
    /// <returns>Information about an object of the UserId class.</returns>
    public static string LogInfo(this UserId userId) =>
        "User ID: { Value: " + userId.Value + " }";

    /// <summary>Method for obtaining information about an object of the VerificationCode class.</summary>
    /// <param name="verificationCode">Object of the VerificationCode class.</param>
    /// <returns>Information about an object of the VerificationCode class.</returns>
    public static string LogInfo(this VerificationCode verificationCode) =>
        "Verification code: { ID: " + verificationCode.Id +
        ", User ID: " + verificationCode.UserId +
        ", Life time minutes: " + verificationCode.LifeTimeMinutes + " }";

    /// <summary>Method for obtaining information about an object of the VerificationCodeResponse class.</summary>
    /// <param name="verificationCodeResponse">Object of the VerificationCodeResponse class.</param>
    /// <returns>Information about an object of the VerificationCodeResponse class.</returns>
    public static string LogInfo(this VerificationCodeResponse verificationCodeResponse) =>
        "Verification code response: { " + verificationCodeResponse.Response.LogInfo() +
        ", " + verificationCodeResponse.VerificationCode?.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the PageInfo class.</summary>
    /// <param name="pageInfo">Object of the PageInfo class.</param>
    /// <returns>Information about an object of the PageInfo class.</returns>
    public static string LogInfo(this PageInfo pageInfo) =>
        "Page info: { Index: " + pageInfo.Index +
        ", Count: " + pageInfo.Count + " }";

    /// <summary>Method for obtaining information about an object of the ConnectionsRequest class.</summary>
    /// <param name="connectionsRequest">Object of the ConnectionsRequest class.</param>
    /// <returns>Information about an object of the ConnectionsRequest class.</returns>
    public static string LogInfo(this ConnectionsRequest connectionsRequest) =>
        "Connections request: { User ID: " + connectionsRequest.UserId +
        ", " + connectionsRequest.PageInfo.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the ConnectionInfo class.</summary>
    /// <param name="connectionInfo">Object of the ConnectionInfo class.</param>
    /// <returns>Information about an object of the ConnectionInfo class.</returns>
    public static string LogInfo(this ConnectionInfo connectionInfo) =>
        "Connection info: { OS: " + connectionInfo.Os +
        ", Browser: " + connectionInfo.Browser +
        ", Location: " + connectionInfo.Location +
        ", IP: " + connectionInfo.Ip + " }";

    /// <summary>Method for obtaining information about an object of the Connection class.</summary>
    /// <param name="connection">Object of the Connection class.</param>
    /// <returns>Information about an object of the Connection class.</returns>
    public static string LogInfo(this Connection connection) =>
        "Connection: { ID: " + connection.Id +
        ", User ID: " + connection.UserId + 
        ", Last activity: " + DateTimeOffset.FromUnixTimeSeconds(connection.LastActivity).ToString("dd.MM.yyyy HH:mm:ss.ffff") +
        ", " + connection.ConnectionInfo.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the ConnectionsPage class.</summary>
    /// <param name="connectionsPage">Object of the ConnectionsPage class.</param>
    /// <returns>Information about an object of the ConnectionsPage class.</returns>
    public static string LogInfo(this ConnectionsPage connectionsPage) =>
        "Connections page: " + connectionsPage.PageInfo.LogInfo() +
        ", Connections count: " + connectionsPage.Connections.Count + " }";

    /// <summary>Method for obtaining information about an object of the ConnectionsPageResponse class.</summary>
    /// <param name="connectionsPageResponse">Object of the ConnectionsPageResponse class.</param>
    /// <returns>Information about an object of the ConnectionsPageResponse class.</returns>
    public static string LogInfo(this ConnectionsPageResponse connectionsPageResponse) =>
        "Connections page response: " + connectionsPageResponse.Response.LogInfo() +
        ", " + connectionsPageResponse.ConnectionsPage?.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the ConnectionRequest class.</summary>
    /// <param name="connectionRequest">Object of the ConnectionRequest class.</param>
    /// <returns>Information about an object of the ConnectionRequest class.</returns>
    public static string LogInfo(this ConnectionRequest connectionRequest) =>
        "Connection request: { Connection ID: " + connectionRequest.ConnectionId +
        ", User ID: " + connectionRequest.UserId + " }";

    /// <summary>Method for obtaining information about an object of the SourceUri class.</summary>
    /// <param name="sourceUri">Object of the SourceUri class.</param>
    /// <returns>Information about an object of the SourceUri class.</returns>
    public static string LogInfo(this SourceUri sourceUri) =>
        "Source URI: { Value: " + sourceUri.Value + " }";

    /// <summary>Method for obtaining information about an object of the UrlResponse class.</summary>
    /// <param name="urlResponse">Object of the UrlResponse class.</param>
    /// <returns>Information about an object of the UrlResponse class.</returns>
    public static string LogInfo(this UrlResponse urlResponse) =>
        "URL response: { " + urlResponse.Response.LogInfo() +
        ", URL: " + urlResponse.Url + " }";

    /// <summary>Method for obtaining information about an object of the UrlDto class.</summary>
    /// <param name="url">Object of the UrlDto class.</param>
    /// <returns>Information about an object of the UrlDto class.</returns>
    public static string LogInfo(this Url url) =>
        "Url: { Value : " + url.Value + " }";

    /// <summary>Method for obtaining information about an object of the UriResponseDto class.</summary>
    /// <param name="uriResponse">Object of the UriResponseDto class.</param>
    /// <returns>Information about an object of the UriResponseDto class.</returns>
    public static string LogInfo(this UriResponse uriResponse) =>
        "URI response: {" + uriResponse.Response.LogInfo() +
        "URI: " + uriResponse.Uri + " }";

    /// <summary>Method for obtaining information about an object of the SignInRequest class.</summary>
    /// <param name="signInRequest">Object of the SignInRequest class.</param>
    /// <returns>Information about an object of the SignInRequest class.</returns>
    public static string LogInfo(this SignInRequest signInRequest) =>
        "Sign in request: { Verification code: " + signInRequest.VerificationCode +
        ", " + signInRequest.ConnectionInfo.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the Token class.</summary>
    /// <param name="token">Object of the Token class.</param>
    /// <returns>Information about an object of the Token class.</returns>
    public static string LogInfo(this Token token) =>
        "Token: { Access token: " + token.AccessToken +
        ", Refresh token: " + token.RefreshToken + " }";

    /// <summary>Method for obtaining information about an object of the TokenResponse class.</summary>
    /// <param name="tokenResponse">Object of the TokenResponse class.</param>
    /// <returns>Information about an object of the TokenResponse class.</returns>
    public static string LogInfo(this TokenResponse tokenResponse) =>
        "Sign in response: { " + tokenResponse.Response +
        ", " + tokenResponse.Token?.LogInfo() + " }";

    /// <summary>Method for obtaining information about an object of the RefreshTokenRequest class.</summary>
    /// <param name="refreshTokenRequest">Object of the RefreshTokenRequest class.</param>
    /// <returns>Information about an object of the RefreshTokenRequest class.</returns>
    public static string LogInfo(this RefreshTokenRequest refreshTokenRequest) =>
        "Refresh token request: { User ID: " + refreshTokenRequest.UserId +
        ", Refresh token: " + refreshTokenRequest.RefreshToken +
        ", " + refreshTokenRequest.ConnectionInfo.LogInfo() + " }";
}
