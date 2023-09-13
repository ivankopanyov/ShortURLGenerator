namespace ShortURLGenerator.Identity.API.Models;

public class VerificationCode
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public TimeSpan LifeTime { get; set; }
}
