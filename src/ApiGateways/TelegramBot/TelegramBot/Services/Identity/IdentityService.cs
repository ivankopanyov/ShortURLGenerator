namespace ShortURLGenerator.TelegramBot.Services.Identity;

/// <summary>Telegram user identification service. The connection is made using gRPC.</summary>
public class IdentityService : IIdentityService
{
    /// <summary>Address of the user identification service.</summary>
    private static readonly string _identityServiceHost =
        "http://" + Environment.GetEnvironmentVariable("IDENTITY_SERVICE_HOST")! +
        ":" + Environment.GetEnvironmentVariable("IDENTITY_SERVICE_PORT")!;

    /// <summary>Log service.</summary>
    private readonly ILogger<UrlService> _logger;

    /// <summary>Initialization of the user identification service service object.</summary>
    /// <param name="logger">Log service.</param>
    public IdentityService(ILogger<UrlService> logger)
    {
        _logger = logger;
    }

    /// <summary>Method for requesting a verification code. The connection is made using gRPC.</summary>
    /// <param name="userId">User ID.</param>
    /// <returns>Verification code.</returns>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<VerificationCodeDto> GetVerificationCodeAsync(long userId)
    {
        _logger.LogInformation($"Get verification code.\n\tUser ID: {userId}");

        var request = new UserIdDto()
        {
            UserId = userId
        };

        _logger.LogInformation($"Get verification code: Connection to IdentityService: {_identityServiceHost}\n\tUser ID: {userId}");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetVerificationCodeAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get verification code: succesful.\n\tUser ID: {userId}\n\tCode: {response.VerificationCode.Code}\n\tLifetime minutes: {response.VerificationCode.LifeTimeMinutes}");
                return response.VerificationCode;
            }

            _logger.LogError($"Get verification code: failed.\n\tUser ID: {userId}\n\tError: {response.Response.ResponseStatus} {response.Response.Error}");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get verification code: failed.\n\tUser ID: {userId}\n\tError: No connection with IdentityService {_identityServiceHost}");
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
        _logger.LogInformation($"Get connections.\n\tUser ID: {userId}\n\tIndex: {index}\n\tSize: {size}");

        var request = new ConnectionsRequestDto()
        {
            UserId = userId,
            PageInfo = new PageInfoDto()
            {
                Index = index,
                Count = size
            }
        };

        _logger.LogInformation($"Get connections: Connection to IdentityService: {_identityServiceHost}\n\tUser ID: {userId}\n\tIndex: {index}\n\tSize: {size}");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetConnectionsAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get connections: succesful.\n\tUser ID: {userId}\n\tIndex: {index}\n\tSize: {size}\n\tResponse index: {response.ConnectionsPage.PageInfo.Index}\n\tResponse size: {response.ConnectionsPage.PageInfo.Count}");
                return response.ConnectionsPage;
            }

            _logger.LogError($"Get connections: failed.\n\tIndex: {index}\n\tSize: {size}\n\tError: {response.Response.ResponseStatus} {response.Response.Error}");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get connections: failed.\n\tIndex: {index}\n\tSize: {size}\n\tError: No connection with IdentityService {_identityServiceHost}");
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
            throw new ArgumentNullException(nameof(connectionId));

        _logger.LogInformation($"Close connection.\n\tUser ID: {userId}\n\tConnection ID: {connectionId}");

        var request = new ConnectionRequestDto()
        {
            UserId = userId,
            ConnectionId = connectionId
        };

        _logger.LogInformation($"Close connection: Connection to IdentityService: {_identityServiceHost}\n\tUser ID: {userId}\n\tConnection ID: {connectionId}");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.CloseConnectionAsync(request);

            if (response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Close connection: succesful.\n\tUser ID: {userId}\n\tConnection ID: {connectionId}");
                return;
            }

            _logger.LogError($"Close connection: failed.\n\tUser ID: {userId}\n\tConnection ID: {connectionId}\n\tError: {response.ResponseStatus} {response.Error}");

            throw new InvalidOperationException(response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Close connection: failed.\n\tUser ID: {userId}\n\tConnection ID: {connectionId}\n\tError: No connection with IdentityService {_identityServiceHost}");
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }
}

