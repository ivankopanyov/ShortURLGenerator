namespace ShortURLGenerator.URLGenerator.API.Extensions;

/// <summary>Static class of extensions for logging information about objects.</summary>
public static class LoggerExtension
{
    /// <summary>Method for obtaining information about an object of the SourceUriDto class.</summary>
    /// <param name="sourceUri">Object of the SourceUriDto class.</param>
    /// <returns>Information about an object of the SourceUriDto class.</returns>
    public static string LogInfo(this SourceUriDto sourceUri) => $"Source URI: {sourceUri.Value}";

    /// <summary>Method for obtaining information about an object of the ResponseDto class.</summary>
    /// <param name="response">Object of the ResponseDto class.</param>
    /// <returns>Information about an object of the ResponseDto class.</returns>
    public static string LogInfo(this ResponseDto response) =>
        $"Status: {response.ResponseStatus}, " +
        $"Error message: {response.Error}";

    /// <summary>Method for obtaining information about an object of the UrlResponseDto class.</summary>
    /// <param name="urlResponse">Object of the UrlResponseDto class.</param>
    /// <returns>Information about an object of the UrlResponseDto class.</returns>
    public static string LogInfo(this UrlResponseDto urlResponse) =>
        $"Response: {urlResponse.Response.LogInfo()}, " +
        $"URL: {urlResponse.Url}";

    /// <summary>Method for obtaining information about an object of the UrlDto class.</summary>
    /// <param name="url">Object of the UrlDto class.</param>
    /// <returns>Information about an object of the UrlDto class.</returns>
    public static string LogInfo(this UrlDto url) => $"Url: {url.Value}";

    /// <summary>Method for obtaining information about an object of the UriResponseDto class.</summary>
    /// <param name="uriResponse">Object of the UriResponseDto class.</param>
    /// <returns>Information about an object of the UriResponseDto class.</returns>
    public static string LogInfo(this UriResponseDto uriResponse) =>
        $"Response: {uriResponse.Response.LogInfo()}, " +
        $"URI: {uriResponse.Uri}";
}

