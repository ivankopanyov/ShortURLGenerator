namespace ShortURLGenerator.QRCreator.UnitTests.Application;

public class QRCodeCreationServiceTest
{
	private readonly Mock<ILogger<QRCodeCreationService>> _loggerMock;

    public QRCodeCreationServiceTest()
	{
        _loggerMock = new Mock<ILogger<QRCodeCreationService>>();
    }

	[Fact]
	public void Generate_jpeg_success()
	{
		var qRCodeCreationService = new QRCodeCreationService(_loggerMock.Object);

		var result = qRCodeCreationService.GenerateJpeg("https://example.com");

		Assert.NotNull(result);
    }

    [Fact]
    public void Generate_jpeg_uri_is_null_failed()
    {
        var qRCodeCreationService = new QRCodeCreationService(_loggerMock.Object);

        Assert.Throws<ArgumentNullException>(() => qRCodeCreationService.GenerateJpeg(null!));
    }
}

