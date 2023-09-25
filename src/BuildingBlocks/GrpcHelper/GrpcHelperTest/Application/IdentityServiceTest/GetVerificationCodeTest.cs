namespace ShortURLGenerator.GrpcHelperTest.Application.IdentityServiceTest;

public class GetVerificationCodeTest : IdentityServiceTestBase
{
    private static Mock<IIdentityServiceClientFactory> GetIdentityServiceClientFactoryMock(VerificationCodeResponse response)
    {
        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.GetVerificationCodeAsync(It.IsAny<UserId>(), null, null, default))
            .Returns(new AsyncUnaryCall<VerificationCodeResponse>(Task.FromResult(response),
                Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        return identityServiceClientFactoryMock;
    }

    [Fact]
    public async Task Success()
    {
        var userId = Random.Shared.Next(100000000, 999999999);

        var response = new VerificationCodeResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            VerificationCode = new VerificationCode
            {
                Id = Random.Shared.Next(100000, 999999).ToString(),
                UserId = userId.ToString(),
                LifeTimeMinutes = 1
            }
        };

        var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(response);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
            identityServiceClientFactoryMock.Object,
            _loggerMock.Object);

        var result = await identityService.GetVerificationCodeAsync(userId);

        Assert.Equal(response.VerificationCode.Id, result.Id);
        Assert.Equal(response.VerificationCode.UserId, result.UserId);
        Assert.Equal(response.VerificationCode.LifeTimeMinutes, result.LifeTimeMinutes);
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var userId = Random.Shared.Next(100000000, 999999999);

            var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(new VerificationCodeResponse
            {
                Response = new Response
                {
                    ResponseStatus = status
                }
            });

            var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.GetVerificationCodeAsync(userId));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var userId = Random.Shared.Next(100000000, 999999999);

        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.GetVerificationCodeAsync(It.IsAny<UserId>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.GetVerificationCodeAsync(userId));
    }
}

