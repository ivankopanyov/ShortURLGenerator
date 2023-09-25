namespace ShortURLGenerator.GrpcHelperTest.Application.IdentityServiceTest;

public class RefreshTokenTest : IdentityServiceTestBase
{
    private static Mock<IIdentityServiceClientFactory> GetIdentityServiceClientFactoryMock(TokenResponse response)
    {
        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();

        identityServiceMock
            .Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>(), null, null, default))
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
        var cases = new (long, string, ConnectionInfo)[]
        {
            (Random.Shared.Next(-999999, 0), string.Empty, null!),
            (0, Random.Shared.Next(-999999, 999999).ToString(), new ConnectionInfo()),
            (Random.Shared.Next(0, 999999), Random.Shared.Next(-999999, 999999).ToString(), new ConnectionInfo()),
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

            var result = await identityService.RefreshTokenAsync(@case.Item1, @case.Item2, @case.Item3);

            Assert.Equal(response.Token.AccessToken, result.AccessToken);
            Assert.Equal(response.Token.RefreshToken, result.RefreshToken);
        }
    }

    [Fact]
    public async Task Refresh_token_is_null_failed()
    {
        var userId = Random.Shared.Next(100000000, 999999999);

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

        await Assert.ThrowsAsync<ArgumentNullException>(() => identityService.RefreshTokenAsync(userId, null!, new ConnectionInfo()));
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var userId = Random.Shared.Next(100000000, 999999999);
            var refreshToken = string.Empty;

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

            await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.RefreshTokenAsync(userId, refreshToken, new ConnectionInfo()));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var userId = Random.Shared.Next(100000000, 999999999);
        var refreshToken = string.Empty;

        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenRequest>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.RefreshTokenAsync(userId, refreshToken, new ConnectionInfo()));
    }
}

