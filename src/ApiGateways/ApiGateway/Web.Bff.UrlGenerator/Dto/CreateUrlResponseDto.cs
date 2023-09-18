namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

/// <summary>Class that describes the response to a request to create a short URL.</summary>
public class CreateUrlResponseDto
{
    /// <summary>Generated short URL.</summary>
    public string Url { get; set; }

    /// <summary>Overriding the method for converting an object to a string type.</summary>
    /// <returns>Object converted to a string type.</returns>
    public override string ToString() => "Create URL response DTO: { URL: " + Url + " }";
}
