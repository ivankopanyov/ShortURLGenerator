namespace ShortURLGenerator.TelegramBot.Services.Url;

/// <summary>
/// Abstract class that describes a service for generating short URLs.
/// Implements the IUrlService interface.
/// The connection is made using gRPC.
/// </summary>
public abstract class UrlServiceBase : IUrlService
{
    /// <summary>Address of the service for generating short URLs.</summary>
    private readonly string _urlServiceHost;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Application configuration.</summary>
    protected IConfiguration AppConfiguration { get; private init; }

    /// <summary>Initialization of the service object for generating short URLs.</summary>
    /// <param name="logger">Log service.</param>
    /// <param name="configuration">Application configuration.</param>
    public UrlServiceBase(ILogger<UrlServiceBase> logger, IConfiguration configuration)
    {
        _logger = logger;
        AppConfiguration = configuration;

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
        if (sourceUri is null)
        {
            _logger.LogError("Generate URL", "Source URI is null");
            throw new ArgumentNullException(nameof(sourceUri));
        }

        _logger.LogStart("Generate URL", sourceUri);

        var request = new SourceUriDto()
        {
            Value = sourceUri
        };

        _logger.LogStart("Send URL", request);

        try
        {
            using var channel = GrpcChannel.ForAddress(_urlServiceHost);
            var client = new Grpc.Services.UrlService.UrlServiceClient(channel);
            var response = await client.GenerateAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogSuccessfully("Send URL", request, response);
                return response.Url;
            }

            if (response.Response.ResponseStatus == ResponseStatus.BadRequest)
                _logger.LogWarning("Send URL", "Bad request", request);
            else
                _logger.LogError("Send URL", response.Response.Error, request, response);

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Send URL", ex.Message, request);
            _logger.LogError("Generate URL", ex.Message, request);
            throw new InvalidOperationException("Нет связи с сервисом генерации ссылок.");
        }
    }

    /// <summary>Abstract method for configuring a short URL generation service.</summary>
    /// <param name="configuration">Configuration object of the service for generating short URLs.</param>
    protected abstract void OnConfiguring(UrlServiceConfiguration configuration);
}

