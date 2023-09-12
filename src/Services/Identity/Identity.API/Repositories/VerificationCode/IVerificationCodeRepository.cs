namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

public interface IVerificationCodeRepository
{
    Task<Models.VerificationCode?> GetAsync(string id);

    Task<Models.VerificationCode?> CreateAsync(Models.VerificationCode verificationCode);

    Task RemoveAsync(string id);

    Task<bool> Contains(string id);
}
