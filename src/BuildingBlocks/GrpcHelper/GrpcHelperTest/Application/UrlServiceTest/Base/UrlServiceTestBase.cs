namespace ShortURLGenerator.GrpcHelperTest.Application.UrlServiceTest.Base;

public abstract class UrlServiceTestBase
{
    protected readonly Mock<IGrpcChannelFactory> _grpcChannelFactoryMock;

    protected readonly Mock<ILogger<GrpcHelper.Services.URL.UrlService>> _loggerMock;

    public UrlServiceTestBase()
    {
        _loggerMock = new Mock<ILogger<GrpcHelper.Services.URL.UrlService>>();

        _grpcChannelFactoryMock = new Mock<IGrpcChannelFactory>();
        _grpcChannelFactoryMock
            .Setup(x => x.ForAddress(It.IsAny<string>()))
            .Returns(() => null!);
    }
}

