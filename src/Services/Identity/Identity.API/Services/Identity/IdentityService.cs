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
        var methodName = nameof(GetVerificationCode);

        _logger.LogStart(methodName, request.LogInfo());

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
                _logger.LogInformation(methodName, ex.Message, request.LogInfo());
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

                _logger.LogError(methodName, ex.Message, errorResponse.LogInfo());

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

        _logger.LogSuccessfully(methodName, okResponse.LogInfo());

        return okResponse;
    }

    public async override Task<ConnectionsPageResponseDto> GetConnections(ConnectionsRequestDto request, ServerCallContext context)
    {
        var methodName = nameof(GetConnections);
        var userId = request.UserId.ToString();

        _logger.LogStart(methodName, request.LogInfo());

        var connectionsPage = await _connectionRepository.GetByUserIdAsync(userId, request.PageInfo.Index, request.PageInfo.Count);
        var response = new ConnectionsPageResponseDto()
        {
            Response = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            ConnectionsPage = connectionsPage
        };

        _logger.LogSuccessfully(methodName, response.LogInfo());

        return response;
    }

    public async override Task<ResponseDto> CloseConnection(ConnectionRequestDto request, ServerCallContext context)
    {
        var methodName = nameof(CloseConnection);

        _logger.LogStart(methodName, request.LogInfo());

        var userId = request.UserId.ToString();
        var connectionId = request.ConnectionId;

        if (await _connectionRepository.GetOrDefaultAsync(connectionId) is not { } connection || connection.UserId != userId)
        {
            var errorResponse = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.NotFound,
                Error = "Подключение не найдено."
            };

            _logger.LogWarning(methodName, "Connection not found", errorResponse.LogInfo());

            return errorResponse;
        }

        await _connectionRepository.RemoveAsync(connection.Id);

        var okResponse = new ResponseDto()
        {
            ResponseStatus = ResponseStatus.Ok
        };

        _logger.LogSuccessfully(methodName, okResponse.LogInfo());

        return okResponse;
    }
}
