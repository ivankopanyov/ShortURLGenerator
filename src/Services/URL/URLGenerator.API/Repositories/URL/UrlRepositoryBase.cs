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
        _logger.LogInformation($"Create URL: start.\n\t{item}");

        try
        {
            _logger.LogInformation($"Create URL: save to database.\n\t{item}");
            await _urlContext.Urls.AddAsync(item);
            await _urlContext.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Create URL: failed.\n\t{item}\n\tWarning: {ex.Message}");
            throw new DuplicateWaitObjectException(ex.Message, ex);
        }

        _logger.LogInformation($"Create URL: save to cache.\n\t{item}");

        await _distributedCache.SetStringAsync(item.Id,
            JsonSerializer.Serialize(item),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = _lifeTimeCache
            });


        _logger.LogInformation($"Create URL: succesful.\n\t{item}");
    }

    /// <summary>Method for requesting a URL from a repository.</summary>
    /// <param name="id">URL identifier.</param>
    /// <returns>The requested URL. Returns null if URL is not found.</returns>
    public async Task<Url?> GetAsync(string id)
    {
        _logger.LogInformation($"Get URL: start.\n\tID: {id}");

        Url? url = null;

        _logger.LogInformation($"Get URL: get from cache.\n\tID: {id}");

        var urlString = await _distributedCache.GetStringAsync(id);
        if (urlString != null)
            url = JsonSerializer.Deserialize<Url>(urlString);

        if (url == null)
        {
            _logger.LogInformation($"Get URL: get from database.\n\tID: {id}");
            url = await _urlContext.Urls.FirstOrDefaultAsync(item => item.Id.Equals(id));
            if (url != null)
            {
                _logger.LogInformation($"Get URL: save to cache.\n\tURL: {url}");
                await _distributedCache.SetStringAsync(url.Id,
                    JsonSerializer.Serialize(url),
                    new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = _lifeTimeCache
                    });
            }
            else
                _logger.LogInformation($"Get URL: failed.\n\tID: {id}\n\tError: URL not found.");
        }

        return url;
    }

    /// <summary>Abstract method for configuring a repository.</summary>
    /// <param name="configuration">The repository configuration object.</param>
    protected abstract void OnConfiguring(UrlRepositoryConfiguration configuration);
}

