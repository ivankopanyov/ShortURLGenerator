namespace ShortURLGenerator.Web.Bff.UrlGenerator.Dto;

/// <summary>Сlass that describes an object that stores the original URI.</summary>
public class SourceUriDto
{
    /// <summary>Source URI.</summary>
    public string SourceUri { get; set; }

    /// <summary>Overriding the method for converting an object to a string type.</summary>
    /// <returns>Object converted to a string type.</returns>
    public override string ToString() => "Source URI DTO: { Source URI: " + SourceUri + " }";
}
