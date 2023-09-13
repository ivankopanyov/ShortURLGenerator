namespace ShortURLGenerator.Identity.API.Extensions;

public static class LoggerExtensions
{
	public static string LogInfo(this ConnectionInfoDto connectionInfo) =>
        $"OS: {connectionInfo.Os}, " +
        $"Browser: {connectionInfo.Browser}, " +
        $"Location: {connectionInfo.Location}, " +
        $"IP: {connectionInfo.Ip}";

    public static string LogInfo(this Connection connection) =>
        $"ID: {connection.Id}, " +
        $"User ID: {connection.UserId}, " +
        $"Created: {connection.Created.ToString("dd.MM.yyyy HH:mm:ss.ffff")}, " +
        $"Info: {connection.ConnectionInfo?.LogInfo()}";

    public static string LogInfo(this PageInfoDto pageInfo) =>
        $"Index: {pageInfo.Index}, " +
        $"Count: {pageInfo.Count}";

    public static string LogInfo(this ConnectionsPageDto connectionsPage) =>
        $"Count: {connectionsPage.Connections.Count}, " +
        $"Page info: {connectionsPage.PageInfo.LogInfo()}";

    public static string LogInfo(this VerificationCode verificationCode) =>
        $"ID: {verificationCode.Id}, " +
        $"User ID: {verificationCode.UserId}, " +
        $"Life time minutes: {verificationCode.LifeTime.Minutes}";

    public static string LogInfo(this VerificationCodeDto verificationCode) =>
        $"Code: {verificationCode.Code}, " +
        $"Life time minutes: {verificationCode.LifeTimeMinutes}";

    public static string LogInfo(this ResponseDto response) =>
        $"Status: {response.ResponseStatus}, " +
        $"Error message: {response.Error}";

    public static string LogInfo(this VerificationCodeResponseDto verificationCodeResponse) =>
        $"Response: {verificationCodeResponse.Response.LogInfo()}, " +
        $"User ID: {verificationCodeResponse.VerificationCode.LogInfo()}";

    public static string LogInfo(this ConnectionsPageResponseDto connectionsPageResponse) =>
        $"Response: {connectionsPageResponse.Response.LogInfo()}, " +
        $"Connections page: {connectionsPageResponse.ConnectionsPage.LogInfo()}";

    public static string LogInfo(this UserIdDto userId) => $"User ID: {userId.UserId}";

    public static string LogInfo(this ConnectionsRequestDto connectionsRequest) =>
        $"User ID: {connectionsRequest.UserId}, " +
        $"Page info: {connectionsRequest.PageInfo.LogInfo()}";

    public static string LogInfo(this ConnectionRequestDto connectionRequest) =>
        $"User ID: {connectionRequest.UserId}, " +
        $"Connection ID: {connectionRequest.ConnectionId}";
}

