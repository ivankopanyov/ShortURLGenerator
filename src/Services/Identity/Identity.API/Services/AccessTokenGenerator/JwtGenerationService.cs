namespace ShortURLGenerator.Identity.API.Services.AccessTokenGenerator;

public class JwtGenerationService : IAccessTokenGenerationService
{
    private const int DEFAULT_EXPIRATION_MINUTES = 1;

    private readonly ILogger _logger;

    private readonly int _expirationMinutes;

    private readonly string _jwtKey;

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

    protected virtual void OnConfiguring(JwtGenerationServiceConfiguration serviceConfiguration, IConfiguration? appConfiguration)
    {
        serviceConfiguration.JwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;

        if (appConfiguration is not null)
            serviceConfiguration.ExpirationMinutes = appConfiguration
                .GetSection("Jwt")
                .GetValue<int>("ExpirationMinutes");
    }
}

