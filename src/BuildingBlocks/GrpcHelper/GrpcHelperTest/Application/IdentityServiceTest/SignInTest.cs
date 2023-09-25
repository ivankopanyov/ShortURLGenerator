namespace ShortURLGenerator.GrpcHelperTest.Application.IdentityServiceTest;

public class SignInTest : IdentityServiceTestBase
{
    private static Mock<IIdentityServiceClientFactory> GetIdentityServiceClientFactoryMock(TokenResponse response)
    {
        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();

        identityServiceMock
            .Setup(x => x.SignInAsync(It.IsAny<SignInRequest>(), null, null, default))
            .Returns(new AsyncUnaryCall<TokenResponse>(Task.FromResult(response),
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
        var cases = new (string, ConnectionInfo)[]
        {
            (string.Empty, null!),
            (Random.Shared.Next(100000, 999999).ToString(), new ConnectionInfo())
        };

        foreach (var @case in cases)
        {
            var response = new TokenResponse
            {
                Response = new Response
                {
                    ResponseStatus = ResponseStatus.Ok
                },
                Token = new Token
                {
                    AccessToken = Random.Shared.Next(100000, 999999).ToString(),
                    RefreshToken = Random.Shared.Next(100000, 999999).ToString()
                }
            };

            var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(response);

            var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                    identityServiceClientFactoryMock.Object,
                    _loggerMock.Object);

            var result = await identityService.SignInAsync(@case.Item1, @case.Item2);

            Assert.Equal(response.Token.AccessToken, result.AccessToken);
            Assert.Equal(response.Token.RefreshToken, result.RefreshToken);
        }
    }

    [Fact]
    public async Task Verification_code_is_null_failed()
    {
        var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(new TokenResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            Token = new Token
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty
            }
        });

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => identityService.SignInAsync(null!, new ConnectionInfo()));
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var verificationCode = Random.Shared.Next(100000, 999999).ToString();

            var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(new TokenResponse
            {
                Response = new Response
                {
                    ResponseStatus = status
                }
            });

            var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.SignInAsync(verificationCode, new ConnectionInfo()));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var verificationCode = Random.Shared.Next(100000, 999999).ToString();

        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.SignInAsync(It.IsAny<SignInRequest>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.SignInAsync(verificationCode, new ConnectionInfo()));
    }
}

