namespace ShortURLGenerator.URLGenerator.API.Services.Generation;

/// <summary>Class that describes a url generator configuration.</summary>
public class UrlGeneratorConfiguration
{
    /// <summary>The length of the generated string.</summary>
    public int UrlLength { get; set; }

    /// <summary>String with the character set to generate.</summary>
    public string SourceSymbols { get; set; }
}

