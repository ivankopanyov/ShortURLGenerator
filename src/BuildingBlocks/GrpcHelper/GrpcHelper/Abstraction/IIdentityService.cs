namespace ShortURLGenerator.GrpcHelper.Abstraction;

/// <summary>User identity service.</summary>
public interface IIdentityService
{
    /// <summary>Method for creating a new connection to a site.</summary>
    /// <param name="verificationCode">Verification code.</param>
    /// <param name="connectionInfo">Connection info.</param>
    /// <returns>Access tokens.</returns>
    Task<Token> SignInAsync(string verificationCode, ConnectionInfo connectionInfo);
}