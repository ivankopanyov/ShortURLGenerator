namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

/// <summary>Repository of generated short URLs.</summary>
public interface IUrlRepository
{
    /// <summary>Method for adding a new URL to the repository.</summary>
    /// <param name="item">URL address.</param>
	Task CreateAsync(Url item);

    /// <summary>Method for requesting source URI from a repository.</summary>
    /// <param name="id">URL identifier.</param>
    /// <returns>Source URI.</returns>
    Task<string?> GetAsync(string id);
}

