namespace ShortURLGenerator.Identity.API.Models;

public class Connection
{
    public string UserId { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    public ConnectionInfoDto ConnectionInfo { get; set; }
}
