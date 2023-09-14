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
        _logger.LogInformation($"Get connection by ID: Start. Connection ID: {id}.");

        if (await _distributedCache.GetStringAsync(id) is not { } connectionData)
        {
            _logger.LogWarning($"Get connection by ID: Connection not found. Connection ID: {id}.");
            return null;
        }

        if (JsonSerializer.Deserialize<Models.Connection>(connectionData) is { } connection)
        {
            _logger.LogInformation($"Get connection by ID: Succesfully. Connection: {connection.LogInfo()}.");
            return connection;
        }
        else
        {
            _logger.LogWarning($"Get connection by ID: Connection not serialized. Connection ID: {id}.");
            await RemoveAsync(id);
            return null;
        }
    }

    public async Task<ConnectionsPageDto> GetByUserIdAsync(string userId, int index, int size)
    {
        _logger.LogInformation($"Get connection by user ID: Start. User ID: {userId}, Index: {index}, Size: {size}");

        var response = new ConnectionsPageDto()
        {
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = 0
            }
        };

        if (size <= 0)
        {
            _logger.LogError($"Get connection by user ID: Page size is less than or equal to 0. User ID: {userId}, Index: {index}, Size: {size}");
            return response;
        }

        string prefixedUserId = $"{PREFIX}{userId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } connectionsData)
        {
            _logger.LogInformation($"Get connection by user ID: User connections not found. User ID: {userId}, Index: {index}, Size: {size}");
            return response;
        };

        if (JsonSerializer.Deserialize<SortedSet<string>>(connectionsData) is not { } userConnections)
        {
            _logger.LogError($"Get connection by user ID: User connections not serialized. User ID: {userId}, Index: {index}, Size: {size}");
            await _distributedCache.RemoveAsync(prefixedUserId);
            return response;
        }

        var counter = -1;

        foreach (var connectionId in userConnections)
        {
            if (await GetConnectionStringData(connectionId) is not { } connectionString)
                continue;

            if (++counter >= index * size && counter < (index + 1) * size)
            {
                if (ConnectionStringDataToConnection(connectionString) is not { } connection)
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

        _logger.LogInformation($"Get connection by user ID: Succesfully. Connections page: ${response.LogInfo()}.");

        return response;
    }

    public async Task<bool> ContainsAsync(string id) => await _distributedCache.GetStringAsync(id) is not null;

    public async Task<Models.Connection> CreateAsync(Models.Connection item)
    {
        _logger.LogInformation($"Create connection: Start. Connection: {item?.LogInfo()}.");

        if (item is null)
        {
            _logger.LogError($"Create connection: Connection is null.");
            throw new ArgumentNullException(nameof(item));
        }

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            _logger.LogError($"Create connection: Connection ID is null or whitespace. Connection: {item.LogInfo()}.");
            throw new ArgumentException("Connection ID is null or whitespace.", nameof(item));
        }

        if (string.IsNullOrWhiteSpace(item.UserId))
        {
            _logger.LogError($"Create connection: User ID is null or whitespace. Connection: {item.LogInfo()}.");
            throw new ArgumentException("User ID is null or whitespace.", nameof(item));
        }

        if (await ContainsAsync(item.Id))
        {
            _logger.LogError($"Create connection: Connection ID is already exists. Connection: {item.LogInfo()}.");
            throw new InvalidOperationException("Connection ID is already exists.");
        }

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

        _logger.LogInformation($"Create connection: Succesfully. Connection: {item.LogInfo()}.");

        return item;
    }

    public async Task RemoveAsync(string id)
    {
        _logger.LogInformation($"Remove connection: Start. Connection ID: {id}.");

        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogError($"Remove connection: Connection ID is null or whitespace. Connection ID: {id}");
            return;
        }

        if (await _distributedCache.GetAsync(id) is not { } connectionData)
        {
            _logger.LogError($"Remove connection: Connection not found. Connection ID: {id}");
            return;
        }

        if (JsonSerializer.Deserialize<Models.Connection>(connectionData) is not { } connection)
        {
            _logger.LogError($"Remove connection: Connection not deserialized. Connection ID: {id}.");
            await _distributedCache.RemoveAsync(id);
            return;
        }

        var userId = connection.UserId;
        var prefixedUserId = $"{PREFIX}{userId}";

        if (await _distributedCache.GetStringAsync(prefixedUserId) is not { } connectionsData)
        {
            _logger.LogWarning($"Remove connection: User connections not found. UserID: {userId}.");
        }
        else if (JsonSerializer.Deserialize<ISet<string>>(connectionsData) is not { } connections)
        {
            _logger.LogError($"Remove connection: User connections not deserialized. UserID: {userId}.");
            await _distributedCache.RemoveAsync(prefixedUserId);
        }
        else
        {
            connections.Remove(connection.Id);
            await _distributedCache.SetStringAsync(prefixedUserId, JsonSerializer.Serialize(connections));
        }

        await _distributedCache.RemoveAsync(id);

        _logger.LogInformation($"Remove connection: Succesfully. Connection: {connection.LogInfo()}.");
    }

    private async Task<string?> GetConnectionStringData(string connectionId)
    {
        _logger.LogInformation($"Get connection string data: Start. Connection ID: {connectionId}.");

        if (await _distributedCache.GetStringAsync(connectionId) is { } connectionString)
        {
            _logger.LogInformation($"Get connection string data: Succesfully. Connection ID: {connectionId}.");
            return connectionString;
        }
        else
        {
            _logger.LogError($"Get connection string data: Connection data is null. Connection ID: {connectionId}.");
            return null;
        }
    }

    private Models.Connection? ConnectionStringDataToConnection(string connectionStringData)
    {
        _logger.LogInformation($"Connection string data to connection: Start.");

        if (JsonSerializer.Deserialize<Models.Connection>(connectionStringData) is { } connection)
        {
            _logger.LogInformation($"Connection string data to connection: Succesfully. Connection: {connection.LogInfo()}.");
            return connection;
        }
        else
        {
            _logger.LogError($"Connection string data to connection: Connection not deserialized.");
            return null;
        }
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
