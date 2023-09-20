﻿namespace ShortURLGenerator.Identity.API.Services.Identity;

/// <summary>class describing the user identification service.
/// Inherited from the class Grpc.Services.IdentityService.IdentityServiceBase,
/// generated from proto file. Uses gRPC.</summary>
public class IdentityService : Grpc.Services.IdentityService.IdentityServiceBase
{
    /// <summary>Verification code repository.</summary>
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    /// <summary>Verification code generation service.</summary>
    private readonly IVerificationCodeGenerationService _verificationCodeGenerationService;

    /// <summary>Connection repository.</summary>
    private readonly IConnectionRepository _connectionRepository;

    /// <summary>Refresh token generation service.</summary>
    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;

    /// <summary>Access token generation service.</summary>
    private readonly IAccessTokenGenerationService _accessTokenGenerationService;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Initializing the user identification service object.</summary>
    /// <param name="verificationCodeRepository">Verification code repository.</param>
    /// <param name="verificationCodeGenerationService">Verification code generation service.</param>
    /// <param name="connectionRepository">Connection repository.</param>
    /// <param name="refreshTokenGenerationService">Refresh token generation service.</param>
    /// <param name="accessTokenGenerationService">Access token generation service.</param>
    /// <param name="logger">Log service.</param>
    public IdentityService(IVerificationCodeRepository verificationCodeRepository,
        IVerificationCodeGenerationService verificationCodeGenerationService,
        IConnectionRepository connectionRepository,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        IAccessTokenGenerationService accessTokenGenerationService,
        ILogger<IdentityService> logger)
    {
        _verificationCodeRepository = verificationCodeRepository;
        _verificationCodeGenerationService = verificationCodeGenerationService;
        _connectionRepository = connectionRepository;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _accessTokenGenerationService = accessTokenGenerationService;
        _logger = logger;
    }

