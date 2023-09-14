namespace ShortURLGenerator.Identity.API.Repositories.VerificationCode;

/// <summary>Class that describes the verification code repository configuration.</summary>
public class VerificationCodeRepositoryConfiguration
{
    /// <summary>The length of time the verification code is stored in the repository.</summary>
    public TimeSpan VerificationCodeLifeTime { get; set; }
}
