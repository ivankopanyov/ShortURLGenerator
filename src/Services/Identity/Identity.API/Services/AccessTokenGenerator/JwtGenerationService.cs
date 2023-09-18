namespace ShortURLGenerator.Identity.API.Services.AccessTokenGenerator;

/// <summary>Class describing the service for generating JWT tokens.</summary>
public class JwtGenerationService : IAccessTokenGenerationService
{
    /// <summary>The default expiration date of the token is in minutes.</summary>
    private const int DEFAULT_EXPIRATION_MINUTES = 1;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>The expiration date of the token is in minutes.</summary>
    private readonly int _expirationMinutes;

    /// <summary>Secret key for creating a JWT token.</summary>
    private readonly string _jwtKey;

    /// <summary>Initializing a service object.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <exception cref="ArgumentException">Exception is thrown if the secret key is null or whitespace.</exception>
    public JwtGenerationService(ILogger<JwtGenerationService> logger, IConfiguration? configuration = null)
    {
        _logger = logger;

        var serviceConfiguration = new JwtGenerationServiceConfiguration();
        OnConfiguring(serviceConfiguration, configuration);

        if (string.IsNullOrWhiteSpace(serviceConfiguration.JwtKey))
            throw new ArgumentException("Jwt key is null or whitespace.", nameof(serviceConfiguration.JwtKey));

        _expirationMinutes = Math.Max(DEFAULT_EXPIRATION_MINUTES, serviceConfiguration.ExpirationMinutes);
        _jwtKey = serviceConfiguration.JwtKey;
    }

    /// <summary>Access token generation method.</summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Access token.</returns>
    public string CreateToken(long userId)
    {
        _logger.LogInformation($"Create token: Start. User ID: {userId}.");

        var claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var expiration = DateTime.UtcNow.AddMinutes(_expirationMinutes);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(claims: claims, expires: expiration, signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        _logger.LogInformation($"Create token: Succesfully. User ID: {userId}, Token: {token}.");

        return token;
    }

    /// <summary>Virtual method for configuring a service.</summary>
    /// <param name="serviceConfiguration">JWT generation service configuration.</param>
    /// <param name="appConfiguration">Application configuration.</param>
    protected virtual void OnConfiguring(JwtGenerationServiceConfiguration serviceConfiguration, IConfiguration? appConfiguration)
    {
        serviceConfiguration.JwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;

        if (appConfiguration is not null)
            serviceConfiguration.ExpirationMinutes = appConfiguration
                .GetSection("Jwt")
                .GetValue<int>("ExpirationMinutes");
    }
}

