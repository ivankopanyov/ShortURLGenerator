namespace ShortURLGenerator.TelegramBot.Services.Identity;

/// <summary>
/// Abstract class that describes a Telegram user identification service.
/// Implements the IIdentityService interface.
/// The connection is made using gRPC.
/// </summary>
public abstract class IdentityServiceBase : IIdentityService
{
    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Application configuration.</summary>
    protected IConfiguration AppConfiguration { get; private init; }

    /// <summary>Address of the user identification service.</summary>
    private readonly string _identityServiceHost =
        "http://" + Environment.GetEnvironmentVariable("IDENTITY_SERVICE_HOST")! +
        ":" + Environment.GetEnvironmentVariable("IDENTITY_SERVICE_PORT")!;

    /// <summary>Initialization of the user identification service service object.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public IdentityServiceBase(ILogger<IdentityServiceBase> logger, IConfiguration configuration)
    {
        _logger = logger;
        AppConfiguration = configuration;

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

        _logger.LogStart("Get verification code", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetVerificationCodeAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Get verification code", request, response);
                return response.VerificationCode;
            }

            _logger.LogError("Get verification code", response.Response.Error, request, response);

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Get verification code", ex.Message, request);
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

        _logger.LogStart("Get connections", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetConnectionsAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Get connections", request, response);
                return response.ConnectionsPage;
            }

            _logger.LogError("Get connections", response.Response.ResponseStatus, request, response);

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Get connections", ex.Message, request);
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
            _logger.LogError("Close connection", "Connection ID is null", userId);
            throw new ArgumentNullException(nameof(connectionId));
        }

        var request = new ConnectionRequestDto()
        {
            UserId = userId,
            ConnectionId = connectionId
        };

        _logger.LogStart("Close connection", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.CloseConnectionAsync(request);

            if (response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Close connection", request, response);
                return;
            }

            _logger.LogError("Close connection", response.Error, request, response);

            throw new InvalidOperationException(response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Close connection", ex.Message, request);
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }

    /// <summary>Abstract method for configuring a Telegram user identification service.</summary>
    /// <param name="configuration">Configuration object of the Telegram user identification service.</param>
    protected abstract void OnConfiguring(IdentityServiceConfiguration configuration);
}

