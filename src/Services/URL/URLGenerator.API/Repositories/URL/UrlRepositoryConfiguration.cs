namespace ShortURLGenerator.URLGenerator.API.Repositories.URL;

/// <summary>Class that describes the repository URL configuration.</summary>
public class UrlRepositoryConfiguration
{
    /// <summary>How long the URL has been cached since the last request.</summary>
    public TimeSpan LifeTimeCache { get; set; }
}

