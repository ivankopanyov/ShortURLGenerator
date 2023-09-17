namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

public class SourceUriDto
{
    public string SourceUri { get; set; }

    public override string ToString() => "Source URI DTO: { Source URI: " + SourceUri + " }";
}
