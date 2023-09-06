namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

/// <summary>Class that describes a repository of generated short URLs.
/// Inherited from the UrlRepositoryBase class
/// </summary>
public class UrlRepository : UrlRepositoryBase
{
    /// <summary>Repository object initialization.</summary>
    /// <param name="urlContext">Database context.</param>
    /// <param name="distributedCache">Cache service.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public UrlRepository(UrlContext urlContext,
        IDistributedCache distributedCache,
        ILogger<UrlRepository> logger,
        IConfiguration configuration) : base(urlContext, distributedCache, logger, configuration) { }

    /// <summary>Repository configuration method override.</summary>
    /// <param name="configuration">The repository configuration object.</param>
    protected override void OnConfiguring(UrlRepositoryConfiguration configuration)
    {
        int days = AppConfiguration.GetSection("Url").GetValue<int>("LifeTimeCacheDays");
        configuration.LifeTimeCache = TimeSpan.FromDays(days);
    }
}

