namespace ShortURLGenerator.GrpcHelperTest.Application.IdentityServiceTest;

public class CloseConnectionTest : IdentityServiceTestBase
{
    private static Mock<IIdentityServiceClientFactory> GetIdentityServiceClientFactoryMock(Response response)
    {
        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.CloseConnectionAsync(It.IsAny<ConnectionRequest>(), null, null, default))
            .Returns(new AsyncUnaryCall<Response>(Task.FromResult(response),
                Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        return identityServiceClientFactoryMock;
    }

    [Fact]
    public async Task Connection_id_is_null_failed()
    {
        var userId = Random.Shared.Next(100000000, 999999999);

        var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(new Response
        {
            ResponseStatus = ResponseStatus.Ok
        });

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => identityService.CloseConnectionAsync(userId, null!));
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var userId = Random.Shared.Next(100000000, 999999999);
            var connctionId = string.Empty;

            var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(new Response
            {
                ResponseStatus = status
            });

            var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.CloseConnectionAsync(userId, connctionId));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var userId = Random.Shared.Next(100000000, 999999999);
        var connctionId = string.Empty;

        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.CloseConnectionAsync(It.IsAny<ConnectionRequest>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.CloseConnectionAsync(userId, connctionId));
    }
}

