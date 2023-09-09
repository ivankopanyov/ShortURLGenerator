using Microsoft.Extensions.Caching.Distributed;

namespace ShortURLGenerator.Identity.API.Repositories.Connection;

public class ConnectionRepository : IConnectionRepository
{
    private const int DEFAULT_CONNECTION_LIFE_TIME_DAYS = 1;

    private const string PREFIX = "connection_";

    /// <summary>Cache service.</summary>
    private readonly IDistributedCache _distributedCache;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    private readonly TimeSpan _connectionLifeTime;

    /// <summary>Repository object initialization.</summary>
    /// <param name="distributedCache">Cache service.</param>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
	public ConnectionRepository(IDistributedCache distributedCache,
        ILogger<ConnectionRepository> logger,
        IConfiguration? configuration = null)
    {
        _distributedCache = distributedCache;
        _logger = logger;

        var repositoryConfiguration = new ConnectionRepositoryConfiguration();
        OnConfiguring(repositoryConfiguration, configuration);

        _connectionLifeTime =
            repositoryConfiguration.ConnectionLifeTime.Days < DEFAULT_CONNECTION_LIFE_TIME_DAYS
                ? TimeSpan.FromMinutes(DEFAULT_CONNECTION_LIFE_TIME_DAYS)
                : repositoryConfiguration.ConnectionLifeTime;
    }

    protected virtual void OnConfiguring(
        ConnectionRepositoryConfiguration repositoryConfiguration,
        IConfiguration? appConfiguration = null)
    {
        if (appConfiguration != null)
        {
            var days = appConfiguration
                .GetSection("Connection")
                .GetValue<int>("LifeTimeDays");

            repositoryConfiguration.ConnectionLifeTime = TimeSpan
                .FromDays(Math.Max(DEFAULT_CONNECTION_LIFE_TIME_DAYS, days));
        }
        else
            repositoryConfiguration.ConnectionLifeTime = TimeSpan
                .FromDays(DEFAULT_CONNECTION_LIFE_TIME_DAYS);
    }
}
