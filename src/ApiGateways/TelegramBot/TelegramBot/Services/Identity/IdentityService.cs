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
        var request = new UserIdDto()
        {
            UserId = userId
        };

        var requestId = request.UserId.ToString();

        _logger.LogStart("Get verification code", requestId);
        _logger.LogObject("Get verification code", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetVerificationCodeAsync(request);

            _logger.LogObject("Get verification code", response);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Get verification code", requestId);
                return response.VerificationCode;
            }

            _logger.LogError("Get verification code", response.Response.Error, requestId);

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Get verification code", ex.Message, requestId);
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
        var request = new ConnectionsRequestDto()
        {
            UserId = userId,
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = size
            }
        };

        var requestId = request.UserId.ToString();

        _logger.LogStart("Get connections", requestId);
        _logger.LogObject("Get connections", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetConnectionsAsync(request);

            _logger.LogObject("Get connections", response);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Get connections", requestId);
                return response.ConnectionsPage;
            }

            _logger.LogError("Get connections", response.Response.Error, requestId);

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Get connections", ex.Message, requestId);
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
        if (connectionId is null)
        {
            _logger.LogError("Close connection", "Connection ID is null");
            throw new ArgumentNullException(nameof(connectionId));
        }

        var request = new ConnectionRequestDto()
        {
            UserId = userId,
            ConnectionId = connectionId
        };

        var requestId = request.UserId.ToString();

        _logger.LogStart("Close connection", requestId);
        _logger.LogObject("Close connection", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.CloseConnectionAsync(request);

            _logger.LogObject("Close connection", response);

            if (response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Close connection", requestId);
                return;
            }

            _logger.LogError("Close connection", response.Error, requestId);

            throw new InvalidOperationException(response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Close connection", ex.Message, requestId);
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

