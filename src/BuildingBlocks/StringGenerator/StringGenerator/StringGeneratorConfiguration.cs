namespace ShortURLGenerator.StringGenerator;

/// <summary>Class that describes a string generator configuration.</summary>
public class StringGeneratorConfiguration
{
    /// <summary>The length of the generated string.</summary>
    public int StringLength { get; set; }

    /// <summary>String with the character set to generate.</summary>
    public string SourceSymbols { get; set; }
}