    /// <summary>Method for obtaining a verification code.</summary>
    /// <param name="request">User ID.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Verification code response.</returns>
    public override async Task<VerificationCodeResponse> GetVerificationCode(UserId request, ServerCallContext context)
    {
        _logger.LogInformation($"Get verification code: Start. {request.LogInfo()}.");

        var userId = request.Value.ToString();

        await _verificationCodeRepository.RemoveByUserIdAsync(userId);

        VerificationCode? verificationCode = null;

        do
        {
            try
            {
                var code = _verificationCodeGenerationService.GenerateString();
                verificationCode = await _verificationCodeRepository.CreateAsync(new VerificationCode()
                {
                    Id = code,
                    UserId = userId
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation(ex, $"Get verification code: {ex.Message}. {request.LogInfo()}.");
            }
            catch (Exception ex)
            {
                var errorResponse = new VerificationCodeResponse()
                {
                    Response = new Response()
                    {
                        ResponseStatus = ResponseStatus.BadRequest,
                        Error = "Не удалось получить проверочный код."
                    }
                };

                _logger.LogError(ex, $"Get verification code: {ex.Message}. {errorResponse.LogInfo()}.");

                return errorResponse;
            }
        }
        while (verificationCode is null);

        var okResponse = new VerificationCodeResponse()
        {
            Response = new Response()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            VerificationCode = verificationCode
        };

        _logger.LogInformation($"Get verification code: Successfully. {okResponse.LogInfo()}.");

        return okResponse;
    }

    /// <summary>Method for obtaining a user's active connections.</summary>
    /// <param name="request">Connections request.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Connections page response.</returns>
    public override async Task<ConnectionsPageResponse> GetConnections(ConnectionsRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Get user connections: Start. {request.LogInfo()}.");

        var connectionsPage = await _connectionRepository.GetByUserIdAsync(request.UserId, request.PageInfo.Index, request.PageInfo.Count);

        var response = new ConnectionsPageResponse()
        {
            Response = new Response()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            ConnectionsPage = connectionsPage
        };

        _logger.LogInformation($"Get user connections: Succesfully. {response.LogInfo()}.");

        return response;
    }

    /// <summary>Method for closing an active user connection.</summary>
    /// <param name="request">Connection request.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response.</returns>
    public override async Task<Response> CloseConnection(ConnectionRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Close connection: Start. {request.LogInfo()}.");

        var connectionId = request.ConnectionId;

        if (await _connectionRepository.GetOrDefaultAsync(connectionId) is not { } connection || connection.UserId != request.UserId)
        {
            var errorResponse = new Response()
            {
                ResponseStatus = ResponseStatus.NotFound,
                Error = "Подключение не найдено."
            };

            _logger.LogError($"Close connection: Connection not found. {errorResponse.LogInfo()}.");

            return errorResponse;
        }

        await _connectionRepository.RemoveAsync(connection.Id);

        var okResponse = new Response()
        {
            ResponseStatus = ResponseStatus.Ok
        };

        _logger.LogInformation($"Close connection: Successfully. {okResponse.LogInfo()}.");

        return okResponse;
    }

    /// <summary>Method for creating a connection to a site.</summary>
    /// <param name="request">Request to connect.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Access tokens.</returns>
    public override async Task<TokenResponse> SignIn(SignInRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Sign in: Start. {request.LogInfo()}.");

        var response = new TokenResponse()
        {
            Response = new Response()
            {
                ResponseStatus = ResponseStatus.NotFound
            }
        };

        if (await _verificationCodeRepository.GetUserIdAsync(request.VerificationCode) is not { } userIdString)
        {
            response.Response.Error = "Проверочный код не действителен.";
            _logger.LogError($"Sign in: Verification code not found. {response.LogInfo()}.");
            return response;
        }

        await _verificationCodeRepository.RemoveByUserIdAsync(userIdString);

        if (!long.TryParse(userIdString, out long userId))
        {
            response.Response.Error = "Проверочный код не действителен.";
            _logger.LogError($"Sign in: Verification code not found. {response.LogInfo()}.");
            return response;
        }

        try
        {
            var token = await CreateConnection(userId, request.ConnectionInfo);
            response.Response.ResponseStatus = ResponseStatus.Ok;
            response.Token = token;

            _logger.LogInformation($"Sign in: Successfully. {response.LogInfo()}.");

            return response;
        }
        catch (InvalidOperationException ex)
        {
            response.Response.ResponseStatus = ResponseStatus.Conflict;
            response.Response.Error = ex.Message;

            _logger.LogError(ex, $"Sign in: {ex.Message}. {response.LogInfo()}");

            return response;
        }
    }

    /// <summary>Method for creating a new connection and deleting an old connection.</summary>
    /// <param name="request">Request to connect.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Access tokens.</returns>
    public override async Task<TokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        _logger.LogInformation($"Refresh token: Start. {request.LogInfo()}");

        var response = new TokenResponse()
        {
            Response = new Response()
            {
                ResponseStatus = ResponseStatus.NotFound
            }
        };

        if (await _connectionRepository.GetOrDefaultAsync(request.RefreshToken) is not { } connection)
        {
            response.Response.Error = "Подключение не найдено.";
            _logger.LogError($"Refresh token: Connection not found. {request.LogInfo()}");
            return response;
        }

        if (connection.UserId != request.UserId)
        {
            response.Response.Error = "Подключение не найдено.";
            _logger.LogError($"Refresh token: The connection belongs to another user. {connection.LogInfo()}, {request.LogInfo()}");
            return response;
        }

        await _connectionRepository.RemoveAsync(request.RefreshToken);

        try
        {
            var token = await CreateConnection(request.UserId, request.ConnectionInfo);
            response.Response.ResponseStatus = ResponseStatus.Ok;
            response.Token = token;

            _logger.LogInformation($"Refresh token: Successfully. {response.LogInfo()}.");

            return response;
        }
        catch (InvalidOperationException ex)
        {
            response.Response.ResponseStatus = ResponseStatus.Conflict;
            response.Response.Error = ex.Message;

            _logger.LogError(ex, $"Refresh token: {ex.Message}. {response.LogInfo()}");

            return response;
        }
    }

    /// <summary>Method for creating a connection to a site.</summary>
    /// <param name="userId">User ID.</param>
    /// <param name="connectionInfo">Connection info.</param>
    /// <returns>Access token.</returns>
    /// <exception cref="InvalidOperationException">
    /// Exception throw if the maximum number of connections has been reached.
    /// </exception>
    private async Task<Token> CreateConnection(long userId, Grpc.Services.ConnectionInfo connectionInfo)
    {
        _logger.LogInformation($"Create connection: Start. User ID: {userId}, {connectionInfo.LogInfo()}");

        var connection = new Connection()
        {
            UserId = userId,
            ConnectionInfo = connectionInfo
        };

        string? refreshToken = null;

        while (refreshToken is null)
        {
            refreshToken = _refreshTokenGenerationService.GenerateString();

            try
            {
                connection.Id = refreshToken;
                await _connectionRepository.CreateAsync(connection);
            }
            catch (InvalidOperationException ex)
            {
                refreshToken = null;
                _logger.LogInformation(ex, $"Create connection: {ex.Message}. {connection.LogInfo()}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, $"Create connection: {ex.Message}.");
                throw new InvalidOperationException("Достигнуто максимальное колличество подключений.");
            }
        }

        var token = new Token()
        {
            AccessToken = _accessTokenGenerationService.CreateToken(userId),
            RefreshToken = refreshToken
        };

        _logger.LogInformation($"Create connection: Successfully. {token.LogInfo()}");

        return token;
    }
}
