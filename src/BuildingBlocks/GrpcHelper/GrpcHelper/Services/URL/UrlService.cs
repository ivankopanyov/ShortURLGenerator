namespace ShortURLGenerator.GrpcHelper.Services.URL;

/// <summary>
/// Class that describes a service for generating short URLs.
/// The connection is made using gRPC.
/// </summary>
public class UrlService : IUrlService
{
    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>GRPC channel object factory.</summary>
    private readonly IGrpcChannelFactory _grpcChannelFactory;

    /// <summary>URL service client factory.</summary>
    private readonly IUrlServiceClientFactory _urlServiceClientFactory;

    /// <summary>Address of the service for generating short URLs.</summary>
    private readonly string _urlServiceHost;

    /// <summary>Initialization of the service object for generating short URLs.</summary>
    /// <param name="grpcChannelFactory">GRPC channel object factory.</param>
    /// <param name="urlServiceClientFactory">URL service client factory.</param>
    /// <param name="logger">Log service.</param>
    public UrlService(IGrpcChannelFactory grpcChannelFactory,
        IUrlServiceClientFactory urlServiceClientFactory,
        ILogger<UrlService> logger)
    {
        _grpcChannelFactory = grpcChannelFactory;
        _urlServiceClientFactory = urlServiceClientFactory;
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
        if (sourceUri is null)
        {
            _logger.LogError($"Generate URL: Source URI is null.");
            throw new ArgumentNullException(nameof(sourceUri));
        }

        var request = new SourceUri()
        {
            Value = sourceUri
        };

        _logger.LogInformation($"Generate URL: Start. {request.LogInfo()}");

        try
        {
            using var channel = _grpcChannelFactory.ForAddress(_urlServiceHost);
            var client = _urlServiceClientFactory.New(channel);
            var response = await client.GenerateAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Generate URL: Successfully. {response.LogInfo()}");
                return response.Url;
            }

            if (response.Response.ResponseStatus == ResponseStatus.BadRequest)
                _logger.LogInformation($"Generate URL: {response.Response.Error}. {response.LogInfo()}");
            else
                _logger.LogError($"Generate URL: {response.Response.Error}. {response.LogInfo()}");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Generate URL: {ex.Message}. Request: {request.LogInfo()}");
            throw new InvalidOperationException("Нет связи с сервисом генерации ссылок.");
        }
    }

    /// <summary>method to obtain the original URI of an address. The connection is made using gRPC.</summary>
    /// <param name="url">The generated short URL.</param>
    /// <returns>Source URI.</returns>
    /// <exception cref="ArgumentNullException">The generated short URL is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Failed to complete the request to the service or the request is invalid.
    /// </exception>
    public async Task<string> GetSourceUriAsync(string url)
    {
        if (url is null)
        {
            _logger.LogError($"Get source URI: URL is null.");
            throw new ArgumentNullException(nameof(url));
        }

        var request = new Url()
        {
            Value = url
        };

        _logger.LogInformation($"Get source URI: Start. {request.LogInfo()}");

        try
        {
            using var channel = _grpcChannelFactory.ForAddress(_urlServiceHost);
            var client = _urlServiceClientFactory.New(channel);
            var response = await client.GetAsync(request);

            if (response.Response.ResponseStatus == ResponseStatus.Ok)
            {
                _logger.LogInformation($"Get source URI: Successfully. {response.LogInfo()}");
                return response.Uri;
            }

            if (response.Response.ResponseStatus == ResponseStatus.NotFound)
                _logger.LogInformation($"Get source URI: {response.Response.Error}. {response.LogInfo()}");
            else
                _logger.LogError($"Get source URI: {response.Response.Error}. {response.LogInfo()}");

            throw new InvalidOperationException(response.Response.Error);
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, $"Get source URI: {ex.Message}.");
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