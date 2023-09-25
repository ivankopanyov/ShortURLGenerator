namespace ShortURLGenerator.GrpcHelperTest.Application.UrlServiceTest;

public class GenerateUrlTest : UrlServiceTestBase
{
    private static Mock<IUrlServiceClientFactory> GetUrlServiceClientFactoryMock(UrlResponse response)
    {
        var urlServiceMock = new Mock<UrlService.UrlServiceClient>();
        urlServiceMock
            .Setup(x => x.GenerateAsync(It.IsAny<SourceUri>(), null, null, default))
            .Returns(new AsyncUnaryCall<UrlResponse>(Task.FromResult(response),
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
        var response = new UrlResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            Url = "RandomString"
        };

        var urlServiceClientFactoryMock = GetUrlServiceClientFactoryMock(response);

        var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

        var result = await urlService.GenerateUrlAsync("https://example.com/example");

        Assert.Equal(response.Url, result);
    }

    [Fact]
    public async Task Source_uri_is_null_failed()
    {
        var urlServiceClientFactoryMock = GetUrlServiceClientFactoryMock(new UrlResponse
        {
            Response = new Response
            {
                ResponseStatus = ResponseStatus.Ok
            },
            Url = string.Empty
        });

        var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => urlService.GenerateUrlAsync(null!));
    }

    [Fact]
    public async Task Failed()
    {
        foreach (var status in Enum.GetValues<ResponseStatus>())
        {
            if (status == ResponseStatus.Ok)
                continue;

            var urlServiceClientFactoryMock = GetUrlServiceClientFactoryMock(new UrlResponse
            {
                Response = new Response
                {
                    ResponseStatus = status
                }
            });

            var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => urlService.GenerateUrlAsync(string.Empty));
        }
    }

    [Fact]
    public async Task Rpc_exception_failed()
    {
        var urlServiceMock = new Mock<UrlService.UrlServiceClient>();
        urlServiceMock
            .Setup(x => x.GenerateAsync(It.IsAny<SourceUri>(), null, null, default))
            .Throws(new RpcException(Status.DefaultSuccess));

        var urlServiceClientFactoryMock = new Mock<IUrlServiceClientFactory>();
        urlServiceClientFactoryMock
            .Setup(x => x.New(It.IsAny<ChannelBase?>()!))
            .Returns(urlServiceMock.Object);

        var urlService = new GrpcHelper.Services.URL.UrlService(_grpcChannelFactoryMock.Object,
            urlServiceClientFactoryMock.Object,
            _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => urlService.GenerateUrlAsync(string.Empty));
    }
}

