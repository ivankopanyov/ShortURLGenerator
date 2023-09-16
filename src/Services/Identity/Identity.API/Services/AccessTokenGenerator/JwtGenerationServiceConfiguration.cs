namespace ShortURLGenerator.Identity.API.Services.AccessTokenGenerator;

public class JwtGenerationServiceConfiguration
{
    public int ExpirationMinutes { get; set; }

    public string JwtKey { get; set; }
}

