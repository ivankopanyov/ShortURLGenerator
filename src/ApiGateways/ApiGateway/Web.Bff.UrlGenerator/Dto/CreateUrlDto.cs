namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

/// <summary>Class that describes a request to create a short URL.</summary>
public class CreateUrlDto
{
    /// <summary>Source URI.</summary>
    public string SourceUri { get; set; }

    /// <summary>Overriding the method for converting an object to a string type.</summary>
    /// <returns>Object converted to a string type.</returns>
    public override string ToString() => "Create URL DTO: { Source URI: " + SourceUri + " }";
}
