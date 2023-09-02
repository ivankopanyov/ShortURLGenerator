namespace ShortURLGenerator.TelegramBot.Services.Identity;

/// <summary>Telegram user identification service.</summary>
public interface IIdentityService
{
    /// <summary>Method for requesting a verification code.</summary>
    /// <param name="userId">User ID.</param>
    /// <returns>VerificationCode</returns>
    Task<VerificationCodeDto> GetVerificationCodeAsync(long userId);

    /// <summary>Method for requesting active connections on the site.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="index">The index of the requested connection page.</param>
    /// <param name="size">The number of connections per page.</param>
    /// <returns>Page with a list of connections and information about yourself.</returns>
    Task<ConnectionsPageDto> GetConnectionsAsync(long userId, int index, int size);

    /// <summary>The method to close the user's connection to the site.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="connectionId">Connection ID.</param>
    Task CloseConnectionAsync(long userId, string connectionId);
}

