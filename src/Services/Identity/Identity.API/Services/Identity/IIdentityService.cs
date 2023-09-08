namespace ShortURLGenerator.Identity.API.Services.Identity;

public interface IIdentityService
{
    Task<VerificationCodeResponseDto> GetVerificationCode(UserIdDto request, ServerCallContext context);

    Task<ConnectionsPageResponseDto> GetConnections(ConnectionsRequestDto request, ServerCallContext context);

    Task<ResponseDto> CloseConnection(ConnectionRequestDto request, ServerCallContext context);
}
