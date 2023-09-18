namespace ShortURLGenerator.Identity.API.Services.AccessTokenGenerator;

/// <summary>Class describing JWT generation service configuration.</summary>
public class JwtGenerationServiceConfiguration
{
    /// <summary>The expiration date of the token is in minutes.</summary>
    public int ExpirationMinutes { get; set; }

    /// <summary>Secret key for creating a JWT token.</summary>
    public string JwtKey { get; set; }
}

