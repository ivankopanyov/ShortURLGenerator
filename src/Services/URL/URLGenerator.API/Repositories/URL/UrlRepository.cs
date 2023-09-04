namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

public class UrlRepository : IUrlRepository
{
    private readonly UrlContext _urlContext;

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger _logger;

    private readonly int _lifeTimeCacheDays;

	public UrlRepository(UrlContext urlContext,
        IDistributedCache distributedCache,
        ILogger<UrlRepository> logger,
        IConfiguration configuration)
    {
        _urlContext = urlContext;
        _distributedCache = distributedCache;
        _logger = logger;
        _lifeTimeCacheDays = configuration.GetSection("Url").GetValue<int>("LifeTimeCacheDays");
	}

	public async Task CreateAsync(Url item)
	{
        _logger.LogInformation($"Create URL: start.\n\t{item}");

        try
        {
            await _urlContext.Urls.AddAsync(item);
            await _urlContext.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Create URL: failed.\n\t{item}\n\tWarning: {ex.Message}");
            throw new DuplicateWaitObjectException(ex.Message, ex);
        }

        await _distributedCache.SetStringAsync(item.Id, item.SourceUri,
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(_lifeTimeCacheDays)
            });


        _logger.LogInformation($"Create URL: succesful.\n\t{item}");
    }
}

