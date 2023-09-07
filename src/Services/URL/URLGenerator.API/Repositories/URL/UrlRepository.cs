namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

/// <summary>
/// Class that describes a repository of generated short URLs.
/// Implements the IUrlRepository interface.
/// </summary>
public class UrlRepository : IUrlRepository
{
    /// <summary>Database context.</summary>
    private readonly UrlContext _urlContext;

    /// <summary>Cache service.</summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Application configuration.</summary>
    protected IConfiguration _appConfiguration;

    /// <summary>How long the URL has been cached since the last request, in days.</summary>
    private readonly TimeSpan _lifeTimeCache;

    /// <summary>Repository object initialization.</summary>
    /// <param name="urlContext">Database context.</param>
    /// <param name="distributedCache">Cache service.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public UrlRepository(UrlContext urlContext,
        IDistributedCache distributedCache,
        ILogger<UrlRepository> logger,
        IConfiguration configuration)
    {
        _urlContext = urlContext;
        _distributedCache = distributedCache;
        _logger = logger;
        _appConfiguration = configuration;

        var urlRepositoryConfiguration = new UrlRepositoryConfiguration();
        OnConfiguring(urlRepositoryConfiguration);

        _lifeTimeCache = urlRepositoryConfiguration.LifeTimeCache;
	}

    /// <summary>Method for adding a new URL to the repository.</summary>
    /// <param name="item">URL address.</param>
    /// <exception cref="DuplicateWaitObjectException">
    /// Exception is thrown if the repository already contains a URL with the passed identifier.
    /// </exception>
	public async Task CreateAsync(Url item)
	{
        var itemId = item.Id;

        _logger.LogStart("Create URL", itemId);
        _logger.LogObject("Create URL", item);

        try
        {
            await _urlContext.Urls.AddAsync(item);
            await _urlContext.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Create URL", "Duplicate", itemId);
            throw new DuplicateWaitObjectException(ex.Message, ex);
        }

        await _distributedCache.SetStringAsync(item.Id,
            JsonSerializer.Serialize(item),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = _lifeTimeCache
            });

        _logger.LogSuccessfully("Create URL", itemId);
    }

    /// <summary>Method for requesting a URL from a repository.</summary>
    /// <param name="id">URL identifier.</param>
    /// <returns>The requested URL. Returns null if URL is not found.</returns>
    public async Task<Url?> GetAsync(string id)
    {
        _logger.LogStart("Get URL", id);

        Url? url = null;

        var urlString = await _distributedCache.GetStringAsync(id);
        if (urlString != null)
            url = JsonSerializer.Deserialize<Url>(urlString);

        if (url == null)
        {
            url = await _urlContext.Urls.FirstOrDefaultAsync(item => item.Id.Equals(id));
            if (url != null)
            {
                await _distributedCache.SetStringAsync(url.Id,
                    JsonSerializer.Serialize(url),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = _lifeTimeCache
                    });
            }
            else
            {
                _logger.LogInformation("Get URL", "URL not found", id);
                return null;
            }
        }

        _logger.LogObject("Get URL", url);
        _logger.LogSuccessfully("Get URL", id);

        return url;
    }

    /// <summary>Virtual method for configuring a repository.</summary>
    /// <param name="configuration">The repository configuration object.</param>
    protected virtual void OnConfiguring(UrlRepositoryConfiguration configuration)
    {
        int days = _appConfiguration.GetSection("Url").GetValue<int>("LifeTimeCacheDays");
        configuration.LifeTimeCache = TimeSpan.FromDays(days);
    }
}

