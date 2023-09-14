namespace ShortURLGenerator.Identity.API.Models;

/// <summary>Class that describes the verification code model.</summary>
public class VerificationCode
{
    /// <summary>Verification code ID.</summary>
    public string Id { get; set; }

    /// <summary>Verification code user ID.</summary>
    public string UserId { get; set; }

    /// <summary>The lifetime of the verification code from the moment of creation.</summary>
    public TimeSpan LifeTime { get; set; }
}
