namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

public interface IVerificationCodeRepository
{
    Task<Models.VerificationCode> CreateAsync(Models.VerificationCode item);

    Task RemoveByUserIdAsync(string userId);

    Task<bool> ContainsAsync(string id);
}
