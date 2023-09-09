namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

public interface IVerificationCodeRepository
{
    Task<TimeSpan> CreateAsync(string userId, string verificationCode);

    Task<string?> GetAndRemoveAsync(string verificationCode);
}
