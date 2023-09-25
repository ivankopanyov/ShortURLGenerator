namespace ShortURLGenerator.GrpcHelperTest.Application.IdentityServiceTest.Base;

public abstract class IdentityServiceTestBase
{
    protected readonly Mock<IGrpcChannelFactory> _grpcChannelFactoryMock;

    protected readonly Mock<ILogger<GrpcHelper.Services.Identity.IdentityService>> _loggerMock;

    public IdentityServiceTestBase()
    {
        _loggerMock = new Mock<ILogger<GrpcHelper.Services.Identity.IdentityService>>();

        _grpcChannelFactoryMock = new Mock<IGrpcChannelFactory>();
        _grpcChannelFactoryMock
            .Setup(x => x.ForAddress(It.IsAny<string>()))
            .Returns(() => null!);
    }
}

