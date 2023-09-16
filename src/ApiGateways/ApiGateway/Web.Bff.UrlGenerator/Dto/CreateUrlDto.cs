namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

public class CreateUrlDto
{
    public string SourceUri { get; set; }

    public override string ToString() => $"Source URI: {SourceUri}";
}
