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

    public async Task<Models.Connection?> GetAsync(string connectionId) =>
        await _distributedCache.GetStringAsync(connectionId) is not { } connectionData
            ? null : JsonSerializer.Deserialize<Models.Connection>(connectionData);

    public async Task<ConnectionsPageDto> GetAsync(string userId, int index, int size)
    {
        string prefixedUserId = $"{PREFIX}{userId}";

        var response = new ConnectionsPageDto()
        {
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = 0
            }
        };

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } connectionsData ||
            JsonSerializer.Deserialize<SortedSet<string>>(connectionsData) is not { } userConnections)
            return response;

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

        response.PageInfo.Count = (int)Math.Ceiling((double)userConnections.Count / size);

        return response;
    }

    public async Task<bool> Contains(string id) =>
        await _distributedCache.GetStringAsync(id) is not null;

    public async Task<Models.Connection?> CreateAsync(Models.Connection connection)
    {
        if (connection is null ||
            string.IsNullOrWhiteSpace(connection.Id) ||
            string.IsNullOrWhiteSpace(connection.UserId))
            return null;

        connection.Created = DateTime.UtcNow;
        if (connection.ConnectionInfo is null)
            connection.ConnectionInfo = new ConnectionInfoDto();

        var prefixedUserId = $"{PREFIX}{connection.UserId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } connectionsData ||
            JsonSerializer.Deserialize<SortedSet<string>>(connectionsData) is not { } connections)
            connections = new SortedSet<string>();

        connections.Add(connection.Id);

        await _distributedCache.SetStringAsync(prefixedUserId, JsonSerializer.Serialize(connection));
        await _distributedCache.SetStringAsync(connection.Id, JsonSerializer.Serialize(connection), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _connectionLifeTime
        });

        return connection;
    }

    public async Task RemoveAsync(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return;

        if (await _distributedCache.GetAsync(connectionId) is not { } connectionData ||
            JsonSerializer.Deserialize<Models.Connection>(connectionData) is not { } connection)
            return;

        var prefixedUserId = $"{PREFIX}{connection.UserId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is { } connectionsData &&
            JsonSerializer.Deserialize<ISet<string>>(connectionsData) is { } connections)
            connections.Remove(connection.Id);

        await _distributedCache.RemoveAsync(connection.Id);
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
