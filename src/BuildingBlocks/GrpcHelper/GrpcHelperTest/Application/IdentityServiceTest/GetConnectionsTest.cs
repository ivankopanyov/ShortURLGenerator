namespace ShortURLGenerator.GrpcHelperTest.Application.IdentityServiceTest;

public class GetConnectionsTest : IdentityServiceTestBase
{
    private static Mock<IIdentityServiceClientFactory> GetIdentityServiceClientFactoryMock(ConnectionsPageResponse response)
    {
        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.GetConnectionsAsync(It.IsAny<ConnectionsRequest>(), null, null, default))
            .Returns(new AsyncUnaryCall<ConnectionsPageResponse>(Task.FromResult(response),
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
        var index = Random.Shared.Next(-10000, 10000);
        var size = Random.Shared.Next(-10000, 10000);

        var response = new ConnectionsPageResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            ConnectionsPage = new ConnectionsPage
            {
                PageInfo = new PageInfo
                {
                    Index = index,
                    Count = size
                }
            }
        };

        var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(response);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        var result = await identityService.GetConnectionsAsync(userId, index, size);

        Assert.Equal(response.ConnectionsPage.PageInfo.Index, result.PageInfo.Index);
        Assert.Equal(response.ConnectionsPage.PageInfo.Count, result.PageInfo.Count);
        Assert.NotNull(response.ConnectionsPage.Connections);
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var userId = Random.Shared.Next(100000000, 999999999);
            var index = Random.Shared.Next(-10000, 10000);
            var size = Random.Shared.Next(-10000, 10000);

            var identityServiceClientFactoryMock = GetIdentityServiceClientFactoryMock(new ConnectionsPageResponse
            {
                Response = new Response
                {
                    ResponseStatus = status
                }
            });

            var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.GetConnectionsAsync(userId, index, size));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var userId = Random.Shared.Next(100000000, 999999999);
        var index = Random.Shared.Next(-10000, 10000);
        var size = Random.Shared.Next(-10000, 10000);

        var identityServiceMock = new Mock<IdentityService.IdentityServiceClient>();
        identityServiceMock
            .Setup(x => x.GetConnectionsAsync(It.IsAny<ConnectionsRequest>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var identityServiceClientFactoryMock = new Mock<IIdentityServiceClientFactory>();
        identityServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(identityServiceMock.Object);

        var identityService = new GrpcHelper.Services.Identity.IdentityService(_grpcChannelFactoryMock.Object,
                identityServiceClientFactoryMock.Object,
                _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => identityService.GetConnectionsAsync(userId, index, size));
    }
}

