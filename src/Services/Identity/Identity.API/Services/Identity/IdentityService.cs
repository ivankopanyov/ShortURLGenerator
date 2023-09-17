namespace ShortURLGenerator.Identity.API.Services.Identity;

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

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Initializing the user identification service object.</summary>
    /// <param name="verificationCodeRepository">Verification code repository.</param>
    /// <param name="verificationCodeGenerationService">Verification code generation service.</param>
    /// <param name="connectionRepository">Connection repository.</param>
    /// <param name="refreshTokenGenerationService">Refresh token generation service.</param>
    /// <param name="logger">Log service.</param>
    public IdentityService(IVerificationCodeRepository verificationCodeRepository,
        IVerificationCodeGenerationService verificationCodeGenerationService,
        IConnectionRepository connectionRepository,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        ILogger<IdentityService> logger)
    {
        _verificationCodeRepository = verificationCodeRepository;
        _verificationCodeGenerationService = verificationCodeGenerationService;
        _connectionRepository = connectionRepository;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _logger = logger;
    }

    /// <summary>Method for obtaining a verification code.</summary>
    /// <param name="request">User ID.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Verification code response.</returns>
    public async override Task<VerificationCodeResponse> GetVerificationCode(UserId request, ServerCallContext context)
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
    public async override Task<ConnectionsPageResponse> GetConnections(ConnectionsRequest request, ServerCallContext context)
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
    public async override Task<Response> CloseConnection(ConnectionRequest request, ServerCallContext context)
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
}
