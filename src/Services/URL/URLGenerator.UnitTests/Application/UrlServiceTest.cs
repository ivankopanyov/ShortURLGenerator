namespace ShortURLGenerator.URLGenerator.UnitTests.Application;

public class UrlServiceTest
{
    private const string SOURCE_URI = "http://example.com/example";

    private const string GENERATED_URL = "00000000";

    private readonly Mock<IGeneratable> _generatorMock;

    private readonly Mock<IUrlRepository> _repositoryMock;

    private readonly Mock<ILogger<UrlService>> _loggerMock;

    public UrlServiceTest()
	{
        _generatorMock = new Mock<IGeneratable>();
        _generatorMock.Setup(x => x.GenerateString()).Returns(GENERATED_URL);

        _repositoryMock = new Mock<IUrlRepository>();
        _loggerMock = new Mock<ILogger<UrlService>>();
    }

    [Fact]
    public async Task Generate_success()
    {
        //Arrange
        var request = new SourceUri
        {
            Value = SOURCE_URI
        };

        var serverCallContext = new Mock<ServerCallContext>();
        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Url>())).Returns(Task.CompletedTask);

        //Act
        var urlService = new UrlService(_generatorMock.Object, _repositoryMock.Object, _loggerMock.Object);
        var result = await urlService.Generate(request, serverCallContext.Object);

        //Assert
        Assert.Equal(GENERATED_URL, result?.Url);
        Assert.Equal(result?.Response.ResponseStatus, ResponseStatus.Ok);
    }

    [Fact]
    public async Task Generate_failed()
    {
        //Arrange
        var request = new SourceUri
        {
            Value = SOURCE_URI
        };

        var serverCallContext = new Mock<ServerCallContext>();
        _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<Url>())).Throws(new Exception());

        //Act
        var urlService = new UrlService(_generatorMock.Object, _repositoryMock.Object, _loggerMock.Object);
        var result = await urlService.Generate(request, serverCallContext.Object);

        //Assert
        Assert.Equal(result.Url, string.Empty);
        Assert.Equal(result?.Response.ResponseStatus, ResponseStatus.BadRequest);
        Assert.Equal("Не удалось сгенерировать URL.", result?.Response.Error);
    }


    [Fact]
    public async Task Get_success()
    {
        //Arrange
        var request = new Grpc.Services.Url
        {
            Value = GENERATED_URL
        };

        var serverCallContext = new Mock<ServerCallContext>();
        _repositoryMock
            .Setup(x => x.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<string?>(SOURCE_URI));

        //Act
        var urlService = new UrlService(_generatorMock.Object, _repositoryMock.Object, _loggerMock.Object);
        var result = await urlService.Get(request, serverCallContext.Object);

        //Assert
        Assert.Equal(SOURCE_URI, result.Uri);
        Assert.Equal(result?.Response.ResponseStatus, ResponseStatus.Ok);
    }

    [Fact]
    public async Task Get_failed()
    {
        //Arrange
        var request = new Grpc.Services.Url
        {
            Value = GENERATED_URL
        };

        var serverCallContext = new Mock<ServerCallContext>();
        _repositoryMock
            .Setup(x => x.GetAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<string?>(null));

        //Act
        var urlService = new UrlService(_generatorMock.Object, _repositoryMock.Object, _loggerMock.Object);
        var result = await urlService.Get(request, serverCallContext.Object);

        //Assert
        Assert.Equal(result.Uri, string.Empty);
        Assert.Equal(result?.Response.ResponseStatus, ResponseStatus.NotFound);
        Assert.Equal("Страница не найдена.", result?.Response.Error);
    }
}

