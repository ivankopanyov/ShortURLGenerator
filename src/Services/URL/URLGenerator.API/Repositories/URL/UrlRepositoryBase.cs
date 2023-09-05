namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

public abstract class UrlRepositoryBase : IUrlRepository
{
    private readonly UrlContext _urlContext;

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger _logger;

    private readonly TimeSpan _lifeTimeCache;

    protected IConfiguration AppConfiguration { get; private init; }

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

    protected abstract void OnConfiguring(UrlRepositoryConfiguration configuration);
}

