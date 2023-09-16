namespace ShortURLGenerator.Web.Bff.UrlGenerator.Controllers;

[ApiController]
[Route("url")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;

    public UrlController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateUrlResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateUrlResponseDto>> CreateAsync([FromBody] CreateUrlDto createUrl)
    {
        if (!Uri.TryCreate(createUrl.SourceUri, UriKind.Absolute, out Uri? uri) || uri is null ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return BadRequest("Некорректная ссылка.");

        try
        {
            var url = await _urlService.GenerateUrlAsync(createUrl.SourceUri);
            return Ok(new CreateUrlResponseDto()
            {
                Url = url
            });
        }
        catch (InvalidOperationException ex)
        { 
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("get/{url}")]
    [ProducesResponseType(typeof(Dto.SourceUriDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Dto.SourceUriDto>> GetAsync([FromRoute] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("Некорректный адрес.");
        
        try
        {
            var sourceUri = await _urlService.GetSourceUriAsync(url);
            return Ok(new Dto.SourceUriDto()
            {
                SourceUri = sourceUri
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
