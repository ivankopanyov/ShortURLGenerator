namespace ShortURLGenerator.Identity.API.Repositories.Connection;

public class ConnectionRepository : IConnectionRepository
{
    private const int DEFAULT_CONNECTION_LIFE_TIME_DAYS = 1;

    private const string PREFIX = "connection_";

    private readonly IDistributedCache _distributedCache;

    private readonly ILogger _logger;

    private readonly TimeSpan _connectionLifeTime;
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

    public async Task CreateAsync(string userId, string connectionId, ConnectionInfoDto connectionInfo)
    {
        _logger.LogStart("Create connection", userId);

        if (_distributedCache.GetStringAsync(connectionId) != null)
        {
            _logger.LogError("Create connection", userId);
            throw new DuplicateWaitObjectException(nameof(connectionId));
        }

        string prefixedUserId = $"{PREFIX}{userId}";

        var userConnectionsData = await _distributedCache.GetStringAsync(prefixedUserId);
        if (userConnectionsData is null || JsonSerializer.Deserialize<ISet<string>>(userConnectionsData) is not { } userConnections)
            userConnections = new HashSet<string>();

        userConnections.Add(connectionId);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _connectionLifeTime
        };

        var connection = new Models.Connection()
        {
            UserId = userId,
            ConnectionInfo = connectionInfo
        };

        await _distributedCache.SetStringAsync(prefixedUserId, JsonSerializer.Serialize(userConnections));
        await _distributedCache.SetStringAsync(connectionId, JsonSerializer.Serialize(connection), options);

        _logger.LogSuccessfully("Create connection", userId);
    }

    public async Task<ConnectionsPageDto> GetAsync(string userId, int index, int size)
    {
        _logger.LogStart("Get connections", userId);

        string prefixedUserId = $"{PREFIX}{userId}";
        var userConnectionsData = await _distributedCache.GetStringAsync(prefixedUserId);

        var response = new ConnectionsPageDto()
        {
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = 0
            }
        };

        if (userConnectionsData is null)
        {
            _logger.LogError("Get connections", "User connections data is null", userId);
            throw new InvalidOperationException("User connections data is null");
        }

        if (JsonSerializer.Deserialize<SortedSet<string>>(userConnectionsData) is not { } userConnections)
        {
            _logger.LogError("Get connections", "User connections data is not deserialized", userId);
            throw new InvalidOperationException("User connections data is not deserialized");
        }

        var counter = -1;

        foreach (var connectionId in userConnections)
        { 
            var connectionString = await _distributedCache.GetStringAsync(connectionId);
            if (connectionString is null)
            {
                userConnections.Remove(connectionId);
                continue;
            }

            if (++counter >= index * size && counter < (index + 1) * size)
            {
                var connection = JsonSerializer.Deserialize<Models.Connection>(connectionString);

                if (connection is null)
                {
                    counter--;
                    userConnections.Remove(connectionId);
                    continue;
                }

                response.Connections.Add(new ConnectionDto()
                {
                    ConnectionId = connectionId,
                    ActiveSecondsAgo = (DateTime.Now - connection.Created).Seconds,
                    ConnectionInfo = connection.ConnectionInfo
                }); 
            }
        }

        await _distributedCache.SetStringAsync(prefixedUserId, JsonSerializer.Serialize(userConnections));

        response.PageInfo.Index = index;
        response.PageInfo.Count = (int)Math.Ceiling((double)userConnections.Count / size);

        _logger.LogSuccessfully("Get connections", userId);

        return response;
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
