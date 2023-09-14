namespace ShortURLGenerator.Identity.API.Models;

/// <summary>Class describing the connection model.</summary>
public class Connection
{
    /// <summary>Connection ID.</summary>
    public string Id { get; set; }

    /// <summary>Connection user ID.</summary>
    public string UserId { get; set; }

    /// <summary>The date and time the connection was created.</summary>
    public DateTime Created { get; set; }

    /// <summary>Connection information.</summary>
    public ConnectionInfoDto ConnectionInfo { get; set; }
}
