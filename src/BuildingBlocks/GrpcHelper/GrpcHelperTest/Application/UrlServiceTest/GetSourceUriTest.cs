namespace ShortURLGenerator.GrpcHelperTest.Application.UrlServiceTest;

public class GetSourceUriTest : UrlServiceTestBase
{
    private static Mock<IUrlServiceClientFactory> GetUrlServiceClientFactoryMock(UriResponse response)
    {
        var urlServiceMock = new Mock<UrlService.UrlServiceClient>();
        urlServiceMock
            .Setup(x => x.GetAsync(It.IsAny<Url>(), null, null, default))
            .Returns(new AsyncUnaryCall<UriResponse>(Task.FromResult(response),
                Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

        var urlServiceClientFactoryMock = new Mock<IUrlServiceClientFactory>();
        urlServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(urlServiceMock.Object);

        return urlServiceClientFactoryMock;
    }

    [Fact]
    public async Task Success()
    {
        var response = new UriResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            Uri = "https://example.com/example"
        };

        var urlServiceClientFactoryMock = GetUrlServiceClientFactoryMock(response);

        var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

        var result = await urlService.GetSourceUriAsync("000000");

        Assert.Equal(response.Uri, result);
    }

    [Fact]
    public async Task Url_is_null_failed()
    {
        var urlServiceClientFactoryMock = GetUrlServiceClientFactoryMock(new UriResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            Uri = string.Empty
        });

        var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => urlService.GetSourceUriAsync(null!));
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var urlServiceClientFactoryMock = GetUrlServiceClientFactoryMock(new UriResponse
            {
                Response = new Response
                {
                    ResponseStatus = status
                }
            });

            var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => urlService.GetSourceUriAsync(string.Empty));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var urlServiceMock = new Mock<UrlService.UrlServiceClient>();
        urlServiceMock
            .Setup(x => x.GetAsync(It.IsAny<Url>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var urlServiceClientFactoryMock = new Mock<IUrlServiceClientFactory>();
        urlServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(urlServiceMock.Object);

        var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => urlService.GetSourceUriAsync(string.Empty));
    }
}

