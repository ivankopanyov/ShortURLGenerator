namespace ShortURLGenerator.Web.Bff.UrlGenerator.Controllers;

/// <summary>Сlass that describes a URL controller.</summary>
[ApiController]
[Route("url")]
public class UrlController : ControllerBase
{
    /// <summary>URL service.</summary>
    private readonly IUrlService _urlService;

    /// <summary>The sender of integration events.</summary>
    private readonly IEventBus _eventBus;

    /// <summary>Log service.</summary>
    private readonly ILogger _logger;

    /// <summary>Initializing the controller object.</summary>
    /// <param name="urlService">URL service.</param>
    /// <param name="eventBus">The sender of integration events.</param>
    /// <param name="logger">Log service.</param>
    public UrlController(IUrlService urlService, IEventBus eventBus, ILogger<UrlController> logger)
    {
        _urlService = urlService;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>Endpoint for creating a short URL.</summary>
    /// <param name="createUrl">The request object to create the short URL.</param>
    /// <returns>The result of creating a short URL.</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateUrlResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateUrlResponseDto>> CreateAsync([FromBody] CreateUrlDto createUrl)
    {
        _logger.LogInformation($"Create URL: Start. {createUrl}.");

        if (!Uri.TryCreate(createUrl.SourceUri, UriKind.Absolute, out Uri? uri) || uri is null ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            _logger.LogInformation($"Create URL: Source URI is not valid. {createUrl}.");
            return BadRequest("Некорректная ссылка.");
        }

        try
        {
            var url = await _urlService.GenerateUrlAsync(createUrl.SourceUri);
            var response = new CreateUrlResponseDto()
            {
                Url = url
            };

            _logger.LogInformation($"Create URL: Successfully. {response}.");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Create URL: {ex.Message}. {createUrl}.");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Endpoint for creating a short URL and sending it to a telegram bot.</summary>
    /// <param name="createUrl">The request object to create the short URL.</param>
    /// <returns>The result of creating a short URL.</returns>
    [HttpPost("createAndSend")]
    [ProducesResponseType(typeof(CreateUrlResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<CreateUrlResponseDto>> CreateAndSendAsync([FromBody] CreateUrlDto createUrl)
    {
        _logger.LogInformation($"Create and send URL: Start. {createUrl}.");

        if (!Uri.TryCreate(createUrl.SourceUri, UriKind.Absolute, out Uri? uri) || uri is null ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            _logger.LogInformation($"Create and send URL: Source URI is not valid. {createUrl}.");
            return BadRequest("Некорректная ссылка.");
        }

        try
        {
            var url = await _urlService.GenerateUrlAsync(createUrl.SourceUri);
            SendUrl(url, createUrl.SourceUri);
            var response = new CreateUrlResponseDto()
            {
                Url = url
            };

            _logger.LogInformation($"Create and send URL: Successfully. {response}.");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Create and send URL: {ex.Message}. {createUrl}.");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Endpoint to get the original URI from a short generated URL.</summary>
    /// <param name="url">Generated URL.</param>
    /// <returns>Result of the request.</returns>
    [HttpGet("get/{url}")]
    [ProducesResponseType(typeof(SourceUriDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SourceUriDto>> GetAsync([FromRoute] string url)
    {
        _logger.LogInformation($"Get source URI: Start. URL: {url}.");

        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogInformation($"Get source URI: URL is null or whitespace. URL: {url}.");
            return BadRequest("Некорректный адрес.");
        }
        
        try
        {
            var sourceUri = await _urlService.GetSourceUriAsync(url);
            var response = new SourceUriDto()
            {
                SourceUri = sourceUri
            };

            _logger.LogInformation($"Get source URI: Successfully. {response}.");

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Get source URI: {ex.Message}. URL: {url}.");
            return NotFound(ex.Message);
        }
    }

    /// <summary>Method for sending a short generated URL to a Telegram bot. </summary>
    /// <param name="url">Generated URL.</param>
    /// <param name="sourceUri">Source URI.</param>
    private void SendUrl(string url, string sourceUri)
    {
        _logger.LogInformation($"Send URL: Start. URL: {url}, Source URI: {sourceUri}");

        if (User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) is { } idClaim && long.TryParse(idClaim.Value, out long id))
        {
            var @event = new UriGeneratedIntegrationEvent(id, url, sourceUri);

            try
            {
                _eventBus.Publish(@event);
                _logger.LogInformation($"Send URL: Succesfully. {@event}.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Send URL: {ex.Message}. {@event}.");
            }
        }
        else
            _logger.LogError($"Send URL: Claim ID is not valid.");
    }
}
