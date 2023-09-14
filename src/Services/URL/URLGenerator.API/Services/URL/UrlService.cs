﻿namespace ShortURLGenerator.URLGenerator.API.Services.URL;

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
    public override async Task<UrlResponseDto> Generate(SourceUriDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Generate URL: Start. Source URI: {request.LogInfo()}.");

        UrlResponseDto? response = null;

        while (response is null)
        {
            var url = _generator.GenerateString();

            try
            {
                await _repository.CreateAsync(new Url()
                {
                    Id = url,
                    SourceUri = request.Value
                });

                response = new UrlResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = ResponseStatus.Ok
                    },
                    Url = url
                };

                _logger.LogInformation($"Generate URL: Succesfully. URL response: {response.LogInfo()}.");
            }
            catch (DuplicateWaitObjectException ex)
            {
                _logger.LogWarning(ex, $"Generate URL: {ex.Message}. Source URI: {request.LogInfo()}");
            }
            catch (Exception ex)
            {
                response = new UrlResponseDto()
                {
                    Response = new ResponseDto()
                    {
                        ResponseStatus = ResponseStatus.BadRequest,
                        Error = "Не удалось сгенерировать URL."
                    }
                };

                _logger.LogError(ex, $"Generate URL: {ex.Message}. URL response: {response.LogInfo()}.");
            }
        }

        return response;
    }

    /// <summary>The request method is the original URI of the address at the generated URL. Works on gRPC.</summary>
    /// <param name="request">Source URI request object.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    public override async Task<UriResponseDto> Get(UrlDto request, ServerCallContext context)
    {
        _logger.LogInformation($"Get URI: Start. URL: {request.LogInfo()}.");

        if (await _repository.GetAsync(request.Value) is not { } uri)
        {
            var response = new UriResponseDto()
            {
                Response = new ResponseDto()
                {
                    ResponseStatus = ResponseStatus.NotFound,
                    Error = "Страница не найдена."
                }
            };

            _logger.LogInformation($"Get URI: URL not found. URI response: {response.LogInfo()}.");

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
                Uri = uri
            };

            _logger.LogInformation($"Get URI: Succesfully. URI response: {response.LogInfo()}.");

            return response;
        }
    }
}

