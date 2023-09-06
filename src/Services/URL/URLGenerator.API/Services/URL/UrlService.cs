namespace ShortURLGenerator.URLGenerator.API.Services.URL;

/// <summary>Service for generating short URLs. Works on gRPC.</summary>
public class UrlService : Grpc.Services.UrlService.UrlServiceBase, IUrlService
{
    /// <summary>Random string generator.</summary>
    private readonly IGeneratable _generator;

    /// <summary>Repository of generated URLs.</summary>
    private readonly IUrlRepository _repository;

    /// <summary>Log service.</summary>
    private readonly ILogger<UrlService> _logger;

    /// <summary>Initialization of the service object for generating short URLs.</summary>
    /// <param name="generator">Random string generator.</param>
    /// <param name="repository">Repository of generated URLs.</param>
    /// <param name="logger">Log service.</param>
    public UrlService(IGeneratable generator, IUrlRepository repository, ILogger<UrlService> logger)
	{
        _generator = generator;
        _repository = repository;
        _logger = logger;
	}

    /// <summary>Short URL generation method. Works on gRPC.</summary>
    /// <param name="request">The request object for generating a short URL.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    public override async Task<UrlResponseDto> Generate(SourceUriDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Generate URL: start.\n\tSource URI: {request.Value}");

        while (true)
        {
            var url = _generator.GenerateString();

            try
            {
                await _repository.CreateAsync(new Url()
                {
                    Id = url,
                    SourceUri = request.Value
                });

                _logger.LogInformation($"Generate URL: succesful.\n\tSource URI: {request.Value}\n\tURL: {url}");

                return new UrlResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = ResponseStatus.Ok
                    },
                    Url = url
                };
            }
            catch (DuplicateWaitObjectException ex)
            {
                _logger.LogWarning(ex, $"Generate URL: failed.\n\tSource URI: {request.Value}\n\tURL: {url}\n\tWarning: duplicate.");
            }
        }
    }

    /// <summary>The request method is the original URI of the address at the generated URL. Works on gRPC.</summary>
    /// <param name="request">Source URI request object.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    public override async Task<UriResponseDto> Get(UrlDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Get URI: start.\n\tID: {request.Value}");

        var url = await _repository.GetAsync(request.Value);

        if (url is null)
        {
            _logger.LogInformation($"Get URI: failed.\n\tID: {request.Value}\n\tError: URL not found.");
            return new UriResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.NotFound,
                    Error = "Страница не найдена."
                }
            };
        }

        _logger.LogInformation($"Get URI: succesful.\n\tID: {request.Value}\n\tURL: {url}");

        return new UriResponseDto()
        {
            Response = new ResponseDto()
            {
                ResponseStatus = ResponseStatus.Ok
            },
            Uri = url.SourceUri
        };
    }
}

