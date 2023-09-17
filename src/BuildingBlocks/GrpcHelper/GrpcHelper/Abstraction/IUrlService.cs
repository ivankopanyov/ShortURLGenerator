namespace ShortURLGenerator.GrpcHelper.Abstraction;

/// <summary>Service for generating short URLs.</summary>
public interface IUrlService : IUrlGenerator
{
    /// <summary>method to obtain the original URI of an address.</summary>
    /// <param name="url">The generated short URL.</param>
    /// <returns>Source URI.</returns>
    Task<string> GetSourceUriAsync(string url);
}

