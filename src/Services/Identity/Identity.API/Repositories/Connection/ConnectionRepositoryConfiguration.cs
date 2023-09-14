namespace ShortURLGenerator.Identity.API.Repositories.Connection;

/// <summary>Class that describes the connection repository configuration.</summary>
public class ConnectionRepositoryConfiguration
{
    /// <summary>The length of time the connection is stored in the repository.</summary>
    public TimeSpan ConnectionLifeTime { get; set; }
}
