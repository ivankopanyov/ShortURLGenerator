namespace ShortURLGenerator.GrpcHelper.Abstraction;

/// <summary>Bot connection service.</summary>
public interface IConnectionService
{
    /// <summary>Method for requesting a verification code.</summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Verification code.</returns>
    Task<VerificationCode> GetVerificationCodeAsync(long userId);

    /// <summary>Method for requesting active connections on the site.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="index">The index of the requested connection page.</param>
    /// <param name="size">The number of connections per page.</param>
    /// <returns>Page with a list of connections and information about yourself.</returns>
    Task<ConnectionsPage> GetConnectionsAsync(long userId, int index, int size);

    /// <summary>The method to close the user's connection to the site. The connection is made using gRPC.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="connectionId">Connection ID.</param>
    Task CloseConnectionAsync(long userId, string connectionId);
}

