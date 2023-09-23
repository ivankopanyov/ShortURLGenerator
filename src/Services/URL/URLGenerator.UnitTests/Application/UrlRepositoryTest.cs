namespace ShortURLGenerator.URLGenerator.UnitTests.Application;

public class UrlRepositoryTest
{
    private const string GENERATED_URL = "00000000";

    private const string SOURCE_URI = "http://example.com/example";

    private readonly Mock<IDistributedCache> _distributedCacheMock;

    private readonly Mock<ILogger<UrlRepository>> _loggerMock;

    public UrlRepositoryTest()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _loggerMock = new Mock<ILogger<UrlRepository>>();
    }

    [Fact]
    public async Task Create_success()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);
        await urlRepository.CreateAsync(new Url
        {
            Id = GENERATED_URL,
            SourceUri = SOURCE_URI
        });

        var result = await urlRepository.GetAsync(GENERATED_URL);

        Assert.Equal(SOURCE_URI, result);
    }

    [Fact]
    public async Task Create_item_is_null_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => urlRepository.CreateAsync(null!));
    }

    [Fact]
    public async Task Create_item_id_is_null_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => urlRepository.CreateAsync(new Url
        {
            Id = null!,
            SourceUri = SOURCE_URI
        }));
    }

    [Fact]
    public async Task Create_item_id_is_empty_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => urlRepository.CreateAsync(new Url
        {
            Id = string.Empty,
            SourceUri = SOURCE_URI
        }));
    }

    [Fact]
    public async Task Create_item_id_is_whitespace_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => urlRepository.CreateAsync(new Url
        {
            Id = Strings.StrDup(Random.Shared.Next(1, 10), ' '),
            SourceUri = SOURCE_URI
        }));
    }

    [Fact]
    public async Task Create_item_source_uri_is_null_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => urlRepository.CreateAsync(new Url
        {
            Id = GENERATED_URL,
            SourceUri = null!
        }));
    }

    [Fact]
    public async Task Create_item_source_uri_is_empty_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => urlRepository.CreateAsync(new Url
        {
            Id = GENERATED_URL,
            SourceUri = string.Empty
        }));
    }

    [Fact]
    public async Task Create_item_source_uri_is_whitespace_failed()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => urlRepository.CreateAsync(new Url
        {
            Id = GENERATED_URL,
            SourceUri = Strings.StrDup(Random.Shared.Next(1, 10), ' ')
        }));
    }

    [Fact]
    public async Task Create_item_duplicate_failed()
    {
        using (var urlContextMock = UrlContextMock.Empty)
        {
            urlContextMock.Urls.Add(new Url
            {
                Id = GENERATED_URL,
                SourceUri = SOURCE_URI
            });

            urlContextMock.SaveChanges();
        }

        var urlRepository = new UrlRepository(new UrlContextMock(), _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<DuplicateWaitObjectException>(() => urlRepository.CreateAsync(new Url
        {
            Id = GENERATED_URL,
            SourceUri = SOURCE_URI
        }));
    }

    [Fact]
    public async Task Create_invalid_operation_failed()
    {
        var urlsMock = Enumerable.Empty<Url>().BuildMock().BuildMockDbSet();
        var urlContextMock = new Mock<UrlContext>();
        urlContextMock.Setup(x => x.Urls).Returns(urlsMock.Object);
        urlContextMock.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new Exception());

        var urlRepository = new UrlRepository(urlContextMock.Object, _distributedCacheMock.Object, _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => urlRepository.CreateAsync(new Url
        {
            Id = GENERATED_URL,
            SourceUri = SOURCE_URI
        }));
    }

    [Fact]
    public async Task Get_from_cache_success()
    {
        var distributedCacheMock = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        await distributedCacheMock.SetStringAsync(GENERATED_URL, SOURCE_URI);

        var urlRepository = new UrlRepository(UrlContextMock.Empty, distributedCacheMock, _loggerMock.Object);
        var result = await urlRepository.GetAsync(GENERATED_URL);

        Assert.Equal(SOURCE_URI, result);
    }

    [Fact]
    public async Task Get_from_db_success()
    {
        using (var context = UrlContextMock.Empty)
        {
            context.Urls.Add(new Url
            {
                Id = GENERATED_URL,
                SourceUri = SOURCE_URI
            });

            context.SaveChanges();
        }

        var urlRepository = new UrlRepository(new UrlContextMock(), _distributedCacheMock.Object, _loggerMock.Object);
        var result = await urlRepository.GetAsync(GENERATED_URL);

        Assert.Equal(SOURCE_URI, result);
    }

    [Fact]
    public async Task Get_returns_null()
    {
        var urlRepository = new UrlRepository(UrlContextMock.Empty, _distributedCacheMock.Object, _loggerMock.Object);
        var result = await urlRepository.GetAsync(GENERATED_URL);

        Assert.Null(result);
    }
}

