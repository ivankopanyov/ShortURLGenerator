namespace ShortURLGenerator.TelegramBot.Services.Url;

/// <summary>
/// Class that describes a service for generating short URLs.
/// Inherited from the UrlServiceBase class.
/// </summary>
public class UrlService : UrlServiceBase
{
    /// <summary>Initialization of the service object for generating short URLs.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public UrlService(ILogger<UrlServiceBase> logger, IConfiguration configuration) : base(logger, configuration) { }

    /// <summary>Overriding the configuration method of the service for generating short URLs.</summary>
    /// <param name="configuration">Configuration object of the service for generating short URLs.</param>
    protected override void OnConfiguring(UrlServiceConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("URL_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable("URL_SERVICE_PORT")!;
        configuration.UrlServiceHost = $"http://{host}:{port}";
    }
}

