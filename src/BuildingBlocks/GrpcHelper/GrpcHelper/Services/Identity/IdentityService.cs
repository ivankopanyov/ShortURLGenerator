namespace ShortURLGenerator.GrpcHelper.Services.Identity;

/// <summary>
/// Class that describes a Telegram user identification service.
/// The connection is made using gRPC.
/// </summary>
public class IdentityService : IConnectionService, IIdentityService
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
    public async Task<VerificationCode> GetVerificationCodeAsync(long userId)
    {
        var request = new UserId()
        {
            Value = userId
        };

        _logger.LogInformation($"Get verification code: Start. {request}.");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetVerificationCodeAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get verification code: Successfully. {response.LogInfo()}.");
                return response.VerificationCode;
            }

            _logger.LogError($"Get verification code: {response.Response.Error}. {response.LogInfo()}.");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get verification code: {ex.Message}.");
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
    public async Task<ConnectionsPage> GetConnectionsAsync(long userId, int index, int size)
    {
        var request = new ConnectionsRequest()
        {
            UserId = userId,
            PageInfo = new PageInfo()
            {
                Index = index,
                Count = size
            }
        };

        _logger.LogInformation($"Get connections: Start. {request.LogInfo()}.");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.GetConnectionsAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get connections: Successfully. {response.LogInfo()}.");
                return response.ConnectionsPage;
            }

            _logger.LogError($"Get connections: {response.Response.Error}. {response.LogInfo()}.");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get connections: {ex.Message}.");
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
            _logger.LogError($"Close connection: Connection ID is null. User ID: {userId}");
            throw new ArgumentNullException(nameof(connectionId));
        }

        var request = new ConnectionRequest()
        {
            UserId = userId,
            ConnectionId = connectionId
        };

        _logger.LogInformation($"Close connection: Start. {request.LogInfo()}.");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.CloseConnectionAsync(request);

            if (response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Close connection: Successfully. {response.LogInfo()}.");
                return;
            }

            _logger.LogInformation($"Close connection: {response.Error}. {response.LogInfo()}.");

            throw new InvalidOperationException(response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogInformation(ex, $"Close connection: {ex.Message}.");
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }

    /// <summary>Method for creating a new connection to a site.</summary>
    /// <param name="verificationCode">Verification code.</param>
    /// <param name="connectionInfo">Connection info.</param>
    /// <returns>Access tokens.</returns>
    /// <exception cref="ArgumentNullException">Verification сode is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<Token> SignInAsync(string verificationCode, ConnectionInfo connectionInfo)
    {
        if (verificationCode is null)
        {
            _logger.LogError($"Sign in: Verification code is null. Verification code: {verificationCode}");
            throw new ArgumentNullException(nameof(verificationCode));
        }

        var request = new SignInRequest()
        {
            VerificationCode = verificationCode,
            ConnectionInfo = connectionInfo ?? new ConnectionInfo()
        };

        _logger.LogInformation($"Sign in: Start. {request.LogInfo()}.");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.SignInAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Sign in: Successfully. {response.LogInfo()}.");
                return response.Token;
            }

            _logger.LogError($"Sign in: {response.Response.Error}. {response.LogInfo()}.");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogInformation(ex, $"Sign in: {ex.Message}.");
            throw new InvalidOperationException("Нет связи с сервисом идентификации пользователей.");
        }
    }

    /// <summary>Method for creating a new connection and deleting an old connection.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="refreshToken">Refresh token or connection ID.</param>
    /// <param name="connectionInfo">Connection info.</param>
    /// <returns>Access tokens.</returns>
    /// <exception cref="ArgumentNullException">Refresh token is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<Token> RefreshTokenAsync(long userId, string refreshToken, ConnectionInfo connectionInfo)
    {
        if (refreshToken is null)
        {
            _logger.LogError($"Sign in: Refresh token is null. Refresh token: {refreshToken}");
            throw new ArgumentNullException(nameof(refreshToken));
        }

        var request = new RefreshTokenRequest()
        {
            UserId = userId,
            RefreshToken = refreshToken,
            ConnectionInfo = connectionInfo ?? new ConnectionInfo()
        };

        _logger.LogInformation($"Refresh token: Start. {request.LogInfo()}.");

        try
        {
            using var channel = GrpcChannel.ForAddress(_identityServiceHost);
            var client = new Grpc.Services.IdentityService.IdentityServiceClient(channel);
            var response = await client.RefreshTokenAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Refresh token: Successfully. {response.LogInfo()}.");
                return response.Token;
            }

            _logger.LogError($"Refresh token: {response.Response.Error}. {response.LogInfo()}.");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogInformation(ex, $"Refresh token: {ex.Message}.");
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

