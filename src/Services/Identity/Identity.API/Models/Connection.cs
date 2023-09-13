namespace ShortURLGenerator.Identity.API.Models;

public class Connection
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public DateTime Created { get; set; }

    public ConnectionInfoDto ConnectionInfo { get; set; }
}
