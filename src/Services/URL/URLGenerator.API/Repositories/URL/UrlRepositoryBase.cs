namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

/// <summary>
/// Abstract class that describes a repository of generated short URLs.
/// Implements the IUrlRepository interface.
/// </summary>
public abstract class UrlRepositoryBase : IUrlRepository
{
    /// <summary>Database context.</summary>
    private readonly UrlContext _urlContext;

    /// <summary>Cache service.</summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>How long the URL has been cached since the last request, in days.</summary>
    private readonly TimeSpan _lifeTimeCache;

    /// <summary>Application configuration.</summary>
    protected IConfiguration AppConfiguration { get; private init; }

    /// <summary>Repository object initialization.</summary>
    /// <param name="urlContext">Database context.</param>
    /// <param name="distributedCache">Cache service.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public UrlRepositoryBase(UrlContext urlContext,
        IDistributedCache distributedCache,
        ILogger<UrlRepositoryBase> logger,
        IConfiguration configuration)
    {
        _urlContext = urlContext;
        _distributedCache = distributedCache;
        _logger = logger;
        AppConfiguration = configuration;

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
        _logger.LogStart("Create URL", item);

        try
        {
            _logger.LogStart("Save URL to database", item);
            await _urlContext.Urls.AddAsync(item);
            await _urlContext.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Save URL to database", "Duplicate", item);
            throw new DuplicateWaitObjectException(ex.Message, ex);
        }

        _logger.LogSuccessfully("Save URL to database", item);

        _logger.LogStart($"Save URL to cache", item);

        await _distributedCache.SetStringAsync(item.Id,
            JsonSerializer.Serialize(item),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = _lifeTimeCache
            });

        _logger.LogSuccessfully("Save URL to cache", item);
        _logger.LogSuccessfully("Create URL", item);
    }

    /// <summary>Method for requesting a URL from a repository.</summary>
    /// <param name="id">URL identifier.</param>
    /// <returns>The requested URL. Returns null if URL is not found.</returns>
    public async Task<Url?> GetAsync(string id)
    {
        _logger.LogStart("Get URL", id);

        Url? url = null;

        _logger.LogStart("Get URL from cache", id);

        var urlString = await _distributedCache.GetStringAsync(id);
        if (urlString != null)
            url = JsonSerializer.Deserialize<Url>(urlString);

        if (url == null)
        {
            _logger.LogWarning("Get URL from cache", "URL not found.", id);
            _logger.LogStart("Get URL from database", id);

            url = await _urlContext.Urls.FirstOrDefaultAsync(item => item.Id.Equals(id));
            if (url != null)
            {
                _logger.LogSuccessfully("Get URL from database", url);
                _logger.LogStart("Save URL to cache", url);
                await _distributedCache.SetStringAsync(url.Id,
                    JsonSerializer.Serialize(url),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = _lifeTimeCache
                    });
                _logger.LogSuccessfully("Save URL to cache", url);
            }
            else
                _logger.LogWarning("Get URL from database", "URL not found", id);
        }

        _logger.LogSuccessfully("Get URL", url ?? (object)id);

        return url;
    }

    /// <summary>Abstract method for configuring a repository.</summary>
    /// <param name="configuration">The repository configuration object.</param>
    protected abstract void OnConfiguring(UrlRepositoryConfiguration configuration);
}

