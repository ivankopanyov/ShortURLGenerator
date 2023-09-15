namespace ShortURLGenerator.TelegramBot.Services.Url;

/// <summary>
/// Class that describes a service for generating short URLs.
/// Implements the IUrlService interface.
/// The connection is made using gRPC.
/// </summary>
public class UrlService : IUrlService
{
    /// <summary>Address of the service for generating short URLs.</summary>
    private readonly string _urlServiceHost;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Initialization of the service object for generating short URLs.</summary>
    /// <param name="logger">Log service.</param>
    public UrlService(ILogger<UrlService> logger)
    {
        _logger = logger;

        var urlServiceConfiguration = new UrlServiceConfiguration();
        OnConfiguring(urlServiceConfiguration);

        _urlServiceHost = urlServiceConfiguration.UrlServiceHost;
    }

    /// <summary>Short URL generation method. The connection is made using gRPC.</summary>
    /// <param name="sourceUri">Source URI.</param>
    /// <returns>The generated short URL.</returns>
    /// <exception cref="ArgumentNullException">Source URI is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<string> GenerateUrlAsync(string sourceUri)
    {
        _logger.LogInformation($"Generate URL: Start. Source URI: {sourceUri}");

        if (sourceUri is null)
        {
            _logger.LogError($"Generate URL: Source URI is null.");
            throw new ArgumentNullException(nameof(sourceUri));
        }

        var request = new SourceUriDto()
        {
            Value = sourceUri
        };

        try
        {
            using var channel = GrpcChannel.ForAddress(_urlServiceHost);
            var client = new Grpc.Services.UrlService.UrlServiceClient(channel);
            var response = await client.GenerateAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Generate URL: Successfully. Request: {request.LogInfo()}, Response: {response.LogInfo()}");
                return response.Url;
            }

            if (response.Response.ResponseStatus == ResponseStatus.BadRequest)
                _logger.LogInformation($"Generate URL: Bad request. Request: {request.LogInfo()}, Response: {response.LogInfo()}");
            else
                _logger.LogError($"Generate URL: Error. Request: {request.LogInfo()}, Response: {response.LogInfo()}");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Generate URL: {ex.Message}. Request: {request.LogInfo()}");
            throw new InvalidOperationException("Нет связи с сервисом генерации ссылок.");
        }
    }

    /// <summary>Virtual method for configuring a short URL generation service.</summary>
    /// <param name="configuration">Configuration object of the service for generating short URLs.</param>
    protected virtual void OnConfiguring(UrlServiceConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("URL_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable("URL_SERVICE_PORT")!;
        configuration.UrlServiceHost = $"http://{host}:{port}";
    }
}

