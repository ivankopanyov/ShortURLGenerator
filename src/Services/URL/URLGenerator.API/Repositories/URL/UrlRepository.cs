namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

public class UrlRepository : UrlRepositoryBase
{
    public UrlRepository(UrlContext urlContext,
        IDistributedCache distributedCache,
        ILogger<UrlRepository> logger,
        IConfiguration configuration) : base(urlContext, distributedCache, logger, configuration) { }

    protected override void OnConfiguring(UrlRepositoryConfiguration configuration)
    {
        int days = AppConfiguration.GetSection("Url").GetValue<int>("LifeTimeCacheDays");
        configuration.LifeTimeCache = TimeSpan.FromDays(days);
    }
}

