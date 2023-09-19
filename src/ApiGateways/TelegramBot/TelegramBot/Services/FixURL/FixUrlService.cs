namespace ShortURLGenerator.TelegramBot.Services.FixURL;

/// <summary>Service for fixing URL.</summary>
public class FixUrlService : IFixUrlService
{
    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Frontend host name.</summary>
    private readonly string _frontend;

    /// <summary>Initializing a service object.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public FixUrlService(ILogger<FixUrlService> logger, IConfiguration? configuration = null)
    {
        _logger = logger;

        var serviceConfiguration = new FixUrlServiceConfiguration();
        OnConfiguring(serviceConfiguration, configuration);

        _frontend = serviceConfiguration.Frontend;
    }

    /// <summary>URL fix method.</summary>
    /// <param name="url">Source URL.</param>
    /// <returns>Corrected URL.</returns>
    public string FixUrl(string url)
    {
        _logger.LogInformation($"Fix URL: Start. URL: {url}.");

        var result = $"https://{_frontend}/{url}";

        _logger.LogInformation($"Fix URL: Successfully. Result: {result}.");

        return result;
    }

    /// <summary>Virtual method for configuring a service.</summary>
    /// <param name="serviceConfiguration">Service configuration.</param>
    /// <param name="appConfiguration">Application configuration.</param>
    protected virtual void OnConfiguring(FixUrlServiceConfiguration serviceConfiguration, IConfiguration? appConfiguration)
    {
        serviceConfiguration.Frontend = Environment.GetEnvironmentVariable("FRONTEND")!;
    }
}
