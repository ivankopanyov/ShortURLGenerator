namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

/// <summary>Verification code repository.</summary>
public interface IVerificationCodeRepository
{
    /// <summary>Method for adding new verification code to the repository.</summary>
    /// <param name="item">New verification code.</param>
    /// <returns>Created verification code.</returns>
    Task<Grpc.Services.VerificationCode> CreateAsync(Grpc.Services.VerificationCode item);

    /// <summary>Method for removing verification code from the repository by user ID.</summary>
    /// <param name="userId">User ID.</param>
    Task RemoveByUserIdAsync(string userId);

    /// <summary>Method that returns user ID by verification code.</summary>
    /// <param name="verificationCode">Verification code.</param>
    /// <returns>User ID.</returns>
    Task<string?> GetUserIdAsync(string verificationCode);

    /// <summary>Method for checking whether a repository is verification code.</summary>
    /// <param name="id">Verification code ID.</param>
    /// <returns>Result of checking.</returns>
    Task<bool> ContainsAsync(string id);
}
