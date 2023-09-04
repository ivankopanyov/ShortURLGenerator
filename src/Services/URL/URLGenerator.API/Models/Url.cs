namespace ShortURLGenerator.URLGenerator.API.Models;

public class Url : IModel<string>
{
    public string Id { get; set; }

    public DateTime Created { get; set; }

    public string SourceUri { get; set; }

    public override string ToString() => $"URL ID: {Id}\n\tSource URI: {SourceUri}\n\tCreated: {Created.ToString("dd.MM,yyyy HH:mm:ss")}";
}

