namespace ShortURLGenerator.TelegramBot.Services.Identity;

/// <summary>
/// Class that describes a Telegram user identification service.
/// Implements the IIdentityService interface.
/// The connection is made using gRPC.
/// </summary>
public class IdentityService : IIdentityService
{
    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Address of the user identification service.</summary>
    private readonly string _identityServiceHost;

    /// <summary>Initialization of the user identification service service object.</summary>
    /// <param name="logger">Log service.</param>
    public IdentityService(ILogger<IdentityService> logger)
    {
        _logger = logger;

        var identityServiceConfiguration = new IdentityServiceConfiguration();
        OnConfiguring(identityServiceConfiguration);

        _identityServiceHost = identityServiceConfiguration.IdentityServiceHost;
    }

    /// <summary>Method for requesting a verification code. The connection is made using gRPC.</summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Verification code.</returns>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<VerificationCodeDto> GetVerificationCodeAsync(long userId)
    {
        _logger.LogInformation($"Get verification code: Start. User ID: {userId}.");

        var request = new UserIdDto()
        {
            UserId = userId
        };

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetVerificationCodeAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get verification code: Successfully. Request: {request.LogInfo()}, Response: {response.LogInfo()}.");
                return response.VerificationCode;
            }

            _logger.LogError($"Get verification code: Error. Request: {request.LogInfo()}, Response: {response.LogInfo()}.");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get verification code: {ex.Message}. Request: {request.LogInfo()}.");
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }

    /// <summary>Method for requesting active connections on the site. The connection is made using gRPC.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="index">The index of the requested connection page.</param>
    /// <param name="size">The number of connections per page.</param>
    /// <returns>Page with a list of connections and information about yourself.</returns>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<ConnectionsPageDto> GetConnectionsAsync(long userId, int index, int size)
    {
        _logger.LogInformation($"Get connections: Start. User ID: {userId}, Index: {index}, Size: {size}.");

        var request = new ConnectionsRequestDto()
        {
            UserId = userId,
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = size
            }
        };

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetConnectionsAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get connections: Successfully. Request: {request.LogInfo()}, Response: {response.LogInfo()}.");
                return response.ConnectionsPage;
            }

            _logger.LogError($"Get connections: Error. Request: {request.LogInfo()}, Response: {response.LogInfo()}.");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get connections: Error. Request: {request.LogInfo()}.");
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }

    /// <summary>The method to close the user's connection to the site. The connection is made using gRPC.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="connectionId">Connection ID.</param>
    /// <exception cref="ArgumentNullException">Connection ID is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task CloseConnectionAsync(long userId, string connectionId)
    {
        _logger.LogInformation($"Close connection: Start. User ID: {userId}, Connection ID: {connectionId}.");

        if (connectionId is null)
        {
            _logger.LogError($"Close connection: Connection ID is null. User ID: {userId}, Connection ID: {connectionId}.");
            throw new ArgumentNullException(nameof(connectionId));
        }

        var request = new ConnectionRequestDto()
        {
            UserId = userId,
            ConnectionId = connectionId
        };

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.CloseConnectionAsync(request);

            if (response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Close connection: Successfully. Request: {request.LogInfo()}, Response: {response.LogInfo()}.");
                return;
            }

            _logger.LogInformation($"Close connection: Error. Request: {request.LogInfo()}, Response: {response.LogInfo()}.");

            throw new InvalidOperationException(response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogInformation(ex, $"Close connection: {ex.Message}. Request: {request.LogInfo()}.");
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }

    /// <summary>Virtual method for configuring a Telegram user identification service.</summary>
    /// <param name="configuration">Configuration object of the Telegram user identification service.</param>
    protected virtual void OnConfiguring(IdentityServiceConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("IDENTITY_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable("IDENTITY_SERVICE_PORT");
        configuration.IdentityServiceHost = $"http://{host}:{port}";
    }
}

