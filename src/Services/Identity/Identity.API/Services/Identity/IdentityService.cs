namespace ShortURLGenerator.Identity.API.Services.Identity;

public class IdentityService : Grpc.Services.IdentityService.IdentityServiceBase, IIdentityService
{
    public override async Task<VerificationCodeResponseDto> GetVerificationCode(UserIdDto request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override async Task<ConnectionsPageResponseDto> GetConnections(ConnectionsRequestDto request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override async Task<ResponseDto> CloseConnection(ConnectionRequestDto request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}
