namespace ShortURLGenerator.URLGenerator.API.Services.URL;

/// <summary>Service for generating short URLs.</summary>
public interface IUrlService
{
    /// <summary>Short URL generation method.</summary>
    /// <param name="request">The request object for generating a short URL.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    Task<UrlResponseDto> Generate(SourceUriDto request, ServerCallContext context);

    /// <summary>The request method is the original URI of the address at the generated URL.</summary>
    /// <param name="request">Source URI request object.</param>
    /// <param name="context">Server call context.</param>
    /// <returns>Response object containing the response status and data.</returns>
    Task<UriResponseDto> Get(UrlDto request, ServerCallContext context);
}

