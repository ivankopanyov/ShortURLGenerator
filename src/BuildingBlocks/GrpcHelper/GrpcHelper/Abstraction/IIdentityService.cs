namespace ShortURLGenerator.GrpcHelper.Abstraction;

/// <summary>User identity service.</summary>
public interface IIdentityService
{
    /// <summary>Method for creating a new connection to a site.</summary>
    /// <param name="verificationCode">Verification code.</param>
    /// <param name="connectionInfo">Connection info.</param>
    /// <returns>Access tokens.</returns>
    Task<Token> SignInAsync(string verificationCode, ConnectionInfo connectionInfo);

    /// <summary>The method to close the user's connection to the site. The connection is made using gRPC.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="connectionId">Connection ID.</param>
    Task CloseConnectionAsync(long userId, string connectionId);

    /// <summary>Method for creating a new connection and deleting an old connection.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="refreshToken">Refresh token or connection ID.</param>
    /// <param name="connectionInfo">Connection info.</param>
    /// <returns>Access tokens.</returns>
    Task<Token> RefreshTokenAsync(long userId, string refreshToken, ConnectionInfo connectionInfo);
}