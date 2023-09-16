namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

public class CreateUrlResponseDto
{
    public string Url { get; set; }

    public override string ToString() => $"URL: {Url}";
}
