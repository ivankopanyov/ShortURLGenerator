namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

/// <summary>
/// Class that describes a repository of generated short URLs.
/// Implements the IUrlRepository interface.
/// </summary>
public class UrlRepository : IUrlRepository
{
    /// <summary>
    /// The default number of days to store a url in cache.
    /// Used if the url storage period is not set or set incorrectly.
    /// </summary>
    private const int DEFAULT_URL_CACHE_LIFE_TIME_DAYS = 1;

    /// <summary>Database context.</summary>
    private readonly UrlContext _urlContext;

    /// <summary>Distributed cache.</summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>How long the URL has been cached since the last request.</summary>
    private readonly TimeSpan _lifeTimeCache;

    /// <summary>Repository object initialization.</summary>
    /// <param name="urlContext">Database context.</param>
    /// <param name="distributedCache">Cache service.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public UrlRepository(UrlContext urlContext,
        IDistributedCache distributedCache,
        ILogger<UrlRepository> logger,
        IConfiguration? configuration = null)
    {
        _urlContext = urlContext;
        _distributedCache = distributedCache;
        _logger = logger;

        var repositoryConfiguration = new UrlRepositoryConfiguration();
        OnConfiguring(repositoryConfiguration, configuration);

        _lifeTimeCache = TimeSpan
                .FromMinutes(Math.Max(DEFAULT_URL_CACHE_LIFE_TIME_DAYS,
                    repositoryConfiguration.LifeTimeCache.Days));
    }

    /// <summary>Method for adding a new URL to the repository.</summary>
    /// <param name="item">URL address.</param>
    /// <exception cref="ArgumentNullException">Exception is thrown if the url is null.</exception>
    /// <exception cref="ArgumentException">Exception is thrown if the url ID or source URI is null or whitespace.</exception>
    /// <exception cref="DuplicateWaitObjectException">Exception is thrown if the url ID is already exists.</exception>
    /// <exception cref="InvalidOperationException">Exception is thrown if the URL could not be stored in the database.</exception>
	public async Task CreateAsync(Models.Url item)
    {
        _logger.LogInformation($"Create URL: Start. URL: {item}.");

        if (item is null)
        {
            _logger.LogError($"Create URL: URL is null.");
            throw new ArgumentNullException(nameof(item));
        }

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            _logger.LogError($"Create URL: URL ID is null or whitespace. URL: {item}");
            throw new ArgumentException("URL ID is null or whitespace.", nameof(item));
        }

        if (string.IsNullOrWhiteSpace(item.SourceUri))
        {
            _logger.LogError($"Create URL: Source URI is null or whitespace. URL: {item}");
            throw new ArgumentException("Source URI is null or whitespace.", nameof(item));
        }

        item.Created = DateTime.UtcNow;

        try
        {
            await _urlContext.Urls.AddAsync(item);
            await _urlContext.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            if (await _urlContext.Urls.AnyAsync(url => url.Id == item.Id))
            {
                _logger.LogError(ex, $"Create URL: Duplicate. URL: {item}");
                throw new DuplicateWaitObjectException(ex.Message, ex);
            }

            _logger.LogError(ex, $"Create URL: {ex.Message}. URL: {item}");
            throw new InvalidOperationException(ex.Message, ex);
        }

        await _distributedCache.SetStringAsync(item.Id, item.SourceUri, new DistributedCacheEntryOptions
        {
            SlidingExpiration = _lifeTimeCache
        });

        _logger.LogInformation($"Create URL: Succesfully. URL: {item}.");
    }

    /// <summary>Method for requesting source URI from a repository.</summary>
    /// <param name="id">URL identifier.</param>
    /// <returns>Source URI. Returns null if URL is not found.</returns>
    public async Task<string?> GetAsync(string id)
    {
        _logger.LogInformation($"Get Source URI: Start. URL ID: {id}.");

        if (await _distributedCache.GetStringAsync(id) is not { } sourceUri)
        {
            if (await _urlContext.Urls.FirstOrDefaultAsync(item => item.Id.Equals(id)) is { } entity)
            {
                sourceUri = entity.SourceUri;

                await _distributedCache.SetStringAsync(id, sourceUri, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = _lifeTimeCache
                });
            }
            else
            {
                _logger.LogInformation($"Get Source URI: Source URI not found. URL ID: {id}.");
                return null;
            }
        }

        _logger.LogInformation($"Get Source URI: Succesfully. URL ID: {id}, Source URI: {sourceUri}.");

        return sourceUri;
    }


    /// <summary>Virtual method for configuring a repository.</summary>
    /// <param name="repositoryConfiguration">Repository configuration.</param>
    /// <param name="appConfiguration">Application configuration.</param>
    protected virtual void OnConfiguring(UrlRepositoryConfiguration repositoryConfiguration, IConfiguration? appConfiguration)
    {
        if (appConfiguration != null)
        {
            var days = appConfiguration
                .GetSection("Url")
                .GetValue<int>("LifeTimeCacheDays");

            repositoryConfiguration.LifeTimeCache = TimeSpan
                .FromMinutes(Math.Max(DEFAULT_URL_CACHE_LIFE_TIME_DAYS, days));
        }
        else
            repositoryConfiguration.LifeTimeCache = TimeSpan
                .FromMinutes(DEFAULT_URL_CACHE_LIFE_TIME_DAYS);
    }
}

