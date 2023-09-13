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

    public async Task<Models.Connection?> GetOrDefaultAsync(string id)
    {
        var methodName = nameof(GetOrDefaultAsync);
        string connectionId = $"Connection ID: {id}";

        _logger.LogStart(methodName, connectionId);

        var connectionData = await _distributedCache.GetStringAsync(id);

        if (connectionData is null)
        {
            _logger.LogError(methodName, "Connection not found", connectionId);
            return null;
        }

        var connection = JsonSerializer.Deserialize<Models.Connection>(connectionData);

        if (connection is null)
        {
            _logger.LogError(methodName, "Connection not deserialized", connectionId);
            await RemoveAsync(id);
        }
        else
            _logger.LogSuccessfully(methodName, $"Connection: {connection.LogInfo()}");

        return connection;
    }

    public async Task<ConnectionsPageDto> GetByUserIdAsync(string userId, int index, int size)
    {
        var methodName = nameof(GetOrDefaultAsync);
        string logUserId = $"User ID: {userId}";

        _logger.LogStart(methodName, $"{userId}, Index: {index}, Size: {size}");

        string prefixedUserId = $"{PREFIX}{userId}";

        var response = new ConnectionsPageDto()
        {
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = 0
            }
        };

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } connectionsData)
        {
            _logger.LogInformation(methodName, "User connections not found", logUserId);
            return response;
        };

        if (JsonSerializer.Deserialize<SortedSet<string>>(connectionsData) is not { } userConnections)
        {
            _logger.LogError(methodName, "User connections not deserialized", logUserId);
            return response;
        }

        var counter = -1;

        foreach (var connectionId in userConnections)
        {
            var connectionString = await _distributedCache.GetStringAsync(connectionId);
            if (connectionString is null)
            {
                _logger.LogWarning(methodName, "Connection is null", $"Connection ID: {connectionId}");
                userConnections.Remove(connectionId);
                continue;
            }

            if (++counter >= index * size && counter < (index + 1) * size)
            {
                var connection = JsonSerializer.Deserialize<Models.Connection>(connectionString);

                if (connection is null)
                {
                    _logger.LogError(methodName, "Connection not deserialized", $"Connection ID: {connectionId}");
                    counter--;
                    userConnections.Remove(connectionId);
                    continue;
                }

                _logger.LogInformation(methodName, "Connection added to response", $"Connection: {connection.LogInfo()}");

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

        _logger.LogSuccessfully(methodName, $"Connections page: {response.LogInfo()}");

        return response;
    }

    public async Task<bool> ContainsAsync(string id) => await _distributedCache.GetStringAsync(id) is not null;

    public async Task<Models.Connection> CreateAsync(Models.Connection item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (string.IsNullOrWhiteSpace(item.Id))
            throw new ArgumentException("Connection ID is null or whitespace.", nameof(item));

        if (string.IsNullOrWhiteSpace(item.UserId))
            throw new ArgumentException("User ID is null or whitespace.", nameof(item));

        if (await ContainsAsync(item.Id))
            throw new InvalidOperationException("Connection ID is already exists.");

        var methodName = nameof(CreateAsync);

        _logger.LogStart(methodName, $"Connection: {item.LogInfo()}");

        item.Created = DateTime.UtcNow;
        if (item.ConnectionInfo is null)
            item.ConnectionInfo = new ConnectionInfoDto();

        var prefixedUserId = $"{PREFIX}{item.UserId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } connectionsData ||
            JsonSerializer.Deserialize<SortedSet<string>>(connectionsData) is not { } connections)
            connections = new SortedSet<string>();

        connections.Add(item.Id);

        await _distributedCache.SetStringAsync(prefixedUserId, JsonSerializer.Serialize(connections));
        await _distributedCache.SetStringAsync(item.Id, JsonSerializer.Serialize(item), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _connectionLifeTime
        });

        _logger.LogSuccessfully(methodName, $"Connection: {item.LogInfo()}");

        return item;
    }

    public async Task RemoveAsync(string id)
    {
        var methodName = nameof(RemoveAsync);
        string connectionId = $"Connection ID: {id}";

        _logger.LogStart(methodName, connectionId);

        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogError(methodName, "Connection ID is null or whitespace", connectionId);
            return;
        }

        if (await _distributedCache.GetAsync(id) is not { } connectionData)
        {
            _logger.LogWarning(methodName, "Connection not found", connectionId);
            return;
        }

        if (JsonSerializer.Deserialize<Models.Connection>(connectionData) is not { } connection)
        {
            _logger.LogError(methodName, "Connection not deserialized", connectionId);
            await _distributedCache.RemoveAsync(id);
            return;
        }

        var prefixedUserId = $"{PREFIX}{connection.UserId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is { } connectionsData &&
            JsonSerializer.Deserialize<ISet<string>>(connectionsData) is { } connections)
        {
            connections.Remove(connection.Id);
            await _distributedCache.SetStringAsync(prefixedUserId, JsonSerializer.Serialize(connections));
        }
        else
            _logger.LogError(methodName, "User connections not deserialized", $"User ID: {connection.UserId}");

        await _distributedCache.RemoveAsync(connection.Id);

        _logger.LogSuccessfully(methodName, $"Connection: {connection.LogInfo()}");
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
