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
        var requestId = request.Value;

        _logger.LogStart("Generate URL", requestId);
        _logger.LogObject("Generate URL", request);

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

                var response = new UrlResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = ResponseStatus.Ok
                    },
                    Url = url
                };

                _logger.LogObject("Generate URL", response);
                _logger.LogSuccessfully($"Generate URL", requestId);

                return response;
            }
            catch (DuplicateWaitObjectException ex)
            {
                _logger.LogWarning(ex, "Generate URL", "Duplicate", requestId);
            }
        }
    }

    /// <summary>The request method is the original URI of the address at the generated URL. Works on gRPC.</summary>
    /// <param name="request">Source URI request object.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    public override async Task<UriResponseDto> Get(UrlDto request, ServerCallContext context)
    {
        var requestId = request.Value;

        _logger.LogStart("Get URI", requestId);
        _logger.LogObject("Get URI", request);

        var url = await _repository.GetAsync(request.Value);

        if (url is null)
        {
            var response = new UriResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.NotFound,
                    Error = "Страница не найдена."
                }
            };

            _logger.LogObject("Get URI", response);
            _logger.LogInformation("Get URI", "Not found", requestId);

            return response;
        }
        else
        {
            var response = new UriResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.Ok
                },
                Uri = url.SourceUri
            };

            _logger.LogObject("Get URI", response);
            _logger.LogSuccessfully("Get URI", requestId);

            return response;
        }
    }
}

