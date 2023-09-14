namespace ShortURLGenerator.Identity.API.Services.Identity;

public class IdentityService : Grpc.Services.IdentityService.IdentityServiceBase
{
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    private readonly IVerificationCodeGenerationService _verificationCodeGenerationService;

    private readonly IConnectionRepository _connectionRepository;

    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;

    private readonly ILogger _logger;

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

    public async override Task<VerificationCodeResponseDto> GetVerificationCode(UserIdDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Get verification code: Start. User ID: {request.LogInfo()}.");

        var userId = request.UserId.ToString();

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
                _logger.LogInformation(ex, $"Get verification code: {ex.Message}. User ID: {request.LogInfo()}.");
            }
            catch (Exception ex)
            {
                var errorResponse = new VerificationCodeResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = ResponseStatus.BadRequest,
                        Error = "Не удалось получить проверочный код."
                    }
                };

                _logger.LogError(ex, $"Get verification code: {ex.Message}. Verification code response: {errorResponse.LogInfo()}.");

                return errorResponse;
            }
        }
        while (verificationCode is null);

        var okResponse = new VerificationCodeResponseDto()
        {
            Response = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            VerificationCode = new VerificationCodeDto()
            {
                Code = verificationCode.Id,
                LifeTimeMinutes = verificationCode.LifeTime.Minutes
            }
        };

        _logger.LogInformation($"Get verification code: Successfully. Verification code response: {okResponse.LogInfo()}.");

        return okResponse;
    }

    public async override Task<ConnectionsPageResponseDto> GetConnections(ConnectionsRequestDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Get user connections: Start. Connections request: {request.LogInfo()}.");

        var userId = request.UserId.ToString();

        var connectionsPage = await _connectionRepository.GetByUserIdAsync(userId, request.PageInfo.Index, request.PageInfo.Count);
        var response = new ConnectionsPageResponseDto()
        {
            Response = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            ConnectionsPage = connectionsPage
        };

        _logger.LogInformation($"Get user connections: Succesfully. Connections page response: {response.LogInfo()}.");

        return response;
    }

    public async override Task<ResponseDto> CloseConnection(ConnectionRequestDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Close connection: Start. Connection request: {request.LogInfo()}.");

        var userId = request.UserId.ToString();
        var connectionId = request.ConnectionId;

        if (await _connectionRepository.GetOrDefaultAsync(connectionId) is not { } connection || connection.UserId != userId)
        {
            var errorResponse = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.NotFound,
                Error = "Подключение не найдено."
            };

            _logger.LogError($"Close connection: Connection not found. Response: ${errorResponse.LogInfo()}.");

            return errorResponse;
        }

        await _connectionRepository.RemoveAsync(connection.Id);

        var okResponse = new ResponseDto()
        {
            ResponseStatus = ResponseStatus.Ok
        };

        _logger.LogInformation($"Close connection: Successfully. Response: ${okResponse.LogInfo()}.");

        return okResponse;
    }
}
