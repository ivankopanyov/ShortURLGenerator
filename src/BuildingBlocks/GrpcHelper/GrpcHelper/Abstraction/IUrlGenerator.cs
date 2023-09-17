namespace ShortURLGenerator.GrpcHelper.Abstraction;

/// <summary>Service for generating short URLs.</summary>
public interface IUrlGenerator
{
    /// <summary>Short URL generation method.</summary>
    /// <param name="sourceUri">Source URI.</param>
    /// <returns>The generated short URL.</returns>
    Task<string> GenerateUrlAsync(string sourceUri);
}

