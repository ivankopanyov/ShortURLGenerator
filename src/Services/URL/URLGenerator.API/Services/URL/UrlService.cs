namespace ShortURLGenerator.URLGenerator.API.Services.URL;

/// <summary>
/// Service for generating short URLs.
/// Inherited from the class Grpc.Services.UrlService.UrlServiceBase.
/// Works on gRPC.
/// </summary>
public class UrlService : Grpc.Services.UrlService.UrlServiceBase
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
    public override async Task<UrlResponse> Generate(SourceUri request, ServerCallContext context)
    {
        _logger.LogInformation($"Generate URL: Start. {request.LogInfo()}.");

        UrlResponse? response = null;

        while (response is null)
        {
            var url = _generator.GenerateString();

            try
            {
                await _repository.CreateAsync(new Models.Url()
                {
                    Id = url,
                    SourceUri = request.Value
                });

                response = new UrlResponse()
                {
                    Response = new Response()
                    {
                        ResponseStatus = ResponseStatus.Ok
                    },
                    Url = url
                };

                _logger.LogInformation($"Generate URL: Succesfully. {response.LogInfo()}.");
            }
            catch (DuplicateWaitObjectException ex)
            {
                _logger.LogWarning(ex, $"Generate URL: {ex.Message}. {request.LogInfo()}");
            }
            catch (Exception ex)
            {
                response = new UrlResponse()
                {
                    Response = new Response()
                    {
                        ResponseStatus = ResponseStatus.BadRequest,
                        Error = "Не удалось сгенерировать URL."
                    }
                };

                _logger.LogError(ex, $"Generate URL: {ex.Message}. {response.LogInfo()}.");
            }
        }

        return response;
    }

    /// <summary>The request method is the original URI of the address at the generated URL. Works on gRPC.</summary>
    /// <param name="request">Source URI request object.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    public override async Task<UriResponse> Get(Url request, ServerCallContext context)
    {
        _logger.LogInformation($"Get URI: Start. {request.LogInfo()}.");

        if (await _repository.GetAsync(request.Value) is not { } uri)
        {
            var response = new UriResponse()
            {
                Response = new Response()
                {
                    ResponseStatus = ResponseStatus.NotFound,
                    Error = "Страница не найдена."
                }
            };

            _logger.LogInformation($"Get URI: URL not found. {response.LogInfo()}.");

            return response;
        }
        else
        {
            var response = new UriResponse()
            {
                Response = new Response()
                {
                    ResponseStatus = ResponseStatus.Ok
                },
                Uri = uri
            };

            _logger.LogInformation($"Get URI: Succesfully. {response.LogInfo()}.");

            return response;
        }
    }
}

