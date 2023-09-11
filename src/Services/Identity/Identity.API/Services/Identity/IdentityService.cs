namespace ShortURLGenerator.Identity.API.Services.Identity;

public class IdentityService : Grpc.Services.IdentityService.IdentityServiceBase, IIdentityService
{
    private readonly IVerificationCodeGenerationService _verificationCodeGenerationService;

    private readonly IRefreshTokenGenerationService _refreshTokenGenerationService;

    private readonly IVerificationCodeRepository _verificationCodeRepository;

    private readonly IConnectionRepository _connectionRepository;

    private readonly ILogger _logger;

    public IdentityService(IVerificationCodeGenerationService verificationCodeGenerationService,
        IRefreshTokenGenerationService refreshTokenGenerationService,
        IVerificationCodeRepository verificationCodeRepository,
        IConnectionRepository connectionRepository,
        ILogger<IdentityService> logger)
    {
        _verificationCodeGenerationService = verificationCodeGenerationService;
        _refreshTokenGenerationService = refreshTokenGenerationService;
        _verificationCodeRepository = verificationCodeRepository;
        _connectionRepository = connectionRepository;
        _logger = logger;
    }

    public override async Task<VerificationCodeResponseDto> GetVerificationCode(UserIdDto request, ServerCallContext context)
    {
        var userId = request.UserId.ToString();

        _logger.LogStart("Get verification code", userId);

        VerificationCodeDto? verificationCode = null;

        while (verificationCode == null)
        {
            try
            {
                var code = _verificationCodeGenerationService.GenerateString();
                var lifeTime = await _verificationCodeRepository.CreateOrUpdateAsync(userId, code);
                verificationCode = new VerificationCodeDto()
                {
                    Code = code,
                    LifeTimeMinutes = lifeTime.Minutes
                };
            }
            catch (DuplicateWaitObjectException)
            {
                _logger.LogInformation("Get verification code", "Duplicate", userId);
            }
        }

        _logger.LogSuccessfully("Get verification code", userId);

        return new VerificationCodeResponseDto()
        {
            Response = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            VerificationCode = verificationCode
        };
    }

    public override async Task<ConnectionsPageResponseDto> GetConnections(ConnectionsRequestDto request, ServerCallContext context)
    {
        var userId = request.UserId.ToString();

        _logger.LogStart("Get connections", userId);

        try
        {
            var response = await _connectionRepository.GetAsync(userId, request.PageInfo.Index, request.PageInfo.Count);

            _logger.LogSuccessfully("Get connections", userId);

            return new ConnectionsPageResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.Ok
                },
                ConnectionsPage = response
            };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Get connections", ex.Message, userId);

            return new ConnectionsPageResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.BadRequest,
                    Error = "Не удалось получить активные подключения пользователя."
                }
            };
        }
    }

    public override async Task<ResponseDto> CloseConnection(ConnectionRequestDto request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}
