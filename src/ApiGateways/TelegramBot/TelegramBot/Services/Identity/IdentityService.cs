namespace ShortURLGenerator.TelegramBot.Services.Identity;

/// <summary>
/// Class that describes a Telegram user identification service.
/// Inherited from the IdentityServiceBase class.
/// </summary>
public class IdentityService : IdentityServiceBase
{
    /// <summary>Initialization of the user identification service service object.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public IdentityService(ILogger<IdentityService> logger, IConfiguration configuration)
        : base(logger, configuration) { }

    /// <summary>Overriding the configuration method of the Telegram user identification service.</summary>
    /// <param name="configuration">Configuration object of the Telegram user identification service.</param>
    protected override void OnConfiguring(IdentityServiceConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("IDENTITY_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable("IDENTITY_SERVICE_PORT");
        configuration.IdentityServiceHost = $"http://{host}:{port}";
    }
}

