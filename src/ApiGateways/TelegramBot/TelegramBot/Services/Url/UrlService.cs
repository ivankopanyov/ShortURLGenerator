namespace ShortURLGenerator.TelegramBot.Services.Url;

/// <summary>Service for generating short URLs. The connection is made using gRPC.</summary>
public class UrlService : IUrlService
{
    /// <summary>Address of the service for generating short URLs.</summary>
    private static readonly string _urlServiceHost =
        "http://" + Environment.GetEnvironmentVariable("URI_SERVICE_HOST")! +
        ":" + Environment.GetEnvironmentVariable("URI_SERVICE_PORT")!;

    /// <summary>Log service.</summary>
    private readonly ILogger<UrlService> _logger;

    /// <summary>Initialization of the service object for generating short URLs.</summary>
    /// <param name="logger">Log service.</param>
    public UrlService(ILogger<UrlService> logger)
    {
        _logger = logger;
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
        if (sourceUri is null)
            throw new ArgumentNullException(nameof(sourceUri));

        _logger.LogInformation($"Generate URL.\n\tSource URI: {sourceUri}");

        var request = new SourceUriDto()
        {
            Value = sourceUri
        };

        _logger.LogInformation($"Generate URL: Connection to UrlService: {_urlServiceHost}\n\tSource URI: {sourceUri}");

        try
        {
            using var channel = GrpcChannel.ForAddress(_urlServiceHost);
            var client = new Grpc.Services.UrlService.UrlServiceClient(channel);
            var response = await client.GenerateAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Generate URL: succesful.\n\tSource URI: {sourceUri}\n\tResult URL: {response.Url}");
                return response.Url;
            }

            if (response.Response.ResponseStatus == ResponseStatus.BadRequest)
                _logger.LogInformation($"Generate URL: failed.\n\tSource URI: {sourceUri}\n\tError: {response.Response.Error}");
            else
                _logger.LogError($"Generate URL: failed.\n\tSource URI: {sourceUri}\n\tError: {response.Response.ResponseStatus} {response.Response.Error}");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Generate URL: failed.\n\tSource URI: {sourceUri}\n\tError: No connection with UriService {_urlServiceHost}");
            throw new InvalidOperationException("Нет связи с сервисом генерации ссылок.");
        }
    }
}

