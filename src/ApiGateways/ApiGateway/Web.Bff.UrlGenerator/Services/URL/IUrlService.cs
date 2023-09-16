namespace ShortURLGenerator.Web.Bff.UrlGenerator.Services.URL;

/// <summary>Service for generating short URLs.</summary>
public interface IUrlService
{
    /// <summary>Short URL generation method.</summary>
    /// <param name="sourceUri">Source URI.</param>
    /// <returns>The generated short URL.</returns>
    Task<string> GenerateUrlAsync(string sourceUri);

    /// <summary>method to obtain the original URI of an address.</summary>
    /// <param name="url">The generated short URL.</param>
    /// <returns>Source URI.</returns>
    Task<string> GetSourceUriAsync(string url);
}
