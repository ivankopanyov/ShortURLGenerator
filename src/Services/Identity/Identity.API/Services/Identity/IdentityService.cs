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
        string code;

        do
        {
            code = _verificationCodeGenerationService.GenerateString();
        }
        while (!await _verificationCodeRepository.Contains(code));

        var verificationCode = await _verificationCodeRepository.CreateAsync(new Models.VerificationCode()
        {
            Id = code,
            UserId = userId
        });

        return verificationCode is not null
            ? new VerificationCodeResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.Ok
                },
                VerificationCode = new VerificationCodeDto()
                { 
                    Code = code,
                    LifeTimeMinutes = verificationCode.LifeTime.Minutes
                }
            }
            : new VerificationCodeResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.BadRequest,
                    Error = "Не удалось получить проверочный код."
                }
            };
    }

    public override async Task<ConnectionsPageResponseDto> GetConnections(ConnectionsRequestDto request, ServerCallContext context)
    {
        var userId = request.UserId.ToString();
        var response = await _connectionRepository.GetAsync(userId, request.PageInfo.Index, request.PageInfo.Count);

        return new ConnectionsPageResponseDto()
        {
            Response = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            ConnectionsPage = response
        };
    }

    public override async Task<ResponseDto> CloseConnection(ConnectionRequestDto request, ServerCallContext context)
    {
        var userId = request.UserId.ToString();
        var connectionId = request.ConnectionId;

        if (await _connectionRepository.GetAsync(connectionId) is { } connection && connection.UserId == userId)
        {
            await _connectionRepository.RemoveAsync(connectionId);
            return new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            };
        }

        return new ResponseDto()
        {
            ResponseStatus = ResponseStatus.BadRequest,
            Error = "Подключение не найдено."
        };
    }
}